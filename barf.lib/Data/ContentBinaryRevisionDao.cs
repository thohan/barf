using System;
using System.Data;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class ContentBinaryRevisionDao : IContentBinaryRevisionDao
	{
		private readonly IDataHelper dataHelper;

		public ContentBinaryRevisionDao(IDataHelper dataHelper)
		{
			this.dataHelper = dataHelper;
		}

		public void Insert(ContentBinary binary, string notes)
		{
			var command = dataHelper.SetCommand("csp_contentbinaryrevision_insert");
			dataHelper.AddInputParameter("ItemID", binary.ItemID, command);
			dataHelper.AddInputParameter("Blob", binary.ContentBinaryBlob, command);
			dataHelper.AddInputParameter("FileName", binary.FileName ?? string.Empty, command);
			dataHelper.AddInputParameter("MimeType", binary.MimeType ?? string.Empty, command);
			dataHelper.AddInputParameter("Notes", notes ?? string.Empty, command);
			dataHelper.AddInputParameter("Editor", binary.Editor, command);
			dataHelper.AddInputParameter("RoleID", binary.RoleID, command);
			dataHelper.AddInputParameter("GoLiveDateTime", binary.GoLiveDateTime, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
		}

		public void Load(ContentBinaryRevision revision)
		{
			var command = dataHelper.SetCommand("");
			dataHelper.AddInputParameter("RevisionID", revision.ID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				SetAllProperties(revision, reader);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		private static void SetAllProperties(ContentBinaryRevision archive, IDataRecord reader)
		{
			archive.ContentBinaryID = Util.TryIntParse(reader["StubID"]);
			DateTime time;
			DateTime.TryParse(reader["DateCreated"].ToString(), out time);
			archive.TimeStamp = time;
			archive.FileName = reader["FileName"].ToString();
			archive.MimeType = reader["MimeType"].ToString();
			archive.Notes = reader["Notes"].ToString();
		}
	}
}
