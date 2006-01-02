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

namespace Castle.Windsor.Adapters.ComponentModel
{
	using System;

	/// <summary>
	/// Implementation of <see cref="IContainerAdapter"/> that assumes ownership of the
	/// wrapped <see cref="IWindsorContainer"/>.  If this adapter is disposed, the underlying
	/// <see cref="IWindsorContainer"/> is diposed as well.
	/// </summary>
	public class ContainerAdapter : ContainerWrapper
	{
		#region ContainerAdapter Constructors 

		/// <summary>
		/// Constructs a default ContainerAdapter.
		/// </summary>
		public ContainerAdapter() : this( null, null )
		{
			// Empty
		}

		/// <summary>
		/// Constructs a chained ContainerAdapter.
		/// </summary>
		/// <param name="parentProvider">The parent <see cref="IServiceProvider"/>.</param>
		public ContainerAdapter(IServiceProvider parentProvider) : this( null, parentProvider )
		{
			// Empty
		}

		/// <summary>
		/// Constructs an initial ContainerAdapter.
		/// </summary>
		/// <param name="container">The <see cref="IWindsorContainer"/> to adapt.</param>
		public ContainerAdapter(IWindsorContainer container) : base( container, null )
		{
			// Empty
		}

		/// <summary>
		/// Constructs an initial ContainerAdapter.
		/// </summary>
		/// <param name="container">The <see cref="IWindsorContainer"/> to adapt.</param>
		/// <param name="parentProvider">The parent <see cref="IServiceProvider"/>.</param>
		public ContainerAdapter(IWindsorContainer container, IServiceProvider parentProvider)
			: base( container, parentProvider )
		{
			// Empty
		}

		#endregion

		#region ContainerWrapper Members

		protected override IWindsorContainer CreateDefaultWindsorContainer()
		{
			return new WindsorContainer();
		}

		protected override void InternalDisposeContainer()
		{
			Container.Dispose();
		}

		#endregion
	}
}
