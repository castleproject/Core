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

namespace Castle.MicroKernel.Handlers
{
	using System;

	using Castle.Model;

	/// <summary>
	/// Summary description for DefaultHandlerFactory.
	/// </summary>
	[Serializable]
	public class DefaultHandlerFactory : IHandlerFactory
	{
		private IKernel kernel;

		public DefaultHandlerFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public virtual IHandler Create(ComponentModel model)
		{
			IHandler handler;

#if DOTNET2
			if (model.RequiresGenericArguments)
			{
				handler = new DefaultGenericHandler(model);
			}
			else
#endif
			{
				handler = new DefaultHandler(model);
			}

			handler.Init(kernel);
			
			return handler;
		}
	}
}
