// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System;

	public abstract class AbstractExceptionHandler : IExceptionHandler
	{
		private IExceptionHandler nextHandler;

		public virtual void Initialize()
		{
		}

		public abstract void Process(IRailsEngineContext context, IServiceProvider serviceProvider);

		public IExceptionHandler Next
		{
			get { return nextHandler; }
			set { nextHandler = value; }
		}

		protected void InvokeNext(IRailsEngineContext context, IServiceProvider serviceProvider)
		{
			if (nextHandler != null)
			{
				nextHandler.Process(context, serviceProvider);
			}
		}
	}
}
