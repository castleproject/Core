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

namespace Castle.Windsor
{
	using System;

	using Castle.MicroKernel;

	/// <summary>
	/// Summary description for IWindsorContainer.
	/// </summary>
	public interface IWindsorContainer : IDisposable
	{
		IKernel Kernel
		{
			get;
		}

		IWindsorContainer Parent
		{
			get; set;
		}

		void AddFacility( IFacility facility );

		void AddComponent( String key, Type classType );

		void AddComponent( String key, Type serviceType, Type classType );

		object Resolve( String key );

		object Resolve( Type service );

		void Release( object instance );

		void AddChildContainer(IWindsorContainer childContainer);
	}
}
