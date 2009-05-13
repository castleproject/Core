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
	using Castle.MicroKernel;
	using System.Collections.Generic;
	using Castle.MicroKernel.Registration;

	/// <summary>
	/// Describes a list of WCF channels to register.
	/// </summary>
	public class WcfClientDescriptor : IRegistration
	{
		private readonly IEnumerable<IWcfClientModel> channels;
		private Action<ComponentRegistration> configurer;

		internal WcfClientDescriptor(IEnumerable<IWcfClientModel> channels)
		{
			ValidateChannels(channels);
			this.channels = channels;
		}

		/// <summary>
		/// Allows customized configurations of the channels.
		/// </summary>
		/// <param name="configurer">The configuration action.</param>
		/// <returns></returns>
		public WcfClientDescriptor Configure(Action<ComponentRegistration> configurer)
		{
			this.configurer = configurer;
			return this;
		}

		#region IRegistration Members

		void IRegistration.Register(IKernel kernel)
		{
			foreach (IWcfClientModel channel in channels)
			{
				var registration = Component.For(channel.Contract);
				registration.DependsOn(Property.ForKey("channel").Eq(channel));

				if (configurer != null)
				{
					configurer(registration);
				}

				if (String.IsNullOrEmpty(registration.Name))
				{
					registration.Named(Guid.NewGuid().ToString());
				}

				if (!kernel.HasComponent(registration.Name))
				{
					kernel.Register(registration);
				}
			}
		}

		#endregion

		private void ValidateChannels(IEnumerable<IWcfClientModel> channels)
		{
			foreach (var channel in channels)
			{
				if (channel.Contract == null)
				{
					throw new ArgumentException("The channel does not specify a contract.");
				}

				if (!channel.Contract.IsInterface)
				{
					throw new ArgumentException("The channel contract must be an interface.");
				}
			}
		}
	}
}
