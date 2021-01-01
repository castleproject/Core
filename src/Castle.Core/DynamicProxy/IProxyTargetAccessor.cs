// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	/// <summary>
	///   Exposes access to the target object and interceptors of proxy objects.
	///   This is a DynamicProxy infrastructure interface and should not be implemented yourself.
	/// </summary>
	public interface IProxyTargetAccessor
	{
		/// <summary>
		///   Get the proxy target (note that null is a valid target!)
		/// </summary>
		object DynProxyGetTarget();

		/// <summary>
		///   Set the proxy target.
		/// </summary>
		/// <param name="target">New proxy target.</param>
		void DynProxySetTarget(object target);

		/// <summary>
		///   Gets the interceptors for the proxy
		/// </summary>
		IInterceptor[] GetInterceptors();
	}
}