using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Web;
using Castle.Igloo.LifestyleManager;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace Igloo.Clinic.Web 
{
	/// <summary>
	/// Description résumée de Global.
	/// </summary>
    public class Global : HttpApplication, IContainerAccessor
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private IContainer components = null;

        private static IWindsorContainer _container = null;

        #region IContainerAccessor Members

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
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			XmlInterpreter interpreter = new XmlInterpreter(new ConfigResource());
			interpreter.ProcessResource(interpreter.Source, store);

            _container = new WindsorContainer(interpreter);
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
            //Exception ex = Server.GetLastError().GetBaseException();

		}

		protected void Session_End(Object sender, EventArgs e)
		{
            ScopeLifestyleModule.ReleaseSessionScopeComponent(this, e);
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
			this.components = new Container();
		}
		#endregion
	}
}

