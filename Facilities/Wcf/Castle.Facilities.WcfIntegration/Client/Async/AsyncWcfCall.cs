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

	public class AsyncWcfCall<TProxy, TResult> : AsyncWcfCallBase<TProxy>, IWcfAsyncCall<TResult>, IWcfAsyncCall
	{
		public AsyncWcfCall(TProxy proxy, Func<TProxy, TResult> func)
			: base(proxy, p => func(p))
		{
		}

		public TResult End()
		{
			TResult result = InternalEnd();
			CreateUnusedOutArgs(0);
			return result;
		}

		public TResult End<TOut1>(out TOut1 out1)
		{
			TResult result = InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			CreateUnusedOutArgs(1);
			return result;
		}

		public TResult End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			TResult result = InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			CreateUnusedOutArgs(2);
			return result;
		}

		public TResult End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			TResult result = InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			CreateUnusedOutArgs(3);
			return result;
		}

		public TResult End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4)
		{
			TResult result = InternalEnd();
			out1 = (TOut1)ExtractOutOfType(typeof(TOut1), 0);
			out2 = (TOut2)ExtractOutOfType(typeof(TOut2), 1);
			out3 = (TOut3)ExtractOutOfType(typeof(TOut3), 2);
			out4 = (TOut4)ExtractOutOfType(typeof(TOut4), 3);
			CreateUnusedOutArgs(4);
			return result;
		}

		#region IWcfAsyncCall Members

		void IWcfAsyncCall.End()
		{
			End();
		}

		void IWcfAsyncCall.End<TOut1>(out TOut1 out1)
		{
			End(out out1);
		}

		void IWcfAsyncCall.End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2)
		{
			End(out out1, out out2);
		}

		void IWcfAsyncCall.End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3)
		{
			End(out out1, out out2, out out3);
		}

		void IWcfAsyncCall.End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4)
		{
			End(out out1, out out2, out out3, out out4);
		}

		#endregion

		private TResult InternalEnd()
		{
			TResult result = default(TResult);
			End((i, c) => { result = i.EndCall<TResult>(c, out outArgs); });
			return result;
		}

		protected override object GetDefaultReturnValue()
		{
			return default(TResult);
		}
	}
}