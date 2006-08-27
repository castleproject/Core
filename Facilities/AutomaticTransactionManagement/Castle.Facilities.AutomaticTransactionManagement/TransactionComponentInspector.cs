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
	/// Summary description for TransactionComponentInspector.
	/// </summary>
	public class TransactionComponentInspector : MethodMetaInspector
	{
		private static readonly String TransactionNodeName = "transaction";
		private TransactionMetaInfoStore metaStore;

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
		}

		private void ConfigureBasedOnAttributes(ComponentModel model)
		{
			if (model.Implementation.IsDefined(typeof(TransactionalAttribute), true))
			{
				metaStore.CreateMetaFromType(model.Implementation);

				model.Dependencies.Add( 
					new DependencyModel(DependencyType.Service, null, typeof(TransactionInterceptor), false) );

				model.Interceptors.AddFirst( new InterceptorReference(typeof(TransactionInterceptor)) );
			}
		}

		protected override String ObtainNodeName()
		{
			return TransactionNodeName;
		}

		protected override void ProcessMeta(ComponentModel model, MethodInfo[] methods, MethodMetaModel metaModel)
		{
			metaStore.CreateMetaFromConfig(model.Implementation, methods, metaModel.ConfigNode);
		}

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

		private bool IsMarkedWithTransactional(IConfiguration configuration)
		{
			return (configuration != null && "true" == configuration.Attributes["isTransactional"]);
		}

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
	}
}
