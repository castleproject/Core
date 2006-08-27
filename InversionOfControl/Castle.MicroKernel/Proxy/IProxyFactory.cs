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

namespace Castle.MicroKernel
{
	using Castle.Core;

	/// <summary>
	/// Defines the contract used by the kernel 
	/// to obtain proxies for components. The implementor
	/// must return a proxied instance that dispatch 
	/// the invocation to the registered interceptors in the model
	/// </summary>
	public interface IProxyFactory
	{
		/// <summary>
		/// Implementors must create a proxy based on 
		/// the information exposed by ComponentModel
		/// </summary>
		/// <param name="kernel"></param>
		/// <param name="model"></param>
		/// <param name="constructorArguments"></param>
		/// <returns></returns>
		object Create( IKernel kernel, ComponentModel model, params object[] constructorArguments );
	}
}
