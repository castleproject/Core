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

namespace Castle.MonoRail.Framework
{
	using System;

	using Castle.MonoRail.Framework.Configuration;

	/// <summary>
	/// 
	/// </summary>
	public class ExtensionComposite : IMonoRailExtension
	{
		private readonly IMonoRailExtension[] extensions;

		public ExtensionComposite(IMonoRailExtension[] extensions)
		{
			this.extensions = extensions;
		}

		public bool HasExtension
		{
			get { return extensions.Length != 0; }
		}

		/// <summary>
		/// Implementors have a chance to read custom 
		/// elements from <see cref="MonoRailConfiguration"/>
		/// </summary>
		/// <param name="configuration"></param>
		public void Init(MonoRailConfiguration configuration)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invoked when an instance of the implementation 
		/// of <see cref="IRailsEngineContext"/> is created
		/// </summary>
		/// <remarks>
		/// The extension instance is shared and should not 
		/// hold a reference to the context
		/// </remarks>
		/// <param name="context"></param>
		public void OnRailsContextCreated(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			foreach(IMonoRailExtension extension in extensions)
			{
				extension.OnRailsContextCreated(context, serviceProvider);
			}
		}

		/// <summary>
		/// Invoked when an instance of the implementation 
		/// of <see cref="IRailsEngineContext"/> is about
		/// to be released.
		/// </summary>
		/// <remarks>
		/// The extension instance is shared and should not 
		/// hold a reference to the context
		/// </remarks>
		/// <param name="context"></param>
		public void OnRailsContextDiscarded(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			foreach(IMonoRailExtension extension in extensions)
			{
				extension.OnRailsContextDiscarded(context, serviceProvider);
			}
		}

		public void OnActionException(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			foreach(IMonoRailExtension extension in extensions)
			{
				extension.OnActionException(context, serviceProvider);
			}
		}
	}
}
