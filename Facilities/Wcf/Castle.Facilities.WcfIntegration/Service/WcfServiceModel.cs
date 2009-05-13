// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
    using System;
    using System.Collections.Generic;
	using Castle.Facilities.WcfIntegration.Behaviors;

    public abstract class WcfServiceModelBase : IWcfServiceModel
    {
        private ICollection<Uri> baseAddresses;
		private ICollection<IWcfEndpoint> endpoints;
		private ICollection<IWcfExtension> extensions;

		#region IWcfServiceModel 

		public bool IsHosted { get; protected set; }

		public bool? ShouldOpenEagerly { get; protected set; }

		public ICollection<Uri> BaseAddresses
		{
			get
			{
				if (baseAddresses == null)
				{
					baseAddresses = new List<Uri>();
				}
				return baseAddresses;
			}
			set { baseAddresses = value; }
		}

		public ICollection<IWcfEndpoint> Endpoints
		{
			get
			{
				if (endpoints == null)
				{
					endpoints = new List<IWcfEndpoint>();
				}
				return endpoints;
			}
			set { endpoints = value; }
		}

		public ICollection<IWcfExtension> Extensions
		{
			get
			{
				if (extensions == null)
				{
					extensions = new List<IWcfExtension>();
				}
				return extensions;
			}
		}

		#endregion
	}

	public abstract class WcfServiceModel<T> : WcfServiceModelBase
		where T : WcfServiceModel<T>
	{
		public T Hosted()
		{
			IsHosted = true;
			return (T)this;
		}

		public T OpenEagerly()
		{
			ShouldOpenEagerly = true;
			return (T)this;
		}

		public T AddBaseAddresses(params Uri[] baseAddresses)
		{
			foreach (Uri baseAddress in baseAddresses)
			{
				BaseAddresses.Add(baseAddress);
			}
			return (T)this;
		}

		public T AddBaseAddresses(params string[] baseAddresses)
		{
			foreach (string baseAddress in baseAddresses)
			{
				BaseAddresses.Add(new Uri(baseAddress, UriKind.Absolute));
			}
			return (T)this;
		}

		public T AddEndpoints(params IWcfEndpoint[] endpoints)
		{
			foreach (IWcfEndpoint endpoint in endpoints)
			{
				Endpoints.Add(endpoint);
			}
			return (T)this;
		}

		public T AddExtensions(params object[] extensions)
		{
			foreach (object extension in extensions)
			{
				Extensions.Add(WcfExplicitExtension.CreateFrom(extension));
			}
			return (T)this;
		}

		#region Logging

		public T LogMessages()
		{
			return AddExtensions(typeof(LogMessageEndpointBehavior));
		}

		public T LogMessages<F>()
			where F : IFormatProvider, new()
		{
			return LogMessages<F>(null);
		}

		public T LogMessages<F>(string format)
			where F : IFormatProvider, new()
		{
			return LogMessages(new F(), format);
		}

		public T LogMessages(IFormatProvider formatter)
		{
			return LogMessages(formatter, null);
		}

		public T LogMessages(IFormatProvider formatter, string format)
		{
			return LogMessages().AddExtensions(new LogMessageFormat(formatter, format));
		}

		public T LogMessages(string format)
		{
			return LogMessages().AddExtensions(new LogMessageFormat(format));
		}

		#endregion
	}
}