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

namespace AspectSharp
{
	using System;
	using AspectSharp.Core;
	using AspectSharp.Core.Matchers;
	using AspectSharp.Core.Proxy;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// The AspectEngine is responsible for matching 
	/// specified types agaisnt an AspectDefinition. 
	/// If a match is found, a proxy is requested and the AOP
	/// features will be implemented in the Proxy.
	/// </summary>
	public class AspectEngine
	{
		private EngineConfiguration _config;
		private IAspectMatcher _aspectMatcher;
		private IProxyFactory _proxyFactory;

		/// <summary>
		/// Constructs an AspectEngine
		/// </summary>
		/// <param name="config">From where to gather the 
		/// configuration</param>
		public AspectEngine(EngineConfiguration config)
		{
			AssertUtil.ArgumentNotNull(config, "config");
			_config = config;
			_aspectMatcher = new DefaultAspectMatcher();
			_proxyFactory = new DefaultProxyFactory(this);
		}

		/// <summary>
		/// The configuration applied to this AspectEngine instance
		/// </summary>
		public EngineConfiguration Configuration
		{
			get { return _config; }
		}

		/// <summary>
		/// The <see cref="IProxyFactory"/> implementation
		/// responsible for generating the proxies.
		/// </summary>
		public IProxyFactory ProxyFactory
		{
			get { return _proxyFactory; }
		}

		/// <summary>
		/// The <see cref="IAspectMatcher"/> implementation
		/// responsible for matching the types against the 
		/// AspectDefinitions in the configuration.
		/// </summary>
		public IAspectMatcher AspectMatcher
		{
			get { return _aspectMatcher; }
		}

		/// <summary>
		/// Wraps an object. The object specified is no longer used
		/// hence this method is marked as obsolete.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <remarks>
		/// We cannot stress enough that the instance passed must not be used
		/// to access the object contents.
		/// </remarks>
		[Obsolete("Use the WrapClass(Type) instead", false)]
		public virtual object Wrap(object instance)
		{
			AssertUtil.ArgumentNotNull(instance, "instance");
			return WrapClass(instance.GetType());
		}

		/// <summary>
		/// Wraps an interface. The target argument must be an
		/// object capable of responding to the interface messages, or
		/// your interceptors must be capable of respoding to them. 
		/// </summary>
		/// <param name="classType">Concrete class with a available constructor (public or protected) to be wrapped</param>
		/// <returns>A proxy</returns>
		public virtual object WrapClass(Type classType)
		{
			AssertUtil.ArgumentNotNull(classType, "classType");
			AssertUtil.ArgumentNotInterface(classType, "classType");

			AspectDefinition[] aspects = AspectMatcher.Match(classType, Configuration.Aspects);

			if (aspects.Length == 0)
			{
				return Activator.CreateInstance(classType);
			}

			AspectDefinition aspectdef = Union(aspects);

			return ProxyFactory.CreateClassProxy(classType, aspectdef);
		}

		/// <summary>
		/// Wraps an interface. The target argument must be an
		/// object capable of responding to the interface messages, or
		/// your interceptors must be capable of respoding to them. 
		/// </summary>
		/// <param name="inter">Interface to be wrapped</param>
		/// <param name="target">The object that responds to the interface messages</param>
		/// <returns>A proxy</returns>
		public virtual object WrapInterface(Type inter, object target)
		{
			AssertUtil.ArgumentNotNull(target, "target");
			AssertUtil.ArgumentIsInterface(inter, "interface");

			AspectDefinition[] aspects = AspectMatcher.Match(inter, Configuration.Aspects);

			AspectDefinition aspectdef = Union(aspects);

			return ProxyFactory.CreateInterfaceProxy(inter, target, aspectdef);
		}

		/// <summary>
		/// Creates a single AspectDefinition as the merge of two or more
		/// AspectDefinitions
		/// </summary>
		/// <param name="aspects">The aspects to be merged</param>
		/// <returns>The result of the merge</returns>
		protected virtual AspectDefinition Union(AspectDefinition[] aspects)
		{
			if (aspects.Length == 1)
			{
				return aspects[0];
			}

			AspectDefinition aspect = new AspectDefinition(LexicalInfo.Empty, String.Empty);

			foreach (AspectDefinition item in aspects)
			{
				aspect.Name += item.Name + "$";
				aspect.Mixins.AddRange(item.Mixins);
				aspect.PointCuts.AddRange(item.PointCuts);
			}

			return aspect;
		}
	}
}