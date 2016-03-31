using System.Threading;
using System.Web;
using System.Web.Mvc;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib
{
	public static class Extensions
	{
		/// <summary>
		/// Returns content to display.
		/// NOTE: This does not create edit-mode hooks, so you will have no way of editing this content in edit mode. You can only edit this in the admin's content list
		/// </summary>
		public static IHtmlString DcmsContent(this HtmlHelper helper, string contentPhrase, ContentType contentType = ContentType.text, string role = "DEFAULT")
		{
			var stubSvc = DependencyResolver.Current.GetService<IContentSvc>();
			var languageSvc = DependencyResolver.Current.GetService<ILanguageSvc>();
			var stub = new ContentStub { ContentPhrase = contentPhrase, ContentType = contentType };
			var currentLanguage = languageSvc.GetLanguage(Thread.CurrentThread.CurrentUICulture.ToString());

			switch (stub.ContentType)
			{
				case ContentType.binary:
					var binary = (ContentBinary)stubSvc.GetItem(stub, currentLanguage.ID);
					return new HtmlString(binary.ContentBinaryBlob.ToString());
				case ContentType.text:
					var text = (ContentText)stubSvc.GetItem(stub, currentLanguage.ID);
					// This prepends a convenient comment for where DcmsContent is getting used.
					return new HtmlString("<!-- dcms-id = " + text.ContentPhrase + " -->\n" + text.ContentTextBlob + "\n<!-- dcms-id-end = " + text.ContentPhrase + " -->\n");
				default:
					return new HtmlString("");
			}
		}
	}
}
