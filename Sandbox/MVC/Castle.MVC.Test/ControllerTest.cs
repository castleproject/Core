using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;
using Castle.Windsor;
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

			_container = new WindsorContainer();

			_container.AddFacility("MVCFacility", new MVCFacility() );
			_container.AddComponent( "state", typeof(IState),typeof(MyApplicationState));
			_container.AddComponent( "viewManager", typeof(IViewManager), typeof(MockViewManager));

			AddControllers();
		}
 
		private void AddControllers()
		{
			_container.AddComponent( "HomeController", typeof(HomeController) );
			_container.AddComponent( "AccountController", typeof(AccountController) );

			_homeController = _container.Resolve("HomeController") as HomeController;
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
