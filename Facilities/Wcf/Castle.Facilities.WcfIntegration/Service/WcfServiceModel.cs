// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
		private bool hosted;
        private ICollection<Uri> baseAddresses;
		private ICollection<IWcfEndpoint> endpoints;
		private ICollection<IWcfBehavior> behaviors;

		#region IWcfServiceModel 

		public bool IsHosted
		{
			get { return hosted; }
			protected set { hosted = value; }
		}

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

		public ICollection<IWcfBehavior> Behaviors
		{
			get
			{
				if (behaviors == null)
				{
					behaviors = new List<IWcfBehavior>();
				}
				return behaviors;
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

		public T AddBehaviors(params object[] behaviors)
		{
			foreach (object behavior in behaviors)
			{
				Behaviors.Add(WcfExplcitBehavior.CreateFrom(behavior));
			}
			return (T)this;
		}

		public T LogMessages()
		{
			return AddBehaviors(typeof(LogMessageEndpointBehavior));
		}
	}
}