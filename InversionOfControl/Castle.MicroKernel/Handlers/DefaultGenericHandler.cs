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

#if DOTNET2

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections.Generic;

	using Castle.Core;

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

		public DefaultGenericHandler(ComponentModel model) : base(model)
		{
			type2SubHandler = new Dictionary<Type, IHandler>();
		}

		public override object Resolve(CreationContext context)
		{
			Type implType = ComponentModel.Implementation.MakeGenericType(context.GenericArguments);

			IHandler handler = GetSubHandler(context, implType);

			return handler.Resolve(context);
		}

		public override void Release(object instance)
		{
			IHandler handler = GetSubHandler(CreationContext.Empty, instance.GetType());

			handler.Release(instance);
		}

		protected IHandler GetSubHandler(CreationContext context, Type genericType)
		{
			lock (type2SubHandler)
			{
				IHandler handler = null;

				if (type2SubHandler.ContainsKey(genericType))
				{
					handler = type2SubHandler[genericType];
				}
				else
				{
					Type service = ComponentModel.Service.MakeGenericType(context.GenericArguments);

					ComponentModel newModel = Kernel.ComponentModelBuilder.BuildModel(
						ComponentModel.Name, service, genericType, null);

					handler = Kernel.HandlerFactory.Create(newModel);

					type2SubHandler[genericType] = handler;
				}

				return handler;
			}
		}
	}
}

#endif