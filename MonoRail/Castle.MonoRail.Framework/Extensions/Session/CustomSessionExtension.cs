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

namespace Castle.MonoRail.Framework.Extensions.Session
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Configuration;

	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Configuration;


	/// <summary>
	/// This extension allow one to provide a custom 
	/// implementation of the session available on <see cref="IRailsEngineContext"/>
	/// </summary>
	/// <remarks>
	/// To successfully install this extension you must add the attribute <c>customSession</c>
	/// to the <c>monoRail</c> configuration node and register the extension on the extensions node.
	/// <code>
	///   &lt;monoRail customSession="Type name that implements ICustomSessionFactory"&gt;
	///   	&lt;extensions&gt;
	///   	  &lt;extension type="Castle.MonoRail.Framework.Extensions.Session.CustomSessionExtension, Castle.MonoRail.Framework" /&gt;
	///   	&lt;/extensions&gt;
	///   &lt;/monoRail&gt;
	/// </code>
	/// </remarks>
	public class CustomSessionExtension : AbstractMonoRailExtension
	{
		/// <summary>
		/// Reference to an instance of <see cref="ICustomSessionFactory"/>
		/// obtained from the configuration
		/// </summary>
		private ICustomSessionFactory customSession;

		/// <summary>
		/// Reads the attribute <c>customSession</c> 
		/// from <see cref="MonoRailConfiguration"/> and
		/// instantiate it based on the type name provided.
		/// </summary>
		/// <exception cref="ConfigurationException">
		/// If the typename was not provided or the type 
		/// could not be instantiated/found
		/// </exception>
		/// <param name="configuration"></param>
		public override void Init(MonoRailConfiguration configuration)
		{
			XmlAttribute customSessionAtt = 
				configuration.ConfigSection.Attributes["customSession"];

			if (customSessionAtt == null || customSessionAtt.Value.Length == 0)
			{
				throw new ConfigurationException("The CustomSessionExtension requires that " + 
					"the type that implements ICustomSessionFactory be specified through the " + 
					"'customSession' attribute on 'monoRail' configuration node");
			}

			Type customSessType = MonoRailConfiguration.GetType(customSessionAtt.Value);

			if (customSessType == null)
			{
				throw new ConfigurationException("The Type for the custom session could not be loaded. " + 
					customSessionAtt.Value);
			}

			try
			{
				customSession = (ICustomSessionFactory) Activator.CreateInstance(customSessType);
			}
			catch(InvalidCastException)
			{
				throw new ConfigurationException("The Type for the custom session must " + 
					"implement ICustomSessionFactory. " + customSessionAtt.Value);
			}
		}

		/// <summary>
		/// Overrides the ISession instance on <see cref="RailsEngineContextAdapter"/>.
		/// </summary>
		/// <remarks>Note that the session available through IHttpContext is left untouched</remarks>
		/// <param name="context"></param>
		public override void OnRailsContextCreated(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			IDictionary session = customSession.ObtainSession(context);

			(context as RailsEngineContextAdapter).Session = session;
		}

		/// <summary>
		/// Retrives the ISession instance from <see cref="RailsEngineContextAdapter"/>.
		/// and invokes <see cref="ICustomSessionFactory.PersistSession"/>
		/// </summary>
		public override void OnRailsContextDiscarded(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			customSession.PersistSession(context.Session, context);
		}
	}
}
