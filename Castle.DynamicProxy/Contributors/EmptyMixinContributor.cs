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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Diagnostics;
	using Generators.Emitters;

	class EmptyMixinContributor : MixinContributorBase
	{
		public override void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			
		}

		public EmptyMixinContributor(Type @interface)
		{
			if (@interface == null) throw new ArgumentNullException("interface");
			// TODO: this method is likely to be moved to the interface
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding mapping only...");
			mixinInterface = @interface;
		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			field = BuildTargetField(@class, mixinInterface);
		}
	}
}