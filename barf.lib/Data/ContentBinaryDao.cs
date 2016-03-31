using System;
using System.Collections.Generic;
using System.Data;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class ContentBinaryDao : IContentBinaryDao
	{
		private readonly IContentBinaryRevisionDao revDao;
		private readonly IDataHelper dataHelper;

		public ContentBinaryDao(IContentBinaryRevisionDao revDao, IDataHelper dataHelper)
		{
			this.revDao = revDao;
			this.dataHelper = dataHelper;
		}

		// Core method
		public void LoadContentBinary(ContentBinary contentBinary, bool getLatest)
		{
			var command = dataHelper.SetCommand("csp_contentbinary_select");
			dataHelper.AddInputParameter("Phrase", contentBinary.ContentPhrase, command);
			dataHelper.AddInputParameter("LangID", contentBinary.Language.ID, command);
			dataHelper.AddInputParameter("RoleID", contentBinary.RoleID, command);
			dataHelper.AddInputParameter("GetLatest", getLatest ? 1 : 0, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				contentBinary.ContentType = Util.GetContentType(reader["TypeID"]);
				contentBinary.StubID = Util.TryIntParse(reader["StubID"]);
				contentBinary.RoleID = Util.TryIntNullableParse(reader["RoleID"]);
				contentBinary.ItemID = Util.TryIntParse(reader["ItemID"]);

				DateTime datetime;
				DateTime.TryParse(reader["GoLiveDateTime"].ToString(), out datetime);
				contentBinary.GoLiveDate = datetime.ToString("MM/dd/yyyy") == "01/01/1900" ? DateTime.Now.ToString("MM/dd/yyyy") : datetime.ToString("MM/dd/yyyy");
				contentBinary.GoLiveTime = datetime.ToString("HH:mm");
				contentBinary.IsLive = datetime <= DateTime.Now;
				contentBinary.ContentID = Util.TryIntParse(reader["ContentID"]);
				contentBinary.ContentBinaryBlob = reader["Blob"] as byte[];
				contentBinary.FileName = reader["FileName"].ToString();
				contentBinary.MimeType = reader["MimeType"].ToString();
			}
			else
			{
				InsertContentStubAndBinary(contentBinary);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		// Admin method
		public void LoadContentBinaryMeta(ContentBinary content)
		{
			var command = dataHelper.SetCommand("csp_contentbinary_select");
			dataHelper.AddInputParameter("Phrase", content.ContentPhrase, command);
			dataHelper.AddInputParameter("LangID", content.Language.ID, command);
			dataHelper.AddInputParameter("RoleID", content.RoleID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				content.ContentType = Util.GetContentType(reader["TypeID"]);
				content.StubID = Util.TryIntParse(reader["StubID"]);
				//content.RoleID = Util.TryIntParse(reader["RoleID"]);
				content.ItemID = Util.TryIntParse(reader["ItemID"]);
				//DateTime datetime;
				//DateTime.TryParse(reader["GoLiveDateTime"].ToString(), out datetime);
				//content.GoLiveDate = datetime.ToString("MM/dd/yyyy") == "01/01/1900" ? DateTime.Now.ToString("MM/dd/yyyy") : datetime.ToString("MM/dd/yyyy");
				//content.GoLiveTime = datetime.ToString("HH:mm");
				//content.IsLive = datetime <= DateTime.Now;
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		// Admin method
		public int GetSingleBinaryContent(string phrase, int? languageID)
		{
			var result = 0;

			var command = dataHelper.SetCommand("csp_contentbinary_select_content_only");
			dataHelper.AddInputParameter("LangID", languageID, command);
			dataHelper.AddInputParameter("Phrase", phrase, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				result = Util.TryIntParse(reader["ContentID"]);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return result;
		}

		// Core method (used by core method LoadContentBinary)
		public void InsertContentStubAndBinary(ContentBinary content)
		{
			var command = dataHelper.SetCommand("csp_contentbinary_insert");
			dataHelper.AddInputParameter("Phrase", content.ContentPhrase, command);
			dataHelper.AddInputParameter("StubID", content.StubID, command);
			dataHelper.AddInputParameter("ItemID", content.ItemID, command);
			dataHelper.AddInputParameter("RoleID", content.RoleID, command);
			dataHelper.AddInputParameter("TypeID", (int)content.ContentType, command);
			dataHelper.AddInputParameter("Blob", content.ContentBinaryBlob, command);
			dataHelper.AddInputParameter("FileName", content.FileName ?? string.Empty, command);
			dataHelper.AddInputParameter("MimeType", content.MimeType ?? string.Empty, command);
			dataHelper.AddInputParameter("LangID", content.Language.ID, command);
			dataHelper.AddInputParameter("GoLiveDateTime", content.GoLiveDateTime, command);
			dataHelper.AddInputParameter("Editor", content.Editor, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				content.StubID = Util.TryIntParse(reader["StubID"]);
				content.ItemID = Util.TryIntParse(reader["ItemID"]);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);

			if (content.ContentBinaryBlob != null)
			{
				if (content.ItemID > 0)
				{
					revDao.Insert(content, string.Empty);
				}
				else
				{
					throw new Exception("ItemID not provided");
				}
			}
		}

		/// <summary>
		/// This was unimplemented for some reason. Well, it's here now.
		/// </summary>
		/// <notes>Admin method</notes>
		public List<ContentBinary> GetContentItems(int stubID)
		{
			var results = new List<ContentBinary>();
			var command = dataHelper.SetCommand("csp_contentbinary_select_by_stubid");	// I need to get the ContentID, using the stubID and presumably the language and RoleID
			dataHelper.AddInputParameter("StubID", stubID, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var binary = new ContentBinary();
				SetProperties(binary, reader);
				results.Add(binary);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return results;
		}

		// Sort of a Core method, but it's only used by the cacheSvc, and the cacheSvc is currently completely unused. So not really a core method until it does get used, which will probably be never.
		public List<ContentBinary> LoadAll()
		{
			var results = new List<ContentBinary>();
			var command = dataHelper.SetCommand("csp_contentbinary_select_all");
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var binary = new ContentBinary();
				SetProperties(binary, reader);
				results.Add(binary);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return results;
		}

		// Admin method
		public void UpdateContentStubAndBinary(ContentBinary content, string notes)
		{
			var command = dataHelper.SetCommand("csp_contentbinary_update");
			dataHelper.AddInputParameter("Phrase", content.ContentPhrase, command);
			dataHelper.AddInputParameter("RoleID", content.RoleID, command);
			dataHelper.AddInputParameter("TypeID", (int)content.ContentType, command);
			dataHelper.AddInputParameter("Blob", content.ContentBinaryBlob, command);
			dataHelper.AddInputParameter("FileName", content.FileName ?? string.Empty, command);
			dataHelper.AddInputParameter("MimeType", content.MimeType ?? string.Empty, command);
			dataHelper.AddInputParameter("LanguageID", content.Language.ID, command);
			dataHelper.AddInputParameter("GoLiveDateTime", content.GoLiveDateTime, command);
			dataHelper.AddInputParameter("Editor", content.Editor, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
			revDao.Insert(content, notes);
		}

		// Obsolete, apparently
		//public ContentBinary LoadByStubIDAndLanguage(int stubID, int languageID)
		//{
		//	ContentBinary binary = new ContentBinary();
		//	SqlCommand command = dataHelper.SetCommand("csp_contentbinary_select_by_stubid_and_lang");
		//	dataHelper.AddInputParameter("StubID", stubID, command);
		//	dataHelper.AddInputParameter("LangID", languageID, command);
		//	IDataReader reader = dataHelper.ExecuteReader(command);

		//	if (reader.Read())
		//	{
		//		binary.ContentPhrase = reader["Phrase"].ToString();
		//		binary.RoleID = Util.TryIntNullableParse(reader["RoleID"]);
		//		binary.ContentType = Util.GetContentType(reader["TypeID"]);
		//		binary.StubID = stubID;
		//		binary.ItemID = Util.TryIntParse(reader["ItemID"]);
		//		int langID = Util.TryIntParse(reader["LangID"]);
		//		binary.Language = langDao.GetLanguage(langID);
		//		binary.FileName = reader["FileName"].ToString();
		//		binary.MimeType = reader["MimeType"].ToString();
		//		binary.ContentBinaryBlob = (byte[])reader["blob"];
		//	}

		//	dataHelper.Close(reader);
		//	dataHelper.Close(command);
		//	return binary;
		//}

		// Core method (used by GetFile special-route thingy)	// The "itemID" in question is ContentBinaryRole.ID, the item that holds content! This will somehow be known at the time of link creation.
		public ContentBinary LoadByContentID(int contentID)
		{
			var binary = new ContentBinary();
			var command = dataHelper.SetCommand("csp_contentbinary_select_by_contentid");
			dataHelper.AddInputParameter("ContentID", contentID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				binary.ContentPhrase = reader["Phrase"].ToString();
				binary.RoleID = Util.TryIntNullableParse(reader["RoleID"]);
				binary.ContentType = Util.GetContentType(reader["TypeID"]);
				binary.StubID = Util.TryIntParse(reader["StubID"]);
				binary.ItemID = Util.TryIntParse(reader["ItemID"]);
				binary.ContentID = Util.TryIntParse(reader["ContentID"]);
				var langID = Util.TryIntParse(reader["LangID"]);
				binary.Language = new Language(langID);
				binary.FileName = reader["FileName"].ToString();
				binary.MimeType = reader["MimeType"].ToString();
				binary.ContentBinaryBlob = (byte[])reader["blob"];
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return binary;
		}

		private void SetProperties(ContentBinary content, IDataRecord reader)
		{
			content.ContentPhrase = reader["Phrase"].ToString();
			content.StubID = Util.TryIntParse(reader["StubID"]);	// Not sure if this is necessary.
			content.ContentType = Util.GetContentType(reader["TypeID"]);
			content.ItemID = Util.TryIntParse(reader["ItemID"]);
			content.FileName = reader["FileName"].ToString();
			content.MimeType = reader["MimeType"].ToString();
			content.ContentID = Util.TryIntParse(reader["ContentID"]);
			var langID = Util.TryIntParse(reader["LangID"]);
			content.Language = new Language(langID);
			content.RoleID = Util.TryIntParse(reader["RoleID"]);
			content.ContentBinaryBlob = reader["blob"] == DBNull.Value ? null : (byte[])reader["blob"];
		}
	}
}
