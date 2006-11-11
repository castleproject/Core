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
	using System.Configuration;
	using System.Xml;

	public enum ServiceIdentification
	{
		Custom,
		ControllerFactory,
		ViewEngine,
		ViewComponentFactory,
		ViewSourceLoader,
		FilterFactory,
		EmailSender,
		ControllerDescriptorProvider,
		ResourceDescriptorProvider,
		RescueDescriptorProvider,
		LayoutDescriptorProvider,
		HelperDescriptorProvider,
		FilterDescriptorProvider,
		ResourceFactory,
		EmailTemplateService,
		ControllerTree,
		CacheProvider,
		ScaffoldingSupport,
		ExecutorFactory
	}

	public class ServiceEntry : ISerializedConfig
	{
		private ServiceIdentification serviceType;
		private Type service;
		private Type _interface;

		#region ISerializedConfig implementation

		public void Deserialize(XmlNode section)
		{
			XmlAttribute idAtt = section.Attributes["id"];
			XmlAttribute typeAtt = section.Attributes["type"];
			XmlAttribute interAtt = section.Attributes["interface"];
			
			if (idAtt == null || idAtt.Value == String.Empty)
			{
				String message = "To add a service, please specify the 'id' attribute. " + 
					"Check the documentation for more information";
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}

			if (typeAtt == null || typeAtt.Value == String.Empty)
			{
				String message = "To add a service, please specify the 'type' attribute. " + 
					"Check the documentation for more information";
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}
			
			try
			{
				serviceType = (ServiceIdentification) 
					Enum.Parse(typeof(ServiceIdentification), idAtt.Value, true);
			}
			catch(Exception ex)
			{
				String message = "Invalid service id: " + idAtt.Value;
#if DOTNET2
				throw new ConfigurationErrorsException(message, ex);
#else
				throw new ConfigurationException(message, ex);
#endif
			}
			
			service = TypeLoadUtil.GetType(typeAtt.Value);
			
			if (interAtt != null)
			{
				_interface = TypeLoadUtil.GetType(interAtt.Value);
			}
		}
		
		#endregion

		public ServiceIdentification ServiceType
		{
			get { return serviceType; }
		}

		public Type Service
		{
			get { return service; }
		}

		public Type Interface
		{
			get { return _interface; }
		}
	}
}
