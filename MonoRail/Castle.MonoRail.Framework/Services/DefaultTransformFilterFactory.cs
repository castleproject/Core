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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.IO;
	using Castle.Core;
	using Castle.Core.Logging;

	/// <summary>
	/// Standard implementation of <see cref="ITransformFilterFactory"/>.
	/// </summary>
	public class DefaultTransformFilterFactory : IServiceEnabledComponent, ITransformFilterFactory
	{		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory)provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(DefaultTransformFilterFactory));
			}
		}

		#endregion
		
		/// <summary>
		/// Creates a transformfilter instance
		/// </summary>
		/// <param name="transformFilterType">The transformfilter's type</param>
		/// <param name="baseStream">The filter's basestream to write to</param>
		/// <returns>The transformfilter instance</returns>
		public virtual ITransformFilter Create(Type transformFilterType, Stream baseStream)
		{
			if (transformFilterType == null) throw new ArgumentNullException("transformFilterType");
			if (baseStream == null) throw new ArgumentNullException("baseStream");

			if (logger.IsDebugEnabled)
			{
				logger.Debug("Creating filter " + transformFilterType.FullName);
			}

			try
			{
				return (ITransformFilter)Activator.CreateInstance(transformFilterType, new object[] { baseStream });
			}
			catch (Exception ex)
			{
				logger.Error("Could not create transformfilter instance. Activation failed", ex);

				throw;
			}
		}

		/// <summary>
		/// Releases a transformfilter instance
		/// </summary>
		/// <param name="transformFilter">The filter instance</param>
		public virtual void Release(ITransformFilter transformFilter)
		{
			if (logger.IsDebugEnabled)
			{
				logger.Debug("Releasing transformfilter " + transformFilter);
			}
		}

	}
}
