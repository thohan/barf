using System;
using System.Collections.Generic;
using System.Data;
using barf.lib.Interface.Data;
using barf.lib.Model;

namespace barf.lib.Data
{
	public class ContentTextRevisionDao : IContentTextRevisionDao
	{
		private readonly IDataHelper dataHelper;

		public ContentTextRevisionDao(IDataHelper dataHelper)
		{
			this.dataHelper = dataHelper;
		}

		// Admin method
		public void Load(ContentTextRevision revision)
		{
			var command = dataHelper.SetCommand("csp_contenttextrevision_select");
			dataHelper.AddInputParameter("RevisionID", revision.ID, command);
			var reader = dataHelper.ExecuteReader(command);

			if (reader.Read())
			{
				SetAllProperties(revision, reader);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
		}

		public void Insert(ContentText text, string notes)
		{
			var command = dataHelper.SetCommand("csp_contenttextrevision_insert");
			dataHelper.AddInputParameter("ItemID", text.ItemID, command);
			dataHelper.AddInputParameter("Blob", text.ContentTextBlob, command);
			dataHelper.AddInputParameter("Notes", notes ?? string.Empty, command);
			dataHelper.AddInputParameter("Editor", text.Editor, command);
			dataHelper.AddInputParameter("GoLiveDateTime", text.GoLiveDateTime, command);
			dataHelper.AddInputParameter("RoleID", text.RoleID, command);
			dataHelper.ExecuteNonQuery(command);
			dataHelper.Close(command);
		}

		// Admin method
		public List<ContentTextRevision> LoadAll(int itemID)
		{
			var archives = new List<ContentTextRevision>();
			var command = dataHelper.SetCommand("csp_contenttextrevision_select_all");
			dataHelper.AddInputParameter("ItemID", itemID, command);
			var reader = dataHelper.ExecuteReader(command);

			while (reader.Read())
			{
				var archive = new ContentTextRevision();
				SetProperties(archive, reader);
				archives.Add(archive);
			}

			dataHelper.Close(reader);
			dataHelper.Close(command);
			return archives;
		}

		private static void SetProperties(ContentTextRevision revision, IDataRecord reader)
		{
			revision.ID = Util.TryIntParse(reader["RevID"]);
			DateTime datecreated;
			DateTime.TryParse(reader["DateCreated"].ToString(), out datecreated);
			revision.DateCreated = datecreated.ToString("dd/MM/yyyy");
			revision.Notes = reader["Notes"].ToString();
			revision.RoleID = Util.TryIntParse(reader["RoleID"]);
			revision.ContentBlob = reader["Blob"].ToString();
		}

		private static void SetAllProperties(ContentTextRevision revision, IDataRecord reader)
		{
			
			DateTime datecreated;
			DateTime.TryParse(reader["DateCreated"].ToString(), out datecreated);
			revision.DateCreated = datecreated.ToString("dd/MM/yyyy");
			revision.Notes = reader["Notes"].ToString();
			revision.RoleID = Util.TryIntParse(reader["RoleID"]);
			revision.ContentBlob = reader["Blob"].ToString();
			
		}
	}
}
