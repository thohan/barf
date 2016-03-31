using System;

namespace barf.lib.Model
{
	[Serializable]
	public class Language
	{
		public int ID { get; set; }
		public string LanguageCode { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
		public bool IsCurrent { get; set; }

		public Language()
		{

		}

		public Language(int langID)
		{
			ID = langID;
		}
	}
}
