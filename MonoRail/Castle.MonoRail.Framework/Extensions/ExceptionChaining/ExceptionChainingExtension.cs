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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using System;
	using System.Configuration;
	using System.Web;
	using System.Xml;

	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// This extension allow one to perform one or more steps
	/// in response to an exception threw by an action. 
	/// <seealso cref="IExceptionHandler"/>
	/// </summary>
	/// <remarks>
	/// To successfully install this extension you must register 
	/// it on the <c>extensions</c> node and the handlers within the <c>exception</c> node:
	/// <code>
	///   &lt;monoRail&gt;
	///   	&lt;extensions&gt;
	///   	  &lt;extension type="Castle.MonoRail.Framework.Extensions.ExceptionChaining.ExceptionChainingExtension, Castle.MonoRail.Framework" /&gt;
	///   	&lt;/extensions&gt;
	///   	
	///   	&lt;exception&gt;
	///   	  &lt;exceptionHandler type="Type name that implements IExceptionHandler" /&gt;
	///   	  &lt;exceptionHandler type="Type name that implements IExceptionHandler" /&gt;
	///   	&lt;/exception&gt;
	///   &lt;/monoRail&gt;
	/// </code>
	/// </remarks>
	public class ExceptionChainingExtension : AbstractMonoRailExtension
	{
		private IExceptionHandler firstHandler;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="configuration"></param>
		public override void Init(MonoRailConfiguration configuration)
		{
			XmlNodeList handlers = configuration.ConfigSection.SelectNodes("exception/exceptionHandler");

			HttpContext.Current.ApplicationInstance.Error += new EventHandler(ApplicationInstance_Error);

			foreach(XmlNode node in handlers)
			{
				XmlAttribute typeAtt = node.Attributes["type"];

				if (typeAtt == null)
				{
					// TODO: Throw configuration exception
				}

				InstallExceptionHandler(node, typeAtt.Value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public override void OnActionException(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			firstHandler.Process(context, serviceProvider);

			// Mark the request as processed (so if the 
			// ApplicationInstance_Error is invoked again, we wouldn't re-invoke the chain)

		}

		private void ApplicationInstance_Error(object sender, EventArgs e)
		{
			// TODO: Delegate to OnActionException
		}

		private void InstallExceptionHandler(XmlNode node, string typeName)
		{
			IExceptionHandler handler = null;

			Type handlerType = MonoRailConfiguration.GetType(typeName);

			if (handlerType == null)
			{
				throw new ConfigurationException("The Type for the custom session could not be loaded. " + 
					typeName);
			}

			try
			{
				handler = (IExceptionHandler) Activator.CreateInstance(handlerType);
			}
			catch(InvalidCastException)
			{
				throw new ConfigurationException("The Type for the custom session must " + 
					"implement ICustomSessionFactory. " + typeName);
			}

			IConfigurableHandler configurableHandler = handler as IConfigurableHandler;

			if (configurableHandler != null)
			{
				configurableHandler.Configure(node);
			}

			handler.Initialize();

			if (firstHandler == null)
			{
				firstHandler = handler;
			}
			else
			{
				IExceptionHandler navHandler = firstHandler;
				
				while(navHandler != null)
				{
					if (navHandler.Next == null)
					{
						navHandler.Next = handler;
						break;
					}

					navHandler = navHandler.Next;
				}
			}
		}
	}
}
