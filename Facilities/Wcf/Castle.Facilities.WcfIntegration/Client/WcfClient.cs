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

	/// <summary>
	/// Simplifies registration of a list of WCF client channels.
	/// </summary>
	public static class WcfClient
	{
		/// <summary>
		/// Prepares to register a list of WCF client channels.
		/// </summary>
		/// <param name="channels">The channels.</param>
		/// <returns>The <see cref="WcfClientDescriptor"/></returns>
		public static WcfClientDescriptor ForChannels(params IWcfClientModel[] channels)
		{
			return ForChannels((IEnumerable<IWcfClientModel>)channels);
		}

		/// <summary>
		/// Prepares to register a list of WCF client channels.
		/// </summary>
		/// <param name="channels">The channels.</param>
		/// <returns>The <see cref="WcfClientDescriptor"/></returns>
		public static WcfClientDescriptor ForChannels(IEnumerable<IWcfClientModel> channels)
		{
			if (channels == null)
			{
				throw new ArgumentNullException("channels");
			}

			return new WcfClientDescriptor(channels);
		}
	}
}
