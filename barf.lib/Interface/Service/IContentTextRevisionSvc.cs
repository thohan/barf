using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface IContentTextRevisionSvc
	{
		void Load(ContentTextRevision revision);

		List<ContentTextRevision> GetRevisions(int id);
	}
}
