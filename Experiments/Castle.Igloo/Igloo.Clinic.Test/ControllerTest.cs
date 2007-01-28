using Castle.Igloo.Mock;
using Castle.Igloo.Scopes.Web;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Igloo.Clinic.Domain;
using NUnit.Framework;

using Igloo.Clinic.Application;

using Castle.Igloo.Controllers;
using Castle.Igloo;
using Castle.Igloo.Navigation;

namespace Igloo.Clinic.Test
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture] 
	public class ControllerTest
	{
		private WindsorContainer _container;
	    
        private IRequestScope _requestScope = null;
        private ISessionScope _sessionScope = null;
        private IPageScope _pageScope = null;
    
		#region SetUp & TearDown

		/// <summary>
		/// SetUp
		/// </summary>
		[TestFixtureSetUp]
        public void FixtureSetUp() 
		{
			 _container = null;

			DefaultConfigurationStore store = new DefaultConfigurationStore();
            XmlInterpreter interpreter = new XmlInterpreter(new ConfigResource());
			interpreter.ProcessResource(interpreter.Source, store);

			_container = new WindsorContainer( interpreter );
		}
        
		/// <summary>
		/// TearDown
		/// </summary>
		[TestFixtureTearDown]
        public void FixtureTearDown()
		{ 
			_container.Dispose();
		}

        /// <summary>
        /// SetUp
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _sessionScope = _container.Resolve<ISessionScope>();
            _pageScope = _container.Resolve<IPageScope>();
            _requestScope = _container.Resolve<IRequestScope>();
        }

        /// <summary>
        /// TearDown
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _requestScope.Flush();
            _pageScope.Flush();
            _sessionScope.Flush();
        }

		#endregion

		#region Test Controller

		/// <summary>
		/// Test Container
		/// </summary>
		[Test] 
		public void TestContainer() 
		{
            IController controller = _container.Resolve<IController>("Login.Controller");
            Assert.IsNotNull(controller);

            controller = _container.Resolve<IController>("Patient.Controller");
            Assert.IsNotNull(controller);
		}

        /// <summary>
        /// Test loading Embeded Resource
        /// </summary>
        [Test]
        public void TestNavigation()
        {
            LoginController controller = _container.Resolve<LoginController>();

            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.NavigationState);

            controller.NavigationState.CurrentView = "login";
            controller.NavigationState.Action = "login";

            controller.Validate("no", "xxxx");

            Assert.AreEqual("login", controller.NavigationState.PreviousView);
            Assert.AreEqual("login", controller.NavigationState.CurrentView);
            Assert.AreEqual("index", controller.NavigationState.NextView);
        }

        /// <summary>
        /// Test session context
        /// </summary>
        [Test]
        public void TestSessionContext_Login()
        {
            LoginController controller = _container.Resolve<LoginController>();

            Assert.IsNotNull(controller);

            controller.NavigationState.CurrentView = "login";
            controller.NavigationState.Action = "login";

            Assert.IsTrue(controller.Validate("no", "xxxx"));

            Assert.IsTrue(_sessionScope.Contains("doctor"));
            object doctor = _sessionScope["doctor"];
            Assert.IsNotNull(doctor);

            Assert.IsInstanceOfType(typeof(Doctor), doctor);
            Doctor doc = (Doctor)doctor;
            Assert.AreEqual("NO", doc.Name);
        }

        /// <summary>
        /// Test session context
        /// </summary>
        [Test]
        public void TestSessionContext_NoLogin()
        {
            LoginController controller = _container.Resolve<LoginController>();

            Assert.IsNotNull(controller);

            controller.NavigationState.CurrentView = "login";
            controller.NavigationState.Action = "login";

            Assert.IsFalse(controller.Validate("Ali Baba and the Forty Thieves", "xxxx"));

            Assert.IsFalse(_sessionScope.Contains("doctor"));
            
            FlashMessages messages = _requestScope[FlashMessages.FLASH_MESSAGES] as FlashMessages;
            
            Assert.IsTrue(messages.ContainsKey("unknown"));

            Assert.AreEqual("login", controller.NavigationState.PreviousView);
            Assert.AreEqual("login", controller.NavigationState.CurrentView);
            Assert.AreEqual("login", controller.NavigationState.NextView);

        }

		#endregion
	}
}
