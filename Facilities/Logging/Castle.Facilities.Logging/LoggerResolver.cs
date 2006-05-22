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

namespace Castle.Facilities.Logging
{
	using System;
	
	using Castle.Model;
	using Castle.MicroKernel;
	using Castle.Services.Logging;

	/// <summary>
	/// Custom resolver used by the MicroKernel. It gives
	/// us some contextual information that we use to set up a logging
	/// before satisfying the dependency
	/// </summary>
	public class LoggerResolver : ISubDependencyResolver
	{
		public ILoggerFactory loggerFactory;

		public LoggerResolver(ILoggerFactory loggerFactory)
		{
			if (loggerFactory == null) throw new ArgumentNullException("loggerFactory");

			this.loggerFactory = loggerFactory;
		}

		public object Resolve(Castle.MicroKernel.CreationContext context,
			ComponentModel model, 
			DependencyModel dependency)
		{
			if (CanResolve(context, model, dependency))
			{
				return loggerFactory.Create( model.Implementation );
			}

			return null;
		}

		public bool CanResolve(Castle.MicroKernel.CreationContext context, ComponentModel model, DependencyModel dependency)
		{
			return dependency.TargetType == typeof(ILogger);
		}
	}
}
