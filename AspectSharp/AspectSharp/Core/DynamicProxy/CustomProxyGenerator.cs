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

namespace AspectSharp.Core
{
	using System;
	using Castle.DynamicProxy;

	/// <summary>
	/// Generates a dynamic proxy. This overrides the .Net proxy requirements 
	/// that forces one to extend MarshalByRefObject or (for a different purpose)
	/// ContextBoundObject to have a Proxiable class.
	/// </summary>
	/// <example>
	/// <code>
	/// MyInvocationinterceptor interceptor = ...
	/// IInterfaceExposed proxy = 
	///		ProxyGenerator.CreateProxy( new Type[] { typeof(IInterfaceExposed) }, interceptor );
	/// </code>
	/// </example>
	public class CustomProxyGenerator : ProxyGenerator
	{
		public CustomProxyGenerator()
		{
		}

		/// <summary>
		/// Generates a proxy implementing the specified interface and the mixins
		/// redirecting method invocations to the specifed interceptor.
		/// </summary>
		/// <param name="inter">Interface to be implemented.</param>
		/// <param name="mixins">Array of instances (mixins) to be introducted.</param>
		/// <param name="interceptor">Instance of <see cref="IInterceptor"/>.</param>
		/// <returns>Proxy Instance.</returns>
		public object CreateProxy(Type inter, object target, object[] mixins, IInterceptor interceptor)
		{
			GeneratorContext context = CreateGeneratorContext(mixins);
			return base.CreateCustomProxy(inter, interceptor, target, context);
		}

		/// <summary>
		/// Generates a class which extends the baseClass, overriding all 
		/// the virtual methods and implementing all the mixin interfaces.
		/// </summary>
		/// <param name="baseClass">Super class</param>
		/// <param name="mixins">Array of mixins to be implemented by the proxy</param>
		/// <param name="interceptor">Instance of <see cref="IInterceptor"/></param>
		/// <returns>Proxy instance</returns>
		public object CreateClassProxy(Type baseClass, object[] mixins, IInterceptor interceptor,
			params object[] constructorArgs)
		{
			GeneratorContext context = CreateGeneratorContext(mixins);
			return base.CreateCustomClassProxy(baseClass, interceptor, context, constructorArgs);
		}

		/// <summary>
		/// Creates a context - which is used to guid custom proxy
		/// generation.
		/// </summary>
		/// <param name="mixins">Array of mixins to be registered</param>
		/// <returns>A GeneratorContext instance</returns>
		protected GeneratorContext CreateGeneratorContext(object[] mixins)
		{
			GeneratorContext context = new GeneratorContext();
			foreach (object mixin in mixins)
			{
				context.AddMixinInstance(mixin);
			}
			return context;
		}
	}
}