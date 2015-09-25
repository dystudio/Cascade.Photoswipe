using Orchard.UI.Resources;

namespace Cascade.PhotoSwipe
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // scripts
            manifest.DefineScript("PhotoSwipe")
                .SetVersion("4.0")
                .SetUrl("PhotoSwipe.min.js", "photoswipe.js");

            manifest.DefineScript("PhotoSwipe.UI")
               .SetVersion("4.0")
               .SetDependencies("PhotoSwipe")
               .SetUrl("PhotoSwipe-ui-cascade.min.js", "PhotoSwipe-ui-cascade.js");

            manifest.DefineScript("InitializePhotoSwipe")
               .SetDependencies("PhotoSwipe")
               .SetUrl("InitializePhotoSwipe.min.js", "InitializePhotoSwipe.js");

            // styles
            manifest.DefineStyle("PhotoSwipe")
                .SetVersion("4.0")
                .SetUrl("photoswipe.css");

            manifest.DefineStyle("PhotoSwipe.DefaultSkin")
               .SetDependencies("PhotoSwipe", "FontAwesome")
               .SetVersion("4.0")
               .SetUrl("cascade-skin.css");

            manifest.DefineStyle("FontAwesome")
               .SetVersion("4.2.0")
               .SetCdn("//cdnjs.cloudflare.com/ajax/libs/font-awesome/4.2.0/css/font-awesome.min.css");


        }
    }
}
