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
// limitations under the License

namespace Castle.Facilities.WcfIntegration
{
	using System.Reflection;

	/// <summary>
	/// Policy for determining if channels should be refreshed when
	/// they are in an invalid state.  i.e. closed, faulted, or aborted.
	/// </summary>
	public interface IRefreshChannelPolicy : IWcfChannelPolicy
	{
		/// <summary>
		/// Called when an attempt is made to use a channel in an invalid state
		/// 	i.e. closed or faulted, or aborted./>
		/// </summary>
		/// <param name="channelHolder">The channel holder.</param>
		/// <param name="method">The attempted method.</param>
		void WantsToUseUnusableChannel(IWcfChannelHolder channelHolder, MethodInfo method);
	}
}
