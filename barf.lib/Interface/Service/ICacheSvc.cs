using System.Collections.Generic;
using barf.lib.Model;

namespace barf.lib.Interface.Service
{
	public interface ICacheSvc
	{
		List<ContentText> ContentTextCache { get; set; }

		List<ContentBinary> ContentBinaryCache { get; set; }
	}
}
