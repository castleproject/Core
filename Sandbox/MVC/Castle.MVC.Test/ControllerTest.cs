using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;
using Castle.Windsor;
using Castle.Model.Configuration;

using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Sources;
using NUnit.Framework;

namespace Castle.MVC.Test
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture] 
	public class ControllerTest
	{
		private WindsorContainer _container;
		private HomeController _homeController = null;
		private MyApplicationState _state = null;

		#region SetUp & TearDown

		/// <summary>
		/// SetUp
		/// </summary>
		[SetUp] 
		public void SetUp() 
		{
			 _container = null;
			 _homeController = null;
			 _state = null;

			// Config by file
			_container = new WindsorContainer( new XmlInterpreter( new AppDomainConfigSource("castle")),
				new XmlInterpreter( "test.config" )
				);

			//-- Config by Code
//			_container = new WindsorContainer(new DefaultConfigurationStore());
//
//			MutableConfiguration confignode = new MutableConfiguration("facility");
//
//			IConfiguration assembyView = confignode.Children.Add(new MutableConfiguration("assembyView"));
//
//			_container.Kernel.ConfigurationStore.AddFacilityConfiguration("MVCFacility", confignode);
//			_container.AddFacility("MVCFacility", new MVCFacility());
//
//			_container.AddComponent( "state", typeof(IState),typeof(MyApplicationState));
//			_container.AddComponent( "navigator", typeof(INavigator), typeof(DefaultNavigator));
//			_container.AddComponent( "viewManager", typeof(IViewManager), typeof(MockViewManager));
//			_container.AddComponent( "statePersister", typeof(IStatePersister), typeof(MemoryStatePersister));
//
//			// controllers
//			_container.AddComponent( "HomeController", typeof(HomeController) );
//			_container.AddComponent( "AccountController", typeof(AccountController) );
//
//			// components
//			_container.AddComponent( "ServiceA", typeof(IServiceA), typeof(ServiceA));


			_homeController = _container["HomeController"] as HomeController;
			_state = _homeController.State as MyApplicationState;
		}
 

		/// <summary>
		/// TearDown
		/// </summary>
		[TearDown] 
		public void Dispose()
		{ 
			_container.Dispose();
		} 

		#endregion

		#region Test Controller

		/// <summary>
		/// Test Container
		/// </summary>
		[Test] 
		public void TestContainer() 
		{
			object controller = _container["HomeController"];
			Assert.IsTrue(controller.GetType()==typeof(HomeController));
			controller = _container["AccountController"];
			Assert.IsTrue(controller.GetType()==typeof(AccountController));

			IServiceA serviceA = _container[typeof(IServiceA)] as IServiceA;
			serviceA.MyMethodNotcached("Gilles");

		}

		/// <summary>
		/// Test loading Embeded Resource
		/// </summary>
		[Test] 
		public void TestController() 
		{
			_state.CurrentView = "index";
			_state.Command = "GoToPage2";
			_homeController.Login("email", "pass");

			Assert.IsTrue(_state.PreviousView=="index");
			Assert.IsTrue(_state.CurrentView=="page2");
		}

		/// <summary>
		/// Test custom state
		/// </summary>
		[Test] 
		public void TestState() 
		{
			_state.CurrentView = "index";
			_state.Command = "GoToPage2";
			_state.SomeSessionString = "toto";
			_homeController.Login("email", "pass");

			Assert.IsTrue(_state.SomeSessionString=="toto");
		}

		#endregion
	}
}
