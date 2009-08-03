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

namespace Castle.Facilities.WcfIntegration.Tests.Components
{
	using System.ServiceModel;

	[ServiceContract]
	public interface ICallbackService
	{
		[OperationContract(IsOneWay = true)]
		void Callback(int value);
	}

	public class CallbackService : ICallbackService
	{
		public void Callback(int valueFromTheOtherSide)
		{
			this.valueFromTheOtherSide = valueFromTheOtherSide;
		}

		int valueFromTheOtherSide;
		
		public int ValueFromTheOtherSide 
		{
			get { return valueFromTheOtherSide; }
			set { valueFromTheOtherSide = value; }
		}
	}

	[ServiceContract(CallbackContract = typeof(ICallbackService))]
	public interface IServiceWithCallback
	{
		[OperationContract]
		void DoSomething(int value);

		[OperationContract]
		void DoSomethingElse(int value);
	}

	public class ServiceWithCallback : IServiceWithCallback
	{
		public void DoSomething(int value)
		{
			ICallbackService callbackService = OperationContext.Current.GetCallbackChannel<ICallbackService>();
			callbackService.Callback(value * 2);
		}

		public void DoSomethingElse(int value)
		{
			ICallbackService callbackService = OperationContext.Current.GetCallbackChannel<ICallbackService>();
			callbackService.BeginWcfCall(p => p.Callback(value * 4));
		}
	}
}
