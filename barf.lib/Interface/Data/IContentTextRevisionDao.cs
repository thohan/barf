using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Data
{
	public interface IContentTextRevisionDao
	{
		void Load(ContentTextRevision revision);

		void Insert(ContentText text, string notes);

		List<ContentTextRevision> LoadAll(int itemID);
	}
}
