using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;

namespace Cascade.PhotoSwipe.Handlers
{
    public class PhoneShapeProvider : IShapeTableProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public PhoneShapeProvider(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        // Add specific shape alternates if a device is detected with 'phone' in the UserAgent string
        // If you need different shape names, try using the theme placement.ini to map the shape names below to the alternates you want
        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("PhotoSwipe")
                .OnDisplaying(displaying =>
                {
                    var context = _workContextAccessor.GetContext();
                    var useragentLower = context.HttpContext.Request.UserAgent.ToLower();
                    if (useragentLower.Contains("phone"))
                    {
                        displaying.ShapeMetadata.Alternates
                            .Add("PhotoSwipe__Phone");
                    }
                });
            builder.Describe("IsotopePhotoSwipe")
               .OnDisplaying(displaying =>
               {
                   var context = _workContextAccessor.GetContext();
                   var useragentLower = context.HttpContext.Request.UserAgent.ToLower();
                   if (useragentLower.Contains("phone"))
                   {
                       displaying.ShapeMetadata.Alternates
                           .Add("PhotoSwipe__Phone");
                   }
               });
            builder.Describe("PhotoSwipeTag")
              .OnDisplaying(displaying =>
              {
                  var context = _workContextAccessor.GetContext();
                  var useragentLower = context.HttpContext.Request.UserAgent.ToLower();
                  if (useragentLower.Contains("phone"))
                  {
                      displaying.ShapeMetadata.Alternates
                          .Add("PhotoSwipeTag__Phone");
                  }
              });
        }
    }
}