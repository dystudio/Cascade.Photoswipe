using Cascade.PhotoSwipe.Models;
using Cascade.PhotoSwipe.Services;
using Orchard.Caching;
using Orchard.ContentManagement.Utilities;
using Orchard.Tags.Models;
using System.Linq;

namespace Cascade.PhotoSwipe.Handlers
{
    public class PhotoSwipeHandler : ContentHandler {
        private readonly ISignals _signals;

        public PhotoSwipeHandler(
            IPhotoSwipeService MediaTagCloudService,
            ISignals signals) {

            _signals = signals;

            OnInitializing<MediaTagCloudPart>((context, part) => part._tagCountField.Loader(tags =>
                    MediaTagCloudService.GetPopularTags(part.Buckets, part).ToList()
                    ));

            OnUpdated<MediaTagCloudPart>((context, part) => InvalidateMediaTagCloudCache());

            OnPublished<TagsPart>((context, part) => InvalidateMediaTagCloudCache());
            OnRemoved<TagsPart>((context, part) => InvalidateMediaTagCloudCache());
            OnUnpublished<TagsPart>((context, part) => InvalidateMediaTagCloudCache());
        }

        public void InvalidateMediaTagCloudCache() {
            _signals.Trigger(PhotoSwipeService.FogTagsChanged);
        }
    }
}