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

namespace TestSiteBrail.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[Layout("master")]
	public class AsyncController : SmartDispatcherController
	{
		#region Delegates

		public delegate string Output();

		#endregion

		private Output output;


		public IAsyncResult BeginWithParams(int id, string name)
		{
			PropertyBag["id"] = id;
			PropertyBag["name"] = name;
			return CallAsync();
		}

		public void EndWithParams()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}


		public IAsyncResult BeginIndex()
		{
			return CallAsync();
		}

		public void EndIndex()
		{
			string s = output.EndInvoke(ControllerContext.Async.Result);
			RenderText(s);
		}

		public IAsyncResult BeginWithView()
		{
			return CallAsync();
		}

		public void EndWithView()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}

		public IAsyncResult BeginWithViewAndLayout()
		{
			return CallAsync();
		}

		public void EndWithViewAndLayout()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}

		public IAsyncResult BeginErrorBegin()
		{
			throw new InvalidCastException("test error on begin");
		}

		public void EndErrorBegin()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}


		public IAsyncResult BeginErrorAsync()
		{
			return CallThrowingAsync();
		}

		public void EndErrorAsync()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}


		public IAsyncResult BeginErrorEnd()
		{
			return CallAsync();
		}

		public void EndErrorEnd()
		{
			throw new InvalidOperationException("test error from end");
		}

		public IAsyncResult BeginWithRescueAsync()
		{
			return CallThrowingAsync();
		}

		
		[Rescue("UpdateErrorMsg")]
		public void EndWithRescueAsync()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}

		[Rescue("UpdateErrorMsg")]
		public IAsyncResult BeginWithRescueBegin()
		{
			throw new InvalidCastException("test error on rescue");
		}

		public void EndWithRescueBegin()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}

		public IAsyncResult BeginWithRescueEnd()
		{
			return CallAsync();
		}

		[Rescue("UpdateErrorMsg")]
		public void EndWithRescueEnd()
		{
			throw new InvalidCastException("error from end");
		}

		public IAsyncResult BeginWithActionLayout()
		{
			return CallAsync();
		}

		[Layout("defaultlayout")]
		public void EndWithActionLayout()
		{
			PropertyBag["value"] = output.EndInvoke(ControllerContext.Async.Result);
		}

		[Rescue("UpdateErrorMsg")]
		[Layout("defaultlayout")]
		public IAsyncResult BeginRescueOnBeginActionLayout()
		{
			throw new InvalidOperationException("blah");
		}

		public void EndRescueOnBeginActionLayout()
		{
		}

		private IAsyncResult CallThrowingAsync()
		{
			output = Throw;
			return output.BeginInvoke(ControllerContext.Async.Callback, ControllerContext.Async.State);
		}

		private string Throw()
		{
			throw new InvalidOperationException("error from async");
		}

		private IAsyncResult CallAsync()
		{
			output = GetString;
			return output.BeginInvoke(
				ControllerContext.Async.Callback,
				ControllerContext.Async.State);
		}

		public string GetString()
		{
			return "value from async task";
		}
	}
}