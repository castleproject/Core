// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel
{
	using System;
	
	using Castle.MicroKernel.Model;

	public delegate void ChangeStateListenerDelegate( IHandler handler );

	/// <summary>
	/// Summary description for IHandler.
	/// </summary>
	public interface IHandler : IResolver, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="kernel"></param>
		void Init(IKernel kernel);

		/// <summary>
		/// 
		/// </summary>
		State ActualState { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		bool IsOwner(object instance);

        /// <summary>
        /// 
        /// </summary>
	    IComponentModel ComponentModel { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="changeStateDelegate"></param>
		void AddChangeStateListener( ChangeStateListenerDelegate changeStateDelegate );
	}
}