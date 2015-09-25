using Cascade.PhotoSwipe.Services;
using Cascade.PhotoSwipe.ViewModels;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.Tags.Services;
using Orchard.Tags.ViewModels;
using Orchard.Themes;
using Orchard.UI.Navigation;
using System.Linq;
using System.Web.Mvc;

// A controller is used so we can get at the route parameter, which is the tagName
// This code is based on the corresponding controller in the Orchard.Tags module.

namespace Cascade.PhotoSwipe.Controllers
{
    [ValidateInput(false), Themed]
    public class ShowController : Controller
    {
        private readonly IPhotoSwipeService _photoSwipeService;
        private readonly ITagService _tagService;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;

        public ShowController(
            IPhotoSwipeService photoSwipeService,
            ITagService tagService,
            IContentManager contentManager,
            ISiteService siteService,
            IShapeFactory shapeFactory)
        {
            _photoSwipeService = photoSwipeService;
            _tagService = tagService;
            _contentManager = contentManager;
            _siteService = siteService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        public dynamic Shape { get; set; }

        public ActionResult Index()
        {
            var tags = _tagService.GetTags();
            var model = new TagsViewModel { List = tags.ToList() };
            return View(model);
        }

        public ActionResult Search(string tagName, PagerParameters pagerParameters)
        {
            Pager pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);

            var tag = _tagService.GetTagByName(tagName);

            if (tag == null)
            {
                return RedirectToAction("Index");
            }

            var taggedItems = _photoSwipeService.GetTaggedContentItems(tag.Id, pager.GetStartIndex(), pager.PageSize).ToList();
            var tagShapes = taggedItems.Select(item => _contentManager.BuildDisplay(item, "Summary"));

            var list = Shape.List();
            list.AddRange(tagShapes);

            var totalItemCount = _photoSwipeService.GetTaggedContentItemCount(tag.Id);
            var viewModel = new TagsViewModel
            {
                TagName = tag.TagName,
                List = list,
                Pager = Shape.Pager(pager).TotalItemCount(totalItemCount)
            };


            return View(viewModel);

        }

    }
}
