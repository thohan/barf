using System.Collections.Generic;

namespace barf.lib.Model
{
	public class ContentStub
	{
		public int StubID { get; set; }
		public string ContentPhrase { get; set; }

		/// <summary>
		/// The idea behind this is that several groups could share the same CMS db, and their content would be separated via a RoleID.
		/// However, this would require a schema change: The combo key would be content phrase, language, and role, instead of just content phrase and language.
		/// For now, unimplemented.
		/// </summary>
		public int? RoleID { get; set; }

		/// <summary>
		/// Represents either text, binary, or unknown.
		/// </summary>
		public ContentType ContentType { get; set; }

		// Do I even need these two properties?
		public bool IsBinary { get { return ContentType == ContentType.binary; } }
		public bool IsText { get { return ContentType == ContentType.text; } }

		/// <summary>
		/// If the GoLiveDateTime is mindate, or any datetime in the past, IsLive = true, else IsLive = false
		/// TODO: Remove from code and schema (or implement a way to make content in a holding pen go live at a given time via a trigger or some such thing. That'd be cool...)
		/// </summary>
		public bool IsLive { get; set; }
	}

	/// <summary>
	/// This may contain instances of ContentText and ContentBinary, both of which inherit from ContentStub
	/// </summary>
	public class ContentStubList : List<ContentStub>
	{
		
	}
}
