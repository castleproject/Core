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

namespace Castle.Services.Logging.Log4netIntegration
{
	using System;
	using Castle.Core.Logging;
	using log4net;
	using ILogger=log4net.Core.ILogger;
	using Logger = Castle.Core.Logging.ILogger;
	using ExtendedLogger = Castle.Core.Logging.IExtendedLogger;

	public class ExtendedLog4netLogger : Log4netLogger, ExtendedLogger
	{
		private static readonly IContextProperties threadContextProperties = new ThreadContextProperties();
		private static readonly IContextProperties globalContextProperties = new GlobalContextProperties();
		private static readonly IContextStacks threadContextStacks = new ThreadContextStacks();

		private ExtendedLog4netFactory factory;

		public ExtendedLog4netLogger(ILog log, ExtendedLog4netFactory factory) : this(log.Logger, factory)
		{
		}

		public ExtendedLog4netLogger(ILogger logger, ExtendedLog4netFactory factory)
		{
			Logger = logger;
			Factory = factory;
		}

		public override Logger CreateChildLogger(String name)
		{
			return CreateExtendedChildLogger(name);
		}

		public ExtendedLogger CreateExtendedChildLogger(string name)
		{
			return Factory.Create(Logger.Name + "." + name);
		}

		protected internal new ExtendedLog4netFactory Factory
		{
			get { return factory; }
			set { factory = value; }
		}

		#region IExtendedLogger Members

		/// <summary>
		/// Exposes the Global Context of the extended logger. 
		/// </summary>
		public IContextProperties GlobalProperties
		{
			get { return globalContextProperties; }
		}

		/// <summary>
		/// Exposes the Thread Context of the extended logger.
		/// </summary>
		public IContextProperties ThreadProperties
		{
			get { return threadContextProperties; }
		}

		/// <summary>
		/// Exposes the Thread Stack of the extended logger.
		/// </summary>
		public IContextStacks ThreadStacks
		{
			get { return threadContextStacks; }
		}

		#endregion
	}
}
