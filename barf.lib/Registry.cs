using Autofac;
using barf.lib.Data;
using barf.lib.Interface.Data;
using barf.lib.Interface.Service;
using barf.lib.Service;

namespace barf.lib
{
	public class Registry
	{
		public static void RegisterCore(ContainerBuilder builder)
		{
			builder.RegisterType<ContentBinaryDao>().As<IContentBinaryDao>();
			builder.RegisterType<ContentBinaryRevisionDao>().As<IContentBinaryRevisionDao>();
			builder.RegisterType<ContentStubDao>().As<IContentStubDao>();
			builder.RegisterType<ContentTextDao>().As<IContentTextDao>();
			builder.RegisterType<ContentTextRevisionDao>().As<IContentTextRevisionDao>();
			builder.RegisterType<DataHelper>().As<IDataHelper>();
			builder.RegisterType<LanguageDao>().As<ILanguageDao>();
			builder.RegisterType<UtilityDao>().As<IUtilityDao>();

			builder.RegisterType<CacheSvc>().As<ICacheSvc>();
			builder.RegisterType<ContentBinaryRevisionSvc>().As<IContentBinaryRevisionSvc>();
			builder.RegisterType<ContentSvc>().As<IContentSvc>();
			builder.RegisterType<ContentTextRevisionSvc>().As<IContentTextRevisionSvc>();
			builder.RegisterType<LanguageSvc>().As<ILanguageSvc>();
			builder.RegisterType<SessionSvc>().As<ISessionSvc>();
			builder.RegisterType<UtilSvc>().As<IUtilSvc>();
		}
	}
}
