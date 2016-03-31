using System.Collections.Generic;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib.Service
{
	public class ContentTextRevisionSvc : IContentTextRevisionSvc
	{
		private readonly IContentTextRevisionDao revDao;

		public ContentTextRevisionSvc(IContentTextRevisionDao revDao)
		{
			this.revDao = revDao;
		}

		/// <summary>
		/// This method presumes the StubID has been set. With that, we will query to find the most recent revision
		/// </summary>
		public void Load(ContentTextRevision revision)
		{
			revDao.Load(revision);
		}

		/// <summary>
		/// Gets all of the archives for a given content piece
		/// </summary>
		/// <returns></returns>
		public List<ContentTextRevision> GetRevisions(int id)
		{
			return revDao.LoadAll(id);
		}
	}
}
