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

namespace Extending2
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Extending2.Components;

	using Castle.Core;
	using Castle.Core.Configuration;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for TransactionFacility.
	/// </summary>
	public class TransactionFacility : IFacility
	{
		TransactionConfigHolder _transactionConfigHolder;

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			kernel.AddComponent( "transactionmanager", 
				typeof(ITransactionManager), typeof(DefaultTransactionManager) );
			kernel.AddComponent( "transaction.interceptor", 
				typeof(TransactionInterceptor) );
			kernel.AddComponent( "transaction.configholder", typeof(TransactionConfigHolder) );

			_transactionConfigHolder = 
				kernel[ typeof(TransactionConfigHolder) ] as TransactionConfigHolder;

			kernel.ComponentModelCreated += new ComponentModelDelegate(OnModelCreated);
		}

		public void Terminate()
		{
		}

		private void OnModelCreated(ComponentModel model)
		{
			if (IsTransactional(model))
			{
				TransactionConfig config = CreateTransactionConfig(model);

				_transactionConfigHolder.Register(model.Implementation, config);

				model.Interceptors.Add( 
					new InterceptorReference(typeof(TransactionInterceptor)) );
			}
		}

		private TransactionConfig CreateTransactionConfig(ComponentModel model)
		{
			TransactionConfig config = new TransactionConfig();
			GatherTransactionAttributes(config, model.Implementation);
			GatherTransactionConfiguration(config, model);
			return config;
		}

		private bool IsTransactional(ComponentModel model)
		{
			if (model.Configuration != null)
			{
				String attrValue = model.Configuration.Attributes["transactional"];
				
				if ("true".Equals(attrValue))
				{
					return true;
				}
			}

			if ( model.Implementation.IsDefined( typeof(TransactionalAttribute), true ) )
			{
				return true;
			}

			return false;
		}

		private void GatherTransactionConfiguration(TransactionConfig config, ComponentModel model)
		{
			if (model.Configuration == null) return;
			
			IConfiguration transactionNode = model.Configuration.Children["transaction"];

			if (transactionNode == null) return;

			foreach(IConfiguration methodNode in transactionNode.Children)
			{
				config.AddMethodName( methodNode.Value );
			}
		}

		private void GatherTransactionAttributes(TransactionConfig config, Type implementation)
		{
			MethodInfo[] methods = implementation.GetMethods( 
				BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic );

			foreach(MethodInfo method in methods)
			{
				if (method.IsDefined( typeof(RequiresTransactionAttribute), true ))
				{
					config.AddMethod( method );
				}
			}
		}
	}


	public class TransactionConfig
	{
		private IList _methods = new ArrayList();
		private IList _methodName = new ArrayList();

		public void AddMethodName(string value)
		{
			_methodName.Add(value);
		}

		public void AddMethod(MethodInfo method)
		{
			_methods.Add(method);
		}
	
		/// <summary>
		/// A 
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public bool IsMethodTransactional(MethodInfo method)
		{
			if (_methods.Contains(method)) return true;

			foreach(String methodName in _methodName)
			{
				if (method.Name.Equals(methodName))
				{
					return true;
				}
			}

			return false;
		}
	}
}
