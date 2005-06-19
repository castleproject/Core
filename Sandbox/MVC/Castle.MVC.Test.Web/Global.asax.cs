using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

using Castle.Facilities.TypedFactory;
using Castle.MVC.Navigation;
using Castle.MVC.Test.Presentation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Views;
using Castle.Windsor;

namespace Castle.MVC.Test.Web 
{
	/// <summary>
	/// Description résumée de Global.
	/// </summary>
	public class Global : System.Web.HttpApplication, IContainerAccessor
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private static WindsorContainer _container;

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return _container; }
		}

		#endregion

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			_container = new WindsorContainer();
			TypedFactoryFacility facility = new TypedFactoryFacility();

			_container.AddFacility("typedfactory", facility );
			facility.AddTypedFactoryEntry( 
				new FactoryEntry("stateFactory", typeof(IStateFactory), "Create", "Release") );

			AddControllers();
		}
 
		private void AddControllers()
		{
			_container.AddComponent( "state", typeof(IState),typeof(MyApplicationState));
			_container.AddComponent( "statePersister", typeof(IStatePersister), typeof(SessionPersister));
			_container.AddComponent( "viewManager", typeof(IViewManager), typeof(XmlWebViewManager));
			_container.AddComponent( "navigator", typeof(INavigator), typeof(DefaultNavigator));
			_container.AddComponent( "HomeController", typeof(HomeController) );
			_container.AddComponent( "AccountController", typeof(AccountController) );

		}

		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{
			_container.Dispose();
		}
			
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

