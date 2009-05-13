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
	using System.ServiceModel;

	/// <summary>
	/// Delegate for creating channels.
	/// </summary>
	public delegate object ChannelCreator();

	/// <summary>
	/// The contract for building client channels.
	/// </summary>
	/// <typeparam name="M">The <see cref="IWcfClientModel"/> type.</typeparam>
	public interface IClientChannelBuilder<M> where M : IWcfClientModel
	{
		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <param name="burden">Receives the client burden.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		ChannelCreator GetChannelCreator(M clientModel, out IWcfBurden burden);

		/// <summary>
		/// Get a delegate capable of creating channels.
		/// </summary>
		/// <param name="clientModel">The client model.</param>
		/// <param name="contract">The contract override.</param>
		/// <param name="burden">Receives the client burden.</param>
		/// <returns>The <see cref="ChannelCreator"/></returns>
		ChannelCreator GetChannelCreator(M clientModel, Type contract, out IWcfBurden burden);
	}
}
