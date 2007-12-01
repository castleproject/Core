// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Enum for all known MonoRail services.
	/// </summary>
	public enum ServiceIdentification
	{
		/// <summary>
		/// Custom ( not know service )
		/// </summary>
		Custom,
		/// <summary>
		/// The <see cref="IControllerFactory"/> service
		/// </summary>
		ControllerFactory,
		/// <summary>
		/// The <see cref="IViewComponentFactory"/> service
		/// </summary>
		ViewComponentFactory,
		/// <summary>
		/// The <see cref="IViewSourceLoader"/> service.
		/// </summary>
		ViewSourceLoader,
		/// <summary>
		/// The <see cref="IFilterFactory"/> service.
		/// </summary>
		FilterFactory,
		/// <summary>
		/// The <see cref="IEmailSender"/> service.
		/// </summary>
		EmailSender,
		/// <summary>
		/// The <see cref="IControllerDescriptorProvider"/> service
		/// </summary>
		ControllerDescriptorProvider,
		/// <summary>
		/// The <see cref="IResourceDescriptorProvider"/> service
		/// </summary>
		ResourceDescriptorProvider,
		/// <summary>
		/// The <see cref="IViewComponentDescriptorProvider"/> service
		/// </summary>
		ViewComponentDescriptorProvider,
		/// <summary>
		/// The <see cref="IRescueDescriptorProvider"/> service
		/// </summary>
		RescueDescriptorProvider,
		/// <summary>
		/// The <see cref="ILayoutDescriptorProvider"/> service
		/// </summary>
		LayoutDescriptorProvider,
		/// <summary>
		/// The <see cref="IHelperDescriptorProvider"/> service
		/// </summary>
		HelperDescriptorProvider,
		/// <summary>
		/// The <see cref="IFilterDescriptorProvider"/> service
		/// </summary>
		FilterDescriptorProvider,
		/// <summary>
		/// The <see cref="IResourceFactory"/> service
		/// </summary>
		ResourceFactory,
		/// <summary>
		/// The <see cref="IEmailTemplateService"/> service
		/// </summary>
		EmailTemplateService,
		/// <summary>
		/// The <see cref="IControllerTree"/> service
		/// </summary>
		ControllerTree,
		/// <summary>
		/// The <see cref="ICacheProvider"/> service
		/// </summary>
		CacheProvider,
		/// <summary>
		/// The <see cref="IScaffoldingSupport"/> service
		/// </summary>
		ScaffoldingSupport,
		/// <summary>
		/// The <see cref="IControllerLifecycleExecutorFactory"/> service
		/// </summary>
		ExecutorFactory,
		/// <summary>
		/// The <see cref="ITransformFilterDescriptorProvider"/> service
		/// </summary>
		TransformFilterDescriptorProvider,
		/// <summary>
		/// The <see cref="ITransformFilterFactory"/> service
		/// </summary>
		TransformationFilterFactory,
		/// <summary>
		/// The <see cref="IViewEngineManager"/> service
		/// </summary>
		ViewEngineManager,
		/// <summary>
		/// The <see cref="IUrlBuilder"/> service
		/// </summary>
		UrlBuilder,
		/// <summary>
		/// The <see cref="IUrlTokenizer"/> service
		/// </summary>
		UrlTokenizer,
		/// <summary>
		/// The <see cref="IServerUtility"/> service
		/// </summary>
		ServerUtility,
		/// <summary>
		/// The <see cref="IValidatorRegistry"/> service
		/// </summary>
		ValidatorRegistry,
		/// <summary>
		/// The <see cref="IAjaxProxyGenerator"/> service
		/// </summary>
		AjaxProxyGenerator,
	}

	/// <summary>
	/// Represents a MonoRail service entry
	/// </summary>
	public class ServiceEntry : ISerializedConfig
	{
		private ServiceIdentification serviceType;
		private Type service;
		private Type _interface;

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlAttribute idAtt = section.Attributes["id"];
			XmlAttribute typeAtt = section.Attributes["type"];
			XmlAttribute interAtt = section.Attributes["interface"];
			
			if (idAtt == null || idAtt.Value == String.Empty)
			{
				String message = "To add a service, please specify the 'id' attribute. " + 
					"Check the documentation for more information";
				throw new ConfigurationErrorsException(message);
			}

			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				String message = "To add a service, please specify the 'type' attribute. " + 
					"Check the documentation for more information";
				throw new ConfigurationErrorsException(message);
			}
			
			try
			{
				serviceType = (ServiceIdentification) 
					Enum.Parse(typeof(ServiceIdentification), idAtt.Value, true);
			}
			catch(Exception ex)
			{
				String message = "Invalid service id: " + idAtt.Value;
				throw new ConfigurationErrorsException(message, ex);
			}
			
			service = TypeLoadUtil.GetType(typeAtt.Value);
			
			if (interAtt != null)
			{
				_interface = TypeLoadUtil.GetType(interAtt.Value);
			}
		}
		
		#endregion

		/// <summary>
		/// Gets the type of the service.
		/// </summary>
		/// <value>The type of the service.</value>
		public ServiceIdentification ServiceType
		{
			get { return serviceType; }
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <value>The service.</value>
		public Type Service
		{
			get { return service; }
		}

		/// <summary>
		/// Gets the interface.
		/// </summary>
		/// <value>The interface.</value>
		public Type Interface
		{
			get { return _interface; }
		}
	}
}
