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
using System.Configuration;
using System.Reflection;

using Castle.Core.Configuration;
using Castle.Igloo.UI;
using Castle.MicroKernel.Facilities;
using Castle.Igloo.UIComponents;
using Castle.Igloo.Contexts;
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
			
            // Added a UIComponent Repository to track it
            Kernel.AddComponent("component.repository", typeof(UIComponentRepository));

            // Added the navigation interceptor
            Kernel.AddComponent("navigation.interceptor", typeof(NavigationInterceptor));

            Kernel.ComponentModelBuilder.AddContributor(new ControllerInspector());
            Kernel.ComponentModelBuilder.AddContributor(new ScopeInspector());
            Kernel.ComponentModelBuilder.AddContributor(new BijectionInspector());

            RegisterSubResolver();
            
            RegisterViewComponent();            
		}

		#endregion

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

        private void RegisterSubResolver()
        {
            Kernel.Resolver.AddSubResolver(new ContextsResolver(Kernel));
            Kernel.Resolver.AddSubResolver(new ContextResolver(Kernel));
        }

	}
}
