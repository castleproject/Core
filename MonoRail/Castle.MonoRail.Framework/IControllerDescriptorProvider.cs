// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Providers;

	/// <summary>
	/// Creator delegate
	/// </summary>
	public delegate ControllerMetaDescriptor MetaCreatorHandler();

	/// <summary>
	/// ControllerMetaDescriptor delegate
	/// </summary>
	/// <returns></returns>
	public delegate void ControllerMetaDescriptorHandler(ControllerMetaDescriptor metaDesc);

	/// <summary>
	/// Action meta creator delegate
	/// </summary>
	public delegate ActionMetaDescriptor ActionMetaCreatorHandler();

	/// <summary>
	/// ActionMetaDescriptor delegate
	/// </summary>
	public delegate void ActionMetaDescriptorHandler(ActionMetaDescriptor actionMetaDesc);

	/// <summary>
	/// Defines the contract for implementations that should
	/// collect from one or more sources the meta information that
	/// dictates the <see cref="IController"/> behavior and the actions it exposes.
	/// </summary>
	public interface IControllerDescriptorProvider : IProvider
	{
		/// <summary>
		/// Occurs when the providers needs to create a <see cref="ControllerMetaDescriptor" />.
		/// </summary>
		event MetaCreatorHandler Create;

		/// <summary>
		/// Occurs when the meta descriptor is about to the returned to the caller.
		/// </summary>
		event ControllerMetaDescriptorHandler AfterProcess;

		/// <summary>
		/// Occurs when the providers needs to create a <see cref="ActionMetaDescriptor" />.
		/// </summary>
		event ActionMetaCreatorHandler ActionCreate;

		/// <summary>
		/// Occurs when the meta descriptor is about to be included on the <see cref="ControllerMetaDescriptor"/>.
		/// </summary>
		event ActionMetaDescriptorHandler AfterActionProcess;

		/// <summary>
		/// Builds the descriptor.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		ControllerMetaDescriptor BuildDescriptor(IController controller);

		/// <summary>
		/// Builds the descriptor.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <returns></returns>
		ControllerMetaDescriptor BuildDescriptor(Type controllerType);
	}
}
