using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Cascade.PhotoSwipe {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Show/{tagName}",
                        new RouteValueDictionary {
                            {"area", "Cascade.PhotoSwipe"},
                            {"controller", "Show"},
                            {"action", "Search"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.PhotoSwipe"}
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Show",
                        new RouteValueDictionary {
                            {"area", "Cascade.PhotoSwipe"},
                            {"controller", "Show"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Cascade.PhotoSwipe"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}