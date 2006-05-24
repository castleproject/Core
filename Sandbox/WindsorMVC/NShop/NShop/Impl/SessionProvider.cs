using System;
using System.Runtime.Remoting.Messaging;
using NHibernate;
using NShop.Services;
using NHibernate.Cfg;

namespace NShop.Impl
{
	public class SessionProvider : ISessionProvider
	{
		private const string sessionKey = "Session.Key";
		static ISessionFactory sessionFactory;

		private ISessionFactory SessionFactory
		{
			get
			{
				if (sessionFactory == null)
				{
					Configuration cfg = new Configuration();
					cfg.AddAssembly(typeof (Customer).Assembly);
					sessionFactory = cfg.BuildSessionFactory();
				}
				return sessionFactory;
			}
		}

		ISession ISessionProvider.Session
		{
			get
			{
				ISession session = CallContext.GetData(sessionKey) as ISession;
				if(session==null)
				{
					session = SessionFactory.OpenSession();
					CallContext.SetData(sessionKey, session);
				}
				return session;
			}
		}
		
		public static void Clear()
		{
			ISession session = CallContext.GetData(sessionKey) as ISession;
			if (session == null)
				session.Dispose();
		}
	}
}