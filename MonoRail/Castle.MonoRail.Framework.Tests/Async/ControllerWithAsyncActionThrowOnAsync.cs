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

namespace Castle.MonoRail.Framework.Tests.Async
{
	using System;

	public class ControllerWithAsyncActionThrowOnAsync : Controller
	{
		private readonly Output output;

		public ControllerWithAsyncActionThrowOnAsync()
		{
			output = ((Output) LongOp);
		}

		public delegate string Output();

		public IAsyncResult BeginIndex()
		{
			return output.BeginInvoke(delegate { }, null);
		}

		private string LongOp()
		{
			throw new InvalidOperationException("ba");
		}

		public void EndIndex()
		{
			string s = output.EndInvoke(ControllerContext.Async.Result);
			RenderText(s);
		}
	}
}