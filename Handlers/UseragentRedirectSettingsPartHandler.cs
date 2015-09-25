using Cascade.PhotoSwipe.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Cascade.PhotoSwipe.Handlers
{
    public class UseragentRedirectSettingsPartHandler:ContentHandler
    {
        public UseragentRedirectSettingsPartHandler()
        {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<UseragentRedirectSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForPart<UseragentRedirectSettingsPart>("UseragentRedirectSettingsPart", "Parts/UseragentRedirectSettings", "UseragentRedirect"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("UseragentRedirect")));
        }
    }
}