#region Apache Notice
/*****************************************************************************
 * 
 * Castle.MVC
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
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MVC.Controllers;
using Castle.MVC.LifestyleManager;
using Castle.MVC.States;
using Castle.MVC.Views;

namespace Castle.MVC
{
	/// <summary>
	/// Facility responsible for initialize the MVC framework.
	/// (Registering the controllers used by the views...)
	/// </summary>
	public class MVCFacility : AbstractFacility
	{
		/// <summary>
		/// Binding token
		/// </summary>
		private BindingFlags BINDING_FLAGS_SET
			= BindingFlags.Public 
			| BindingFlags.SetProperty
			| BindingFlags.Instance 
			| BindingFlags.SetField
			;

		private Assembly _assembly = null;

		/// <summary>
		/// Cosntructor
		/// </summary>
		public MVCFacility()
		{
		}


		#region IFacility Members

		/// <summary>
		/// 
		/// </summary>
		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException(
					"The MVCFacility requires an 'assembyView' child tag.");
			}

			IConfiguration factoriesConfig = FacilityConfig.Children["assembyView"];
			if (factoriesConfig!=null && 
				factoriesConfig.Value!= null && factoriesConfig.Value!= string.Empty )
			{
				_assembly =  Assembly.Load(factoriesConfig.Value) ;	
			}

			// Added TypedFactory to have a IState factory
			TypedFactoryFacility facility = new TypedFactoryFacility();

			Kernel.AddFacility("typedfactory", facility );
			facility.AddTypedFactoryEntry( new FactoryEntry("stateFactory", typeof(IStateFactory), "Create", "Release") );
			
			// Added a ControlerTree component to track controller by view
			Kernel.AddComponent( "mvc.controllerTree", typeof(ControllerTree) );
			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);

			Initialize();
		}

		#endregion

		private void Initialize()
		{
			ControllerTree tree = (ControllerTree) Kernel["mvc.controllerTree"];

			if (_assembly != null)
			{
				Type[] types = _assembly.GetExportedTypes();
				
				foreach( Type type in types )
				{
					// Web page / UserControl
					if ( type.IsSubclassOf(typeof(WebFormView)) || type.IsSubclassOf(typeof(WebUserControlView))  )
					{
						// Retrieve the properties controllers
						PropertyInfo[] properties = type.GetProperties(BINDING_FLAGS_SET);
						for (int i = 0; i < properties.Length; i++) 
						{
							if (properties[i].PropertyType.IsSubclassOf(typeof(Controller)))
							{
								tree.AddController(type, properties[i], properties[i].PropertyType);
							}
						}	
					}				
				}				
			}
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			if ( !typeof(Controller).IsAssignableFrom(model.Implementation) )
			{
				return;
			}

			// Ensure its CustomLifestyle
			model.LifestyleType = LifestyleType.Custom;
			model.CustomLifestyle = typeof(PerRequestLifestyleManager);
		}

	}
}
