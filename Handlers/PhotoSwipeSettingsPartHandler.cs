using Cascade.PhotoSwipe.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Cascade.PhotoSwipe.Handlers
{
    public class PhotoSwipeSettingsPartHandler:ContentHandler
    {
        public PhotoSwipeSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<PhotoSwipeSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForPart<PhotoSwipeSettingsPart>("PhotoSwipeSettingsPart", "Parts/PhotoSwipeSettings", "PhotoSwipe"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("PhotoSwipe")));
        }
    }
}