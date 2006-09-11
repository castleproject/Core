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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Collections;
	using System.Xml;
	
	using Castle.Components.Common.EmailSender;
	using Castle.MonoRail.Framework.Internal;
	

	public class ServiceEntryCollection : ISerializedConfig
	{
		private Hashtable service2Impl = new Hashtable();
		private IList customServices = new ArrayList();
		
		public ServiceEntryCollection()
		{
		}

		public ICollection CustomServices
		{
			get { return customServices; }
		}

		#region ISerializedConfig implementation
		
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

		public void RegisterService(ServiceIdentification id, Type service)
		{
			RegisterService(ToInterface(id), service);
		}
		
		public void RegisterService(Type inter, Type service)
		{
			service2Impl[inter] = service;
		}
		
		public Type GetService(ServiceIdentification id)
		{
			return (Type) service2Impl[ToInterface(id)];
		}
		
		public bool HasService(ServiceIdentification id)
		{
			return service2Impl.Contains(ToInterface(id));
		}
		
		public IDictionary ServiceImplMap
		{
			get { return service2Impl; }
		}

		private Type ToInterface(ServiceIdentification id)
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
				case ServiceIdentification.ViewEngine:
					return typeof(IViewEngine);
				case ServiceIdentification.ResourceFactory:
					return typeof(IResourceFactory);
				default:
					throw new NotSupportedException("Id not supported " + id.ToString());
			}
		}
	}
}
