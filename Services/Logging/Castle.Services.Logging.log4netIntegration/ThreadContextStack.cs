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

	public class ThreadContextStack : IContextStack
	{
		private log4net.Util.ThreadContextStack log4netStack;
		public ThreadContextStack(log4net.Util.ThreadContextStack log4netStack)
		{
			this.log4netStack = log4netStack;
		}

		#region IContextStack Members

		public int Count
		{
			get { return log4netStack.Count; }
		}

		public void Clear()
		{
			log4netStack.Clear();
		}

		public string Pop()
		{
			return log4netStack.Pop();
		}

		public IDisposable Push(string message)
		{
			return log4netStack.Push(message);
		}

		#endregion
	}
}
