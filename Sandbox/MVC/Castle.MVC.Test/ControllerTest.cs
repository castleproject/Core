using System;

using NUnit.Framework;
using Castle.Windsor;
using Castle.MVC.States;
using Castle.MVC.Navigation;
using Castle.MVC.Views;
using Castle.MVC.StatePersister;

using Castle.MVC.Web;
using Castle.MVC.Web.Controllers;
using Castle.Facilities.TypedFactory;

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
		private AccountController _accountController = null;

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
			_accountController = null;

			_container = new WindsorContainer();

			TypedFactoryFacility facility = new TypedFactoryFacility();

			_container.AddFacility("typedfactory", facility );
			facility.AddTypedFactoryEntry( 
				new FactoryEntry("stateFactory", typeof(IStateFactory), "Create", "Release") );


			AddControllers(_container);
		}
 
		private void AddControllers(WindsorContainer container)
		{
			_container.AddComponent( "state", typeof(IState), typeof(MyApplicationState));
			_container.AddComponent( "statePersister", typeof(IStatePersister), typeof(MemoryStatePersister));
			_container.AddComponent( "viewManager", typeof(IViewManager), typeof(MockViewManager));
			_container.AddComponent( "navigator", typeof(INavigator), typeof(DefaultNavigator));

			_container.AddComponent( "HomeController", typeof(HomeController) );
			_container.AddComponent( "AccountController", typeof(AccountController) );

			_homeController = _container.Resolve("HomeController") as HomeController;
			_accountController = _container.Resolve("AccountController") as AccountController;
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
			object controller = _container.Resolve("HomeController");
			Assert.IsTrue(controller.GetType()==typeof(HomeController));
		}

		/// <summary>
		/// Test loading Embeded Resource
		/// </summary>
		[Test] 
		public void TestController() 
		{
			_state.CurrentView = "index";
			_state.Command = "Login";
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
			_state.Command = "Login";
			_state.SomeSessionString = "toto";
			_homeController.Login("email", "pass");

			Assert.IsTrue(_state.SomeSessionString=="toto");
		}
		#endregion
	}
}
