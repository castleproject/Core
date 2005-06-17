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

using AspectSharp.Core;
using AspectSharp.Lang.AST;

namespace Castle.Facilities.AspectSharp
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel;
	
	using Castle.Windsor.Proxy;
	
	using Castle.DynamicProxy;

	/// <summary>
	/// Specialization of <see cref="Castle.Windsor.Proxy.DefaultProxyFactory"/>
	/// that checks for aspects in the model and potential mixins.
	/// </summary>
	public class AopProxyFactory : DefaultProxyFactory
	{
		protected override void CustomizeContext(GeneratorContext context, IKernel kernel, 
			ComponentModel model, object[] arguments)
		{
			AspectDefinition aspect = (AspectDefinition) model.ExtendedProperties["aop.aspect"];

			if (aspect == null) return;

			MixinDefinitionCollection mixins = aspect.Mixins;

			foreach(MixinDefinition definition in mixins)
			{
				Type mixinType = definition.TypeReference.ResolvedType;
				
				try
				{
					context.AddMixinInstance( Activator.CreateInstance( mixinType ) );
				}
				catch(Exception e)
				{
					throw new ApplicationException("Could not instantiate mixin " + mixinType.FullName, e);
				}
			}
		}

		protected override void CustomizeProxy(object proxy, GeneratorContext context, IKernel kernel, ComponentModel model)
		{
			object[] mixins = context.MixinsAsArray();

			for(int i=0; i < mixins.Length; i++)
			{
				object mixin = mixins[i];
				
				if (mixin is IProxyAware)
				{
					(mixin as IProxyAware).SetProxy(proxy);
				}
			}
		}
	}
}
