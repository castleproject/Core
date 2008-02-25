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

namespace Castle.MonoRail.Framework.Providers
{
	using System.Reflection;
	using Castle.Core.Logging;
	using Descriptors;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultReturnBinderDescriptorProvider : IReturnBinderDescriptorProvider
	{
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		#region IMRServiceEnabled implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IMonoRailServices provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultReturnBinderDescriptorProvider));
			}
		}

		#endregion

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="method">The action method.</param>
		/// <returns></returns>
		public ReturnBinderDescriptor Collect(MethodInfo method)
		{
			object[] atts = method.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(IReturnBinder), false);

			if (atts.Length != 0)
			{
				return new ReturnBinderDescriptor(method.ReturnType, (IReturnBinder) atts[0]);
			}

			return null;
		}
	}
}
