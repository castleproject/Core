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

namespace Castle.Services.Logging.NLogtIntegration
{
	using System;
	using Castle.Core.Logging;

	public class ThreadContextStack : IContextStack
	{

		#region IContextStack Members

		public int Count
		{
			get { throw new NotImplementedException("NLog does not implement a Count of it's stack."); }
		}

		public void Clear()
		{
			NLog.NDC.Clear();
		}

		public string Pop()
		{
			return NLog.NDC.Pop();
		}

		public IDisposable Push(string message)
		{
			return NLog.NDC.Push(message);
		}

		#endregion
	}
}
