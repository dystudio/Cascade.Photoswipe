using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Cascade.PhotoSwipe {
    public class PhotoSwipeDataMigration : DataMigrationImpl {

        public int Create() {
          

            ContentDefinitionManager.AlterTypeDefinition(
                "MediaTagCloud",
                cfg => cfg
                           .WithPart("MediaTagCloudPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }
    }
}