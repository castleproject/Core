#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using Castle.Core.Configuration;
using Castle.Igloo.Contexts.Windows;
using Castle.Igloo.Mock;
using Castle.Igloo.Scopes;
using Castle.Igloo.Scopes.Web;
using Castle.Igloo.Scopes.Windows;
using Castle.Igloo.UI;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.Igloo.UIComponents;
using Castle.Igloo.Controllers;
using Castle.Igloo.Interceptors;

namespace Castle.Igloo
{
	/// <summary>
	/// Facility responsible for initialize the MVC framework.
	/// (Registering the controllers used by the views...)
	/// </summary>
	public class IglooFacility : AbstractFacility
	{
		private Assembly _assembly = null;
        private IList<IHandler> _waitListHandlers = new List<IHandler>();

		#region IFacility Members


        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationErrorsException(
					"The IglooFacility requires an 'assembyView' child tag.");
			}

			IConfiguration factoriesConfig = FacilityConfig.Children["assembyView"];
			if (factoriesConfig!=null && 
				factoriesConfig.Value!= null && factoriesConfig.Value!= string.Empty )
			{
				_assembly =  Assembly.Load(factoriesConfig.Value) ;	
			}

            RegisterContributor();
            Kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);

            // Added a Scope Registry to track scope instance
            Kernel.AddComponent("scope.repository", typeof(IScopeRegistry),typeof(ScopeRegistry));
			
            // Added a UIComponent Repository to track it
            Kernel.AddComponent("component.repository", typeof(UIComponentRepository));
            
            RegisterInternaleScopes();
            
            // Added interceptors
            Kernel.AddComponent("navigation.interceptor", typeof(NavigationInterceptor));
            Kernel.AddComponent("bijection.interceptor", typeof(BijectionInterceptor));

            RegisterViewComponent();            
		}

		#endregion

        private void OnComponentRegistered(string key, IHandler handler)
        {
            bool isScope = typeof(IScope).IsAssignableFrom(handler.ComponentModel.Service);

            if (isScope)
            {
                if (handler.CurrentState == HandlerState.WaitingDependency)
                {
                    _waitListHandlers.Add(handler);
                }
                else
                {
                    RegisterScope(handler.ComponentModel.Name);
                }
            }

            CheckWaitingList();
        }

        /// For each new component registered,
        /// some components in the WaitingDependency
        /// state may have became valid, so we check them
        private void CheckWaitingList()
        {
            IList<IHandler> handlersToRemove = new List<IHandler>();

            foreach (IHandler handler in _waitListHandlers)
            {
                if (handler.CurrentState == HandlerState.Valid)
                {
                    RegisterScope(handler.ComponentModel.Name);
                    handlersToRemove.Add(handler);
                }
            }
            foreach (IHandler handler in handlersToRemove)
            {
               _waitListHandlers.Remove(handler);
            }
        }

        private void RegisterScope(string scopeName)
        {
            IScopeRegistry scopeRegistry = (IScopeRegistry)Kernel[typeof(IScopeRegistry)];
            IScope scope = (IScope)Kernel[scopeName];
            scopeRegistry.RegisterScope(scopeName, scope);
        }

	    private void RegisterInternaleScopes()
        {
            // For unit test
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                // Web scope
                Kernel.AddComponent(ScopeType.Application, typeof (IApplicationScope), typeof (WebApplicationScope));
                Kernel.AddComponent(ScopeType.Page, typeof(IPageScope), typeof(WebPageScope));
                Kernel.AddComponent(ScopeType.Request, typeof(IRequestScope), typeof(WebRequestScope));
                Kernel.AddComponent(ScopeType.Session, typeof(ISessionScope), typeof(WebSessionScope));
            }
            else
            {
                // Mock web scope
                Kernel.AddComponent(ScopeType.Application, typeof(IApplicationScope), typeof(MockApplicationScope));
                Kernel.AddComponent(ScopeType.Page, typeof(IPageScope), typeof(MockPageScope));
                Kernel.AddComponent(ScopeType.Request, typeof(IRequestScope), typeof(MockRequestScope));
                Kernel.AddComponent(ScopeType.Session, typeof(ISessionScope), typeof(MockSessionScope));

                // Windows scope
                Kernel.AddComponent(ScopeType.Thread, typeof(IScope), typeof(ThreadScope));
                Kernel.AddComponent(ScopeType.Singleton, typeof(IScope), typeof(SingletonScope));
                Kernel.AddComponent(ScopeType.Transient, typeof(IScope), typeof(TransientScope));
            }
        }

        private void RegisterContributor()
        {
            Kernel.ComponentModelBuilder.AddContributor(new ControllerInspector());
            Kernel.ComponentModelBuilder.AddContributor(new ScopeInspector());
            Kernel.ComponentModelBuilder.AddContributor(new BijectionInspector());
            //Kernel.ComponentModelBuilder.AddContributor(new ScopeRegisterInspector());
        }
        
	    private void RegisterViewComponent()
		{
            UIComponentRepository repository = (UIComponentRepository)Kernel[typeof(UIComponentRepository)];

			if (_assembly != null)
			{
				Type[] types = _assembly.GetExportedTypes();
				
				foreach( Type type in types )
				{
                    // MasterPage / Web page / UserControl
                    if (typeof(IView).IsAssignableFrom(type))
                    {
                        UIComponent uiComponent = new UIComponent(type, Kernel);
                        repository.AddComponent(uiComponent);	
					}				
				}				
			}
		}

	}
}
