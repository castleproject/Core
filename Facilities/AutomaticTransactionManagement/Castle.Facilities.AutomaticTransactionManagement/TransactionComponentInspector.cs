// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.AutomaticTransactionManagement
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Castle.Core;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder.Inspectors;
	using Castle.MicroKernel.Facilities;
	using Castle.Core.Configuration;
	using Castle.Services.Transaction;

	/// <summary>
	/// Tries to obtain transaction configuration based on 
	/// the component configuration or (if not available) check
	/// for the attributes.
	/// </summary>
	public class TransactionComponentInspector : MethodMetaInspector
	{
		private static readonly String TransactionNodeName = "transaction";
		private TransactionMetaInfoStore metaStore;

		/// <summary>
		/// Tries to obtain transaction configuration based on 
		/// the component configuration or (if not available) check
		/// for the attributes.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="model">The model.</param>
		public override void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (metaStore == null)
			{
				metaStore = (TransactionMetaInfoStore) kernel[ typeof(TransactionMetaInfoStore) ];
			}

			if (IsMarkedWithTransactional(model.Configuration))
			{
				base.ProcessModel(kernel, model);
			}
			else
			{
				AssertThereNoTransactionOnConfig(model);

				ConfigureBasedOnAttributes(model);
			}

			Validate(model, metaStore);
			
			AddTransactionInterceptorIfIsTransactional(model, metaStore);
		}

		/// <summary>
		/// Tries to configure the ComponentModel based on attributes.
		/// </summary>
		/// <param name="model">The model.</param>
		private void ConfigureBasedOnAttributes(ComponentModel model)
		{
			if (model.Implementation.IsDefined(typeof(TransactionalAttribute), true))
			{
				metaStore.CreateMetaFromType(model.Implementation);
			}
		}

		/// <summary>
		/// Obtains the name of the 
		/// node (overrides MethodMetaInspector.ObtainNodeName)
		/// </summary>
		/// <returns>the node name on the configuration</returns>
		protected override String ObtainNodeName()
		{
			return TransactionNodeName;
		}

		/// <summary>
		/// Processes the meta information available on
		/// the component configuration. (overrides MethodMetaInspector.ProcessMeta)
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="methods">The methods.</param>
		/// <param name="metaModel">The meta model.</param>
		protected override void ProcessMeta(ComponentModel model, 
		                                    MethodInfo[] methods, 
		                                    MethodMetaModel metaModel)
		{
			metaStore.CreateMetaFromConfig(model.Implementation, methods, metaModel.ConfigNode);
		}

		/// <summary>
		/// Validates the type is OK to generate a proxy.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="store">The store.</param>
		private void Validate(ComponentModel model, TransactionMetaInfoStore store)
		{
			if (model.Service == null || model.Service.IsInterface) return;

			TransactionMetaInfo meta = store.GetMetaFor(model.Implementation);

			if (meta == null) return;

			ArrayList problematicMethods = new ArrayList();

			foreach(MethodInfo method in meta.Methods)
			{
				if (!method.IsVirtual)
				{
					problematicMethods.Add( method.Name );
				}
			}

			if (problematicMethods.Count != 0)
			{
				String[] methodNames = (String[]) problematicMethods.ToArray( typeof(String) );

				String message = String.Format( "The class {0} wants to use transaction interception, " + 
					"however the methods must be marked as virtual in order to do so. Please correct " + 
					"the following methods: {1}", model.Implementation.FullName, String.Join(", ", methodNames) );

				throw new FacilityException(message);
			}		
		}

		/// <summary>
		/// Determines whether the configuration has <c>istransaction="true"</c> attribute.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <returns>
		/// <c>true</c> if yes; otherwise, <c>false</c>.
		/// </returns>
		private bool IsMarkedWithTransactional(IConfiguration configuration)
		{
			return (configuration != null && "true" == configuration.Attributes["isTransactional"]);
		}

		/// <summary>
		/// Asserts that if there are transaction behavior
		/// configured for methods, the component node has <c>istransaction="true"</c> attribute
		/// </summary>
		/// <param name="model">The model.</param>
		private void AssertThereNoTransactionOnConfig(ComponentModel model)
		{
			IConfiguration configuration = model.Configuration;

			if (configuration != null && configuration.Children[TransactionNodeName] != null)
			{
				String message = String.Format( "The class {0} has configured transaction in a child node but has not " + 
					"specified istransaction=\"true\" on the component node.", model.Implementation.FullName );

				throw new FacilityException(message);
			}
		}
		
		/// <summary>
		/// Associates the transaction interceptor with the ComponentModel.
		/// </summary>
		/// <param name="model">The model.</param>
		private static void AddTransactionInterceptorIfIsTransactional(ComponentModel model, 
		                                                               TransactionMetaInfoStore store)
		{
			TransactionMetaInfo meta = store.GetMetaFor(model.Implementation);

			if (meta == null) return;

			model.Dependencies.Add(
				new DependencyModel(DependencyType.Service, null, typeof(TransactionInterceptor), false));

			model.Interceptors.AddFirst(new InterceptorReference(typeof(TransactionInterceptor)));
		}
	}
}
