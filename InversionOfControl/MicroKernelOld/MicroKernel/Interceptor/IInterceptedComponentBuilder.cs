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

namespace Castle.MicroKernel.Interceptor
{
	using System;

	/// <summary>
	/// Implementors should define their strategy to create 
	/// <see cref="IInterceptedComponent"/> instance for the
	/// specified component instance.
	/// </summary>
	public interface IInterceptedComponentBuilder
	{
		/// <summary>
		/// Should return an implementation of <see cref="IInterceptedComponent"/>
		/// which should be responsible for exposing a proxied instance 
		/// capable of dealing with an interception chain.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		IInterceptedComponent CreateInterceptedComponent( object instance, Type service );
	}
}
