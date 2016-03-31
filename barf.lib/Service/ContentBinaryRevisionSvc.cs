using barf.lib.Interface.Data;
using barf.lib.Interface.Service;

namespace barf.lib.Service
{
	public class ContentBinaryRevisionSvc : IContentBinaryRevisionSvc
	{
		private readonly IContentBinaryRevisionDao revDao;

		public ContentBinaryRevisionSvc(IContentBinaryRevisionDao revDao)
		{
			this.revDao = revDao;
		}
	}
}
