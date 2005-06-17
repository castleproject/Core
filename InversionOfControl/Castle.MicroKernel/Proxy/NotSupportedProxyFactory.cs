// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Proxy
{
	using System;

	using Castle.Model;

	/// <summary>
	/// This is a placeholder implementation of <see cref="IProxyFactory"/>.
	/// </summary>
	/// <remarks>
	/// The decision to supply no implementation for <see cref="IProxyFactory"/>
	/// is supported by the fact that the MicroKernel should be a thin
	/// assembly with the minimal set of features, although extensible.
	/// Providing the support for this interface would obligate 
	/// the user to import another assembly, even if the large majority of
	/// simple cases, no use use of interceptors will take place.
	/// If you want to use however, see the Windsor container.
	/// </remarks>
	[Serializable]
	public class NotSupportedProxyFactory : IProxyFactory
	{
		#region IProxyFactory Members

		public object Create(IKernel kernel, ComponentModel mode, params object[] constructorArguments)
		{
			throw new NotImplementedException(
				"You must supply an implementation of IProxyFactory " + 
				"to use interceptors on the Microkernel");
		}

		#endregion
	}
}
