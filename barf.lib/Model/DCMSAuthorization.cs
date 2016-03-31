using System;
using System.Web;
using System.Web.Mvc;
using barf.lib.Interface.Service;

// not sure if this guy belongs in this namespace. Probably not.
namespace barf.lib.Model
{
	/// <summary>
	/// As per http://www.diaryofaninja.com/blog/2011/07/24/writing-your-own-custom-aspnet-mvc-authorize-attributes ,
	/// I am attempting a custom attribute just for authorization of the cms and no other pages on a website
	/// I've set the AttributeTarget to Class. I guess I have to create another one if I want a different target (Method, most likely)
	/// Also referring to http://stackoverflow.com/questions/977071/redirecting-unauthorized-controller-in-asp-net-mvc
	/// as well as http://stackoverflow.com/questions/977071/redirecting-unauthorized-controller-in-asp-net-mvc
	/// I don't claim to understand everything. The key thing seems to be that, if the user is not authenticated, I'm going to redirect to the cms login page.
	/// Since it appears to be based on the AuthorizeCore which returns DCMSSession.IsLoggedOn, that should suffice
	/// </summary>
	// I shouldn't need this at all, really. We aren't enabling a CMS admin page on the website itself.
	// Perhaps there will be some in-line editing capabilities in the future, but not now.
	
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class DCMSAuthorizeAttribute : AuthorizeAttribute
	{
		private readonly ISessionSvc sessionSvc; 

		public DCMSAuthorizeAttribute()
		{
			sessionSvc = DependencyResolver.Current.GetService<ISessionSvc>();
		}

		public virtual string MasterName { get; set; }
		public virtual string ViewName { get; set; }

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			return sessionSvc.IsLoggedOn;
		}

		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}

			if (AuthorizeCore(filterContext.HttpContext))	// Should only happen within the context of the cms!
			{
				SetCachePolicy(filterContext);
			}
			else if (!sessionSvc.IsLoggedOn)
			{
				filterContext.Result = new RedirectResult("/DCMSAdmin/Login");
			}
			else
			{
				ViewDataDictionary viewData = new ViewDataDictionary();
				viewData.Add("Message", "You do not have sufficient privileges for this operation.");
				filterContext.Result = new ViewResult { MasterName = MasterName, ViewName = ViewName, ViewData = viewData };
			}
		}

		protected void SetCachePolicy(AuthorizationContext filterContext)
		{
			// ** IMPORTANT **
			// Since we're performing authorization at the action level, the authorization code runs
			// after the output caching module. In the worst case this could allow an authorized user
			// to cause the page to be cached, then an unauthorized user would later be served the
			// cached page. We work around this by telling proxies not to cache the sensitive page,
			// then we hook our custom authorization code into the caching mechanism so that we have
			// the final say on whether a page should be served from the cache.
			var cachePolicy = filterContext.HttpContext.Response.Cache;
			cachePolicy.SetProxyMaxAge(new TimeSpan(0));
			cachePolicy.AddValidationCallback(CacheValidateHandler, null);
		}

		protected virtual void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
		{
			validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
		}
	}
}
