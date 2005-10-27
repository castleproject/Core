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

namespace Castle.MonoRail.Framework.Extensions.Session
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Configuration;

	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Configuration;


	/// <summary>
	/// 
	/// </summary>
	public class CustomSessionExtension : IMonoRailExtension
	{
		private ICustomSessionFactory customSession;

		public CustomSessionExtension()
		{
		}

		public void Init(MonoRailConfiguration configuration)
		{
			XmlAttribute customSessionAtt = 
				configuration.ConfigSection.Attributes["customsession"];

			if (customSessionAtt == null || customSessionAtt.Value.Length == 0)
			{
				throw new ConfigurationException("The CustomSessionExtension requires that " + 
					"the type that implements ICustomSessionFactory be specified through the " + 
					"'customsession' attribute on 'monoRail' configuration node");
			}

			Type customSessType = MonoRailConfiguration.GetType(customSessionAtt.Value);

			if (customSessType == null)
			{
				throw new ConfigurationException("The Type for the custom session could not be loaded. " + 
					customSessionAtt.Value);
			}

			try
			{
				customSession = (ICustomSessionFactory) 
					Activator.CreateInstance(customSessType);
			}
			catch(InvalidCastException)
			{
				throw new ConfigurationException("The Type for the custom session must " + 
					"implement ICustomSessionFactory. " + customSessionAtt.Value);
			}
		}

		public void OnRailsContextCreated(IRailsEngineContext context)
		{
			IDictionary session = customSession.ObtainSession(context);

			(context as RailsEngineContextAdapter).Session = session;
		}

		public void OnRailsContextDiscarded(IRailsEngineContext context)
		{
			customSession.PersistSession(context.Session, context);
		}
	}
}
