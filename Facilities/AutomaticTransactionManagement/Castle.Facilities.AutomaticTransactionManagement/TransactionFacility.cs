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
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Augments the kernel to handle transactional components
	/// </summary>
	public class TransactionFacility : AbstractFacility
	{
		/// <summary>
		/// Registers the interceptor component, the metainfo store and
		/// adds a contributor to the ModelBuilder
		/// </summary>
		protected override void Init()
		{
			Kernel.AddComponent("transaction.interceptor", typeof(TransactionInterceptor));
			Kernel.AddComponent("transaction.MetaInfoStore", typeof(TransactionMetaInfoStore));
			Kernel.ComponentModelBuilder.AddContributor(new TransactionComponentInspector());
		}
	}
}