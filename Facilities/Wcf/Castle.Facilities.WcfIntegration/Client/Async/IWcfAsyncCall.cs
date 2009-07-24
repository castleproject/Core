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
// limitations under the License

namespace Castle.Facilities.WcfIntegration
{
	using System;

	/// <summary>
	/// Exposes output parameters.
	/// </summary>
	public interface IWcfAsyncBindings : IAsyncResult
	{
		object[] OutArgs { get; }
		object[] UnboundOutArgs { get; }

		bool UseSynchronizationContext { get; set; }

		TOut GetOutArg<TOut>(int index);
		TOut GetUnboundOutArg<TOut>(int index);
	}

	/// <summary>
	/// Represents an asynchronous call with a return value.
	/// </summary>
	/// <typeparam name="TReturn"></typeparam>
	public interface IWcfAsyncCall<TReturn> : IWcfAsyncBindings
	{
		TReturn End();
		TReturn End<TOut1>(out TOut1 out1);
		TReturn End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2);
		TReturn End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3);
		TReturn End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4);
	}

	/// <summary>
	/// Represents an asynchronous call without a return value.
	/// </summary>
	public interface IWcfAsyncCall : IWcfAsyncBindings
	{
		void End();
		void End<TOut1>(out TOut1 out1);
		void End<TOut1, TOut2>(out TOut1 out1, out TOut2 out2);
		void End<TOut1, TOut2, TOut3>(out TOut1 out1, out TOut2 out2, out TOut3 out3);
		void End<TOut1, TOut2, TOut3, TOut4>(out TOut1 out1, out TOut2 out2, out TOut3 out3, out TOut4 out4);
	}
}