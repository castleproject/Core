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
	using System.Collections;
	using System.Xml;
	
	using Castle.Components.Common.EmailSender;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;

	/// <summary>
	/// Represents a set of MonoRail services entries
	/// </summary>
	public class ServiceEntryCollection : ISerializedConfig
	{
		private readonly Hashtable service2Impl = new Hashtable();
		private readonly IList customServices = new ArrayList();

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceEntryCollection"/> class.
		/// </summary>
		public ServiceEntryCollection()
		{
		}

		/// <summary>
		/// Gets the custom services.
		/// </summary>
		/// <value>The custom services.</value>
		public ICollection CustomServices
		{
			get { return customServices; }
		}

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlNodeList services = section.SelectNodes("services/service");
			
			foreach(XmlNode node in services)
			{
				ServiceEntry entry = new ServiceEntry();
				
				entry.Deserialize(node);
				
				if (entry.ServiceType == ServiceIdentification.Custom)
				{
					if (entry.Interface != null)
					{
						RegisterService(entry.Interface, entry.Service);
					}
					else
					{
						customServices.Add(entry.Service);
					}
				}
				else
				{
					RegisterService(entry.ServiceType, entry.Service);
				}
			}
		}

		#endregion

		/// <summary>
		/// Registers the service.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="service">The service.</param>
		public void RegisterService(ServiceIdentification id, Type service)
		{
			RegisterService(ToInterface(id), service);
		}

		/// <summary>
		/// Registers the service.
		/// </summary>
		/// <param name="inter">The inter.</param>
		/// <param name="service">The service.</param>
		public void RegisterService(Type inter, Type service)
		{
			service2Impl[inter] = service;
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		public Type GetService(ServiceIdentification id)
		{
			return (Type) service2Impl[ToInterface(id)];
		}

		/// <summary>
		/// Determines whether it has service.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified id has service; otherwise, <c>false</c>.
		/// </returns>
		public bool HasService(ServiceIdentification id)
		{
			return service2Impl.Contains(ToInterface(id));
		}

		/// <summary>
		/// Gets the service impl map.
		/// </summary>
		/// <value>The service impl map.</value>
		public IDictionary ServiceImplMap
		{
			get { return service2Impl; }
		}

		private static Type ToInterface(ServiceIdentification id)
		{
			switch(id)
			{
				case ServiceIdentification.ControllerFactory:
					return typeof(IControllerFactory);
				case ServiceIdentification.ViewComponentFactory:
					return typeof(IViewComponentFactory);
				case ServiceIdentification.FilterFactory:
					return typeof(IFilterFactory);
				case ServiceIdentification.EmailSender:
					return typeof(IEmailSender);
				case ServiceIdentification.ControllerDescriptorProvider:
					return typeof(IControllerDescriptorProvider);
				case ServiceIdentification.ResourceDescriptorProvider:
					return typeof(IResourceDescriptorProvider);
				case ServiceIdentification.RescueDescriptorProvider:
					return typeof(IRescueDescriptorProvider);
				case ServiceIdentification.LayoutDescriptorProvider:
					return typeof(ILayoutDescriptorProvider);
				case ServiceIdentification.HelperDescriptorProvider:
					return typeof(IHelperDescriptorProvider);
				case ServiceIdentification.FilterDescriptorProvider:
					return typeof(IFilterDescriptorProvider);
				case ServiceIdentification.EmailTemplateService:
					return typeof(IEmailTemplateService);
				case ServiceIdentification.ControllerTree:
					return typeof(IControllerTree);
				case ServiceIdentification.CacheProvider:	
					return typeof(ICacheProvider);
				case ServiceIdentification.ViewSourceLoader:
					return typeof(IViewSourceLoader);
				case ServiceIdentification.ScaffoldingSupport:
					return typeof(IScaffoldingSupport);
				case ServiceIdentification.ViewEngineManager:
					return typeof(IViewEngineManager);
				case ServiceIdentification.ResourceFactory:
					return typeof(IResourceFactory);
				case ServiceIdentification.ExecutorFactory:
					return typeof(IControllerLifecycleExecutorFactory);
				case ServiceIdentification.TransformationFilterFactory:
					return typeof(ITransformFilterFactory);
				case ServiceIdentification.TransformFilterDescriptorProvider:
					return typeof(ITransformFilterDescriptorProvider);
				case ServiceIdentification.UrlBuilder:
					return typeof(IUrlBuilder);
				case ServiceIdentification.UrlTokenizer:
					return typeof(IUrlTokenizer);
				case ServiceIdentification.ServerUtility:
					return typeof(IServerUtility);
				case ServiceIdentification.ValidatorRegistry:
					return typeof(IValidatorRegistry);
				case ServiceIdentification.AjaxProxyGenerator:
					return typeof(IAjaxProxyGenerator);
				case ServiceIdentification.ViewComponentDescriptorProvider:
					return typeof(IViewComponentDescriptorProvider);
				default:
					throw new NotSupportedException("Id not supported " + id);
			}
		}
	}
}
