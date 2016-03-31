using System;
using System.Web;
using System.Web.Mvc;

namespace barf.lib.Model
{
	// see http://stackoverflow.com/questions/10011780/prevent-caching-in-asp-net-mvc-for-specific-actions-using-an-attribute
	// I'm not sure if I'm using this...
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class DcmsNoCacheAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
			filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
			filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			filterContext.HttpContext.Response.Cache.SetNoStore();

			base.OnResultExecuting(filterContext);
		}
	}
}
