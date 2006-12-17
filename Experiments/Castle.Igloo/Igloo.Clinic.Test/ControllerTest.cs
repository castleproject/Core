using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Igloo.Clinic.Domain;
using NUnit.Framework;

using Igloo.Clinic.Application;

using Castle.Igloo.Controllers;
using Castle.Igloo;
using Castle.Igloo.Contexts;
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
        private IContexts _contexts = null;
        private NavigationContext _navigationContext = null;
        private Messages _messages = null;

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
            _contexts = _container.Resolve<IContexts>();
            _navigationContext = _contexts.RequestContext[NavigationContext.NAVIGATION_CONTEXT] as NavigationContext;
            _messages = _contexts.RequestContext[Messages.REQUEST_MESSAGES] as Messages;
        }

        /// <summary>
        /// TearDown
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            //_contexts.ConversationContext.Flush();
            _contexts.PageContext.Flush();
            _messages.Clear();
            _contexts.SessionContext.Flush();
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
            Assert.IsNotNull(_navigationContext);

            _navigationContext.CurrentView = "login";
            _navigationContext.Action = "login";

            controller.Validate("no", "xxxx");

            Assert.AreEqual("login", _navigationContext.PreviousView);
            Assert.AreEqual("login", _navigationContext.CurrentView);
            Assert.AreEqual("index", _navigationContext.NextView);
        }

        /// <summary>
        /// Test session context
        /// </summary>
        [Test]
        public void TestSessionContext_Login()
        {
            LoginController controller = _container.Resolve<LoginController>();

            Assert.IsNotNull(controller);

            _navigationContext.CurrentView = "login";
            _navigationContext.Action = "login";

            Assert.IsTrue( controller.Validate("no", "xxxx") );

            Assert.IsTrue(_contexts.SessionContext.Contains("doctor"));
            object doctor = _contexts.SessionContext["doctor"];
            Assert.IsNotNull(doctor);

            Assert.IsInstanceOfType(typeof(Doctor), doctor);
            Doctor doc = (Doctor)doctor;
            Assert.AreEqual("no", doc.Name);
        }

        /// <summary>
        /// Test session context
        /// </summary>
        [Test]
        public void TestSessionContext_NoLogin()
        {
            LoginController controller = _container.Resolve<LoginController>();

            Assert.IsNotNull(controller);

            _navigationContext.CurrentView = "login";
            _navigationContext.Action = "login";

            Assert.IsFalse(controller.Validate("Ali Baba and the Forty Thieves", "xxxx"));

            Assert.IsFalse(_contexts.SessionContext.Contains("doctor"));
            Assert.IsTrue(_messages.ContainsKey("unknown"));

            Assert.AreEqual("login", _navigationContext.PreviousView);
            Assert.AreEqual("login", _navigationContext.CurrentView);
            Assert.AreEqual("login", _navigationContext.NextView);

        }

		#endregion
	}
}
