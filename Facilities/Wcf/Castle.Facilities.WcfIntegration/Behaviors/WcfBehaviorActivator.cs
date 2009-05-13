// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Proxy;

	public class WcfBehaviorActivator : DefaultComponentActivator
	{
		public WcfBehaviorActivator(ComponentModel model, IKernel kernel,
			ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
			: base(model, kernel, onCreation, onDestruction)
		{
		}

		protected override object Instantiate(CreationContext context)
		{
			object instance = base.Instantiate(context);

			object behavior = ProxyUtil.GetUnproxiedInstance(instance);
			WcfExtensionScope scope = WcfUtils.GetScope(Model);
			WcfUtils.ExtendBehavior(Kernel, scope, behavior);

			return instance;
		}
	}
}
