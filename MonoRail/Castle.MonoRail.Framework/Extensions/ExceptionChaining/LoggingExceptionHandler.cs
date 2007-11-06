// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Extensions.ExceptionChaining
{
	using Castle.Core.Logging;

	/// <summary>
	/// Handles that logs the exception using the the logger factory.
	/// </summary>
	public class LoggingExceptionHandler : AbstractExceptionHandler
	{
		/// <summary>
		/// Implementors should perform the action
		/// on the exception. Note that the exception
		/// is available in <see cref="IRailsEngineContext.LastException"/>
		/// </summary>
		/// <param name="context"></param>
		public override void Process(IRailsEngineContext context)
		{
			ILoggerFactory factory = (ILoggerFactory) context.GetService(typeof(ILoggerFactory));
			ILogger logger = factory.Create(context.CurrentController.GetType());

			logger.Error(BuildStandardMessage(context));
			InvokeNext(context);
		}
	}
}