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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections.Generic;
	using Castle.Core;
	using Castle.MicroKernel.Proxy;

	/// <summary>
	/// Summary description for DefaultGenericHandler.
	/// </summary>
	/// <remarks>
	/// TODO: Consider refactoring AbstractHandler moving lifestylemanager
	/// creation to DefaultHandler
	/// </remarks>
	[Serializable]
	public class DefaultGenericHandler : AbstractHandler
	{
		private readonly IDictionary<Type, IHandler> type2SubHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultGenericHandler"/> class.
		/// </summary>
		/// <param name="model"></param>
		public DefaultGenericHandler(ComponentModel model) : base(model)
		{
			type2SubHandler = new Dictionary<Type, IHandler>();
		}

		public override object Resolve(CreationContext context)
		{
			Type implType = ComponentModel.Implementation.MakeGenericType(context.GenericArguments);

			IHandler handler = GetSubHandler(context, implType);

			// so the generic version wouldn't be considered as well
			using(context.EnterResolutionContext(this, false))
			{
				return handler.Resolve(context);
			}
		}

		public override bool Release(object instance)
		{
			Type genericType = ProxyUtil.GetUnproxiedType(instance);

			IHandler handler = GetSubHandler(CreationContext.Empty, genericType);

			return handler.Release(instance);
		}

		protected IHandler GetSubHandler(CreationContext context, Type genericType)
		{
			lock (type2SubHandler)
			{
				IHandler handler;

				if (type2SubHandler.ContainsKey(genericType))
				{
					handler = type2SubHandler[genericType];
				}
				else
				{
					Type service = ComponentModel.Service.MakeGenericType(context.GenericArguments);

					ComponentModel newModel = Kernel.ComponentModelBuilder.BuildModel(
						ComponentModel.Name, service, genericType, ComponentModel.ExtendedProperties);

					newModel.ExtendedProperties[ComponentModel.SkipRegistration] = true;
					CloneParentProperties(newModel);

					// Create the handler and add to type2SubHandler before we add to the kernel.
					// Adding to the kernel could satisfy other dependencies and cause this method
					// to be called again which would result in extra instances being created.
					handler = Kernel.HandlerFactory.Create(newModel);
					type2SubHandler[genericType] = handler;

					Kernel.AddCustomComponent(newModel);
				}

				return handler;
			}
		}

		/// <summary>
		/// Clone some of the parent componentmodel properties to the generic subhandler.
		/// </summary>
		/// <remarks>
		/// The following properties are copied:
		/// <list type="bullet">
		/// <item>
		///		<description>The <see cref="LifestyleType"/></description>
		/// </item>
		/// <item>
		///		<description>The <see cref="ComponentModel.Interceptors"/></description>
		/// </item>
		/// </list>
		/// </remarks>
		/// <param name="newModel">the subhandler</param>
		private void CloneParentProperties(ComponentModel newModel)
		{
			// Inherits from LifeStyle's context.
			newModel.LifestyleType = ComponentModel.LifestyleType;

			// Inherit the parent handler interceptors.
			foreach (InterceptorReference interceptor in ComponentModel.Interceptors)
			{
				// we need to check that we are not adding the inteceptor again, if it was added
				// by a facility already
				newModel.Interceptors.AddIfNotInCollection(interceptor);
			}
		}
	}
}