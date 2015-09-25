using Cascade.PhotoSwipe.Models;
using Orchard.ContentManagement;
using Orchard.Mvc.Filters;
using Orchard.Settings;
using System.Web;
using System.Web.Mvc;

namespace Cascade.PhotoSwipe.Filters
{
    public class UseragentFilter : FilterProvider, IResultFilter, IActionFilter
    {
        private readonly ISiteService _siteSettings;

        public UseragentFilter(ISiteService siteSettings)
        {
            _siteSettings = siteSettings;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) { }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var urSettings = _siteSettings.GetSiteSettings().As<UseragentRedirectSettingsPart>();
            if (urSettings == null || string.IsNullOrWhiteSpace(urSettings.UseragentKeyword) || string.IsNullOrWhiteSpace(urSettings.Path))
                return;

            var url = filterContext.RequestContext.HttpContext.Request.Url.ToString().ToLower();
            var useragent = filterContext.RequestContext.HttpContext.Request.UserAgent.ToLower();

            if (useragent.Contains(urSettings.UseragentKeyword) && (url.EndsWith(urSettings.Path) || url.Contains(urSettings.Path + "/")))
            {
                string redirectUrl = url.Substring(0, url.IndexOf(urSettings.Path));
                redirectUrl += urSettings.ReplacementPath;
                
                filterContext.Result = new RedirectResult(redirectUrl, false); // temporary
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) { }
        public void OnResultExecuted(ResultExecutedContext filterContext) { }
    }
}

