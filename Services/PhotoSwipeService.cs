using Cascade.PhotoSwipe.Models;
using Orchard;
using Orchard.Autoroute.Models;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.MediaLibrary.Models;
using Orchard.Tags.Models;
using Orchard.Tags.Services;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Cascade.PhotoSwipe.Services
{
    public interface IPhotoSwipeService : IDependency
    {
        IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int count);
        IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int count, VersionOptions versionOptions);
        int GetTaggedContentItemCount(int tagId);
        int GetTaggedContentItemCount(int tagId, VersionOptions versionOptions);

        IEnumerable<TagCount> GetPopularTags(int buckets, MediaTagCloudPart part);
    }

    public class PhotoSwipeService : IPhotoSwipeService
    {
        readonly private IContentManager _cm;
        readonly private ITagService _tagService;
        readonly private IRepository<ContentTagRecord> _contentTagRepository;
        readonly private IOrchardServices _orchardServices;

        private readonly IRepository<AutoroutePartRecord> _autorouteRepository;
        private readonly IContentManager _contentManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        internal static readonly string FogTagsChanged = "Cascade.PhotoSwipe.MediaTagCloud.TagsChanged";

        public PhotoSwipeService(IContentManager cm, ITagService tagService, IRepository<ContentTagRecord> contentTagRepository, IOrchardServices orchardServices, IRepository<AutoroutePartRecord> autorouteRepository,
            IContentManager contentManager,
            ICacheManager cacheManager,
            ISignals signals)
        {
            _cm = cm;
            _tagService = tagService;
            _contentTagRepository = contentTagRepository;
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _signals = signals;
            _autorouteRepository = autorouteRepository;
            _cacheManager = cacheManager;
        }

        public IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int take)
        {
            return GetTaggedContentItems(tagId, skip, take, VersionOptions.Published);
        }

        public IEnumerable<IContent> GetTaggedContentItems(int tagId, int skip, int take, VersionOptions options)
        {
            return _orchardServices.ContentManager
                .Query<TagsPart, TagsPartRecord>()
                .Where(tpr => tpr.Tags.Any(tr => tr.TagRecord.Id == tagId))
                .Join<CommonPartRecord>().OrderByDescending(x => x.CreatedUtc)
                .Join<MediaPartRecord>()
                .Slice(skip, take);
        }

        public int GetTaggedContentItemCount(int tagId)
        {
            return GetTaggedContentItemCount(tagId, VersionOptions.Published);
        }

        public int GetTaggedContentItemCount(int tagId, VersionOptions options)
        {
            return _orchardServices.ContentManager
                .Query<TagsPart, TagsPartRecord>()
                .Where(tpr => tpr.Tags.Any(tr => tr.TagRecord.Id == tagId))
                .Join<MediaPartRecord>()
                .Count();
        }

        public IEnumerable<TagCount> GetPopularTags(int buckets, MediaTagCloudPart part)
        {
            var cacheKey = "Cascade.PhotoSwipe.MediaTagCloud." + (part.Slug ?? "") + '.' + buckets;
            return _cacheManager.Get(cacheKey,
                ctx =>
                {
                    ctx.Monitor(_signals.When(FogTagsChanged));
                    IEnumerable<TagCount> tagCounts;
                    if (string.IsNullOrWhiteSpace(part.Slug))
                    {
                        var tagCountsQuery = (from tc in _contentTagRepository.Table
                                              where tc.TagsPartRecord.ContentItemRecord.Versions.Any(v => v.Published)
                                              //&& tc.TagsPartRecord.ContentItemRecord.ContentType.Name == "Image"
                                              group tc by tc.TagRecord.TagName
                                                  into g
                                                  select new TagCount
                                                  {
                                                      TagName = g.Key,
                                                      Count = g.Count()
                                                  });

                        tagCounts = tagCountsQuery.ToList();
                    }
                    else
                    {
                        if (part.Slug == "/")
                        {
                            part.Slug = "";
                        }

                        var containerId = _autorouteRepository.Table
                            .Where(c => c.DisplayAlias == part.Slug)
                            .Select(x => x.Id)
                            .ToList() // don't try to optimize with slicing  as there should be only one result
                            .FirstOrDefault();

                        if (containerId == 0)
                        {
                            return new List<TagCount>();
                        }

                        var tagCountsQuery = _contentManager
                                          .Query<TagsPart, TagsPartRecord>(VersionOptions.Published)
                                          .Join<MediaPartRecord>()
                                          .Join<CommonPartRecord>()
                                          .Where(t => t.Container.Id == containerId)
                                          .List()
                                          .SelectMany(t => t.CurrentTags)
                                          .GroupBy(t => t)
                                          .Select(g => new TagCount
                                          {
                                              TagName = g.Key,
                                              Count = g.Count()
                                          });

                        tagCounts = tagCountsQuery.ToList();

                        if (!tagCounts.Any())
                        {
                            return new List<TagCount>();
                        }
                    }

                    // initialize centroids with a linear distribution
                    var centroids = new int[buckets];
                    var maxCount = tagCounts.Any() ? tagCounts.Max(tc => tc.Count) : 0;
                    var minCount = tagCounts.Any() ? tagCounts.Min(tc => tc.Count) : 0;
                    var maxDistance = maxCount - minCount;
                    for (int i = 0; i < centroids.Length; i++)
                    {
                        centroids[i] = maxDistance / buckets * (i + 1);
                    }

                    var balanced = false;
                    var loops = 0;

                    // loop until equilibrium or instability
                    while (!balanced && loops++ < 50)
                    {
                        balanced = true;
                        // assign to closest buckets
                        foreach (var tagCount in tagCounts)
                        {
                            // look for closest bucket
                            var currentDistance = Math.Abs(tagCount.Count - centroids[tagCount.Bucket - 1]);
                            for (int i = 0; i < buckets; i++)
                            {
                                var distance = Math.Abs(tagCount.Count - centroids[i]);
                                if (distance < currentDistance)
                                {
                                    tagCount.Bucket = i + 1;
                                    currentDistance = distance;
                                    balanced = false;
                                }
                            }
                        }

                        // recalculate centroids
                        for (int i = 0; i < buckets; i++)
                        {
                            var target = tagCounts.Where(x => x.Bucket == i + 1).ToArray();
                            if (target.Any())
                            {
                                centroids[i] = (int)target.Average(x => x.Count);
                            }
                        }
                    }

                    if (part.SortByPopularity)
                        tagCounts = tagCounts.OrderByDescending(tc => tc.Count);
                    else
                        tagCounts = tagCounts.OrderBy(tc => tc.TagName);


                    return tagCounts.ToList();
                });
        }

    }

}