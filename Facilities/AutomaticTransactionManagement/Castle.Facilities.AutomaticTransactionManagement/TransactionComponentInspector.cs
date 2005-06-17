// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	using Castle.Model;

	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel;

	using Castle.Services.Transaction;

	/// <summary>
	/// Summary description for TransactionComponentInspector.
	/// </summary>
	public class TransactionComponentInspector : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Implementation.IsDefined( typeof(TransactionalAttribute), true ))
			{
				model.Dependencies.Add( 
					new DependencyModel( DependencyType.Service, null, typeof(TransactionInterceptor), false ) );

				model.Interceptors.Insert( 0,  
					new InterceptorReference( typeof(TransactionInterceptor) ) );
			}
		}
	}
}
