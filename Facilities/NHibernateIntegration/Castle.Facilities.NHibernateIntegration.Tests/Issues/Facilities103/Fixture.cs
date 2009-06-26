using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Castle.MicroKernel;
using Castle.Services.Transaction;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;
using ITransaction = Castle.Services.Transaction.ITransaction;
namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities103
{
	using SessionStores;

	[TestFixture]
	public class DefaultSessionManagerTestCase : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "EmptyConfiguration.xml";
			}
		}
		public override void OnSetUp()
		{
			sessionStore = new CallContextSessionStore();
			kernel = mockRepository.DynamicMock<IKernel>();
			factoryResolver = mockRepository.DynamicMock<ISessionFactoryResolver>();
			transactionManager = mockRepository.DynamicMock<ITransactionManager>();
			transaction = mockRepository.DynamicMock<ITransaction>();
			sessionFactory = mockRepository.DynamicMock<ISessionFactory>();
			session = mockRepository.DynamicMock<ISession>();
			contextDictionary = new Hashtable();
			sessionManager = new DefaultSessionManager(sessionStore, kernel, factoryResolver);
		}
		
		private const string Alias = "myAlias";
		private const string InterceptorFormatString = DefaultSessionManager.InterceptorFormatString;
		private const string InterceptorName = DefaultSessionManager.InterceptorName;
		private const IsolationMode DefaultIsolationMode = IsolationMode.ReadUncommitted;
		private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadUncommitted;

		#region mock variables
		private ISessionStore sessionStore;
		private IKernel kernel;
		private ISessionFactoryResolver factoryResolver;
		private ITransactionManager transactionManager;
		private ITransaction transaction;
		private ISessionFactory sessionFactory;
		private ISession session;
		private IDictionary contextDictionary;
		private ISessionManager sessionManager;
		#endregion

		[Test]
		public void WhenBeginTransactionFailsSessionIsRemovedFromSessionStore()
		{
			using (mockRepository.Record())
			{
				Expect.Call(kernel[typeof(ITransactionManager)]).Return(transactionManager);
				Expect.Call(transactionManager.CurrentTransaction).Return(transaction);
				Expect.Call(factoryResolver.GetSessionFactory(Alias)).Return(sessionFactory);
				Expect.Call(kernel.HasComponent(string.Format(InterceptorFormatString, Alias))).Return(false);
				Expect.Call(kernel.HasComponent(InterceptorName)).Return(false);
				Expect.Call(sessionFactory.OpenSession()).Return(session);
				session.FlushMode = sessionManager.DefaultFlushMode;
				Expect.Call(transaction.Context).Return(contextDictionary).Repeat.Any();
				Expect.Call(transaction.DistributedTransaction).Return(false);
				Expect.Call(transaction.IsolationMode).Return(DefaultIsolationMode);
				Expect.Call(session.BeginTransaction(DefaultIsolationLevel)).Throw(new Exception());
			}

			using (mockRepository.Playback())
			{
				try
				{
					sessionManager.OpenSession(Alias);
					Assert.Fail("DbException not thrown");
				}
				catch (Exception)
				{

				}
				Assert.IsNull(sessionStore.FindCompatibleSession(Alias),"The sessionStore shouldn't contain compatible session if the session creation fails");
			}
		}
	}
}
