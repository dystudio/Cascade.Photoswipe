using Cascade.PhotoSwipe.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Mvc;

namespace Cascade.PhotoSwipe.Drivers {
    public class MediaTagCloudDriver : ContentPartDriver<MediaTagCloudPart> {

        protected override string Prefix {
            get {
                return "MediaTagCloud";
            }
        }
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MediaTagCloudDriver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override DriverResult Display(MediaTagCloudPart part, string displayType, dynamic shapeHelper) {

            // TagName follows controller name: eg show/covers
            string tagName = null;
            var httpContext = _httpContextAccessor.Current();
            if(httpContext != null)
            {
                var components = httpContext.Request.Path.Split('/');

                for(int i = 0; i < components.Length; ++i)
                {
                    if (string.Compare(components[i], "show", true) == 0 && i+1 < components.Length)
                        tagName = components[i + 1];
                }
            }
            return ContentShape("Parts_MediaTagCloud",
                () => shapeHelper.Parts_MediaTagCloud(
                    TagCounts: part.TagCounts,
                    ContentPart: part,
                    ContentItem: part.ContentItem,
                    TagName: tagName
                    ));
        }

        protected override DriverResult Editor(MediaTagCloudPart part, dynamic shapeHelper) {

            return ContentShape("Parts_MediaTagCloud_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/MediaTagCloud",
                    Model: part,
                    Prefix: Prefix));
        }
        
        protected override DriverResult Editor(MediaTagCloudPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}