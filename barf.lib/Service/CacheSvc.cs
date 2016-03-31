using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Model;

namespace barf.lib.Service
{
	// I'm pretty sure this is never used. It's supposed to provide some basic caching capabilities.
	public class CacheSvc : ICacheSvc
	{
		private readonly IContentTextDao textDao;
		private readonly IContentBinaryDao binaryDao;

		public CacheSvc(IContentTextDao textDao, IContentBinaryDao binaryDao)
		{
			this.textDao = textDao;
			this.binaryDao = binaryDao;
		}

		public List<ContentText> ContentTextCache
		{
			get
			{
				if (HttpRuntime.Cache["DCMSContentTextCache"] == null)
				{
					HttpRuntime.Cache.Insert("DCMSContentTextCache", textDao.LoadAll(), null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration);
				}

				return (List<ContentText>)HttpRuntime.Cache["DCMSContentTextCache"];
			}
			set
			{
				HttpRuntime.Cache["DCMSContentTextCache"] = value;
			}
		}

		public List<ContentBinary> ContentBinaryCache
		{
			get
			{
				if (HttpRuntime.Cache["DCMSContentBinaryCache"] == null)
				{
					HttpRuntime.Cache.Insert("DCMSContentBinaryCache", binaryDao.LoadAll(), null, DateTime.Now.AddDays(1), Cache.NoSlidingExpiration);
				}

				return (List<ContentBinary>)HttpRuntime.Cache["DCMSContentBinaryCache"];
			}
			set
			{
				HttpRuntime.Cache["DCMSContentBinaryCache"] = value;
			}
		}
	}
}
