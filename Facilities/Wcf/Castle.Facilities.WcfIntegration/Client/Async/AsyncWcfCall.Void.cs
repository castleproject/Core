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

namespace Castle.Facilities.WcfIntegration.Async
{
	using System;

	public class AsyncWcfCall<TProxy> : AsyncWcfCallBase<TProxy>, IWcfAsyncCall
	{
		public AsyncWcfCall(TProxy proxy, Action<TProxy> func)
			: base(proxy, func)
		{
		}

		public void End()
		{
			InternalEnd();
			CreateUnusedOutArgs(0);
		}

		public void End<TOut1>(out TOut1 out1)
		{
			InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			CreateUnusedOutArgs(1);
		}

		public void End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			CreateUnusedOutArgs(2);
		}

		public void End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			CreateUnusedOutArgs(3);
		}

		public void End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4)
		{
			InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			out4 = (TOut4)ExtractOutOfType(typeof(TOut4), 3);
			CreateUnusedOutArgs(4);
		}

		private void InternalEnd()
		{
			End((i, c) => i.EndCall(c, out outArgs));
		}

		protected override object GetDefaultReturnValue()
		{
			return null;
		}
	}
}