// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.Services.Logging.NLogIntegration
{
	using System;

	using Castle.Core.Logging;

	using NLog;

	using ExtendedLogger = Castle.Core.Logging.IExtendedLogger;

	/// <summary>
	///   Implementation of <see cref="IExtendedLogger" /> for NLog.
	/// </summary>
	public class ExtendedNLogLogger : NLogLogger, ExtendedLogger
	{
		private static readonly IContextProperties globalProperties = new GlobalContextProperties();
		private static readonly IContextProperties threadProperties = new ThreadContextProperties();
		private static readonly IContextStacks threadStacks = new ThreadContextStacks();

		/// <summary>
		///   Initializes a new instance of the <see cref="ExtendedNLogLogger" /> class.
		/// </summary>
		/// <param name="logger"> The logger. </param>
		/// <param name="factory"> The factory. </param>
		public ExtendedNLogLogger(Logger logger, ExtendedNLogFactory factory)
		{
			Logger = logger;
			Factory = factory;
		}

		/// <summary>
		///   Exposes the Global Context of the extended logger.
		/// </summary>
		public IContextProperties GlobalProperties
		{
			get { return globalProperties; }
		}

		/// <summary>
		///   Exposes the Thread Context of the extended logger.
		/// </summary>
		public IContextProperties ThreadProperties
		{
			get { return threadProperties; }
		}

		/// <summary>
		///   Exposes the Thread Stack of the extended logger.
		/// </summary>
		public IContextStacks ThreadStacks
		{
			get { return threadStacks; }
		}

		/// <summary>
		///   Gets or sets the factory.
		/// </summary>
		/// <value> The factory. </value>
		protected internal new ExtendedNLogFactory Factory { get; set; }

		/// <summary>
		///   Creates an extended child logger with the specified <paramref name="name" />
		/// </summary>
		/// <param name="name"> The name. </param>
		public ExtendedLogger CreateExtendedChildLogger(String name)
		{
			return Factory.Create(Logger.Name + "." + name);
		}

		/// <summary>
		///   Creates a child logger with the specified <paramref name="name" />.
		/// </summary>
		/// <param name="name"> The name. </param>
		public override Core.Logging.ILogger CreateChildLogger(String name)
		{
			return CreateExtendedChildLogger(name);
		}
	}
}