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
using System.Threading;

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System;
	using System.Collections.Generic;
	using Castle.Facilities.WcfIntegration.Tests.Behaviors;

	public class Operations : IOperations, IOperationsEx
	{
		private readonly int number;

		public Operations(int number)
		{
			this.number = number;
		}

		#region IOperations Members

		public int GetValueFromConstructor()
		{
			return number;
		}

		public int GetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue)
		{
			Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
			Thread.Sleep(2000);
			return (refValue = outValue = number);
		}
        
		public bool UnitOfWorkIsInitialized()
		{
			return UnitOfWork.initialized;
		}

		#endregion

		#region IOperationsEx Members

		public void Backup(IDictionary<string, object> context)
		{
		}

		public void ThrowException()
		{
			throw new InvalidOperationException("Oh No!");
		}
		#endregion
	}

	delegate int GetValueFromConstructor();
	delegate int GetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue);
	delegate bool UnitOfWorkIsInitialized();
 
	public class AsyncOperations : IAsyncOperations
	{
		private readonly Operations operations;
		private GetValueFromConstructor getValueCtor;
		private GetValueFromConstructorAsRefAndOut getValueCtorRefOut;
		private UnitOfWorkIsInitialized uow;

		public AsyncOperations(int number)
		{
			operations = new Operations(number);
			getValueCtor = operations.GetValueFromConstructor;
			getValueCtorRefOut = operations.GetValueFromConstructorAsRefAndOut;
			uow = operations.UnitOfWorkIsInitialized;
		}

		public IAsyncResult BeginGetValueFromConstructor(AsyncCallback callback, object asyncState)
		{
			return getValueCtor.BeginInvoke(callback, asyncState);
		}

		public int EndGetValueFromConstructor(IAsyncResult result)
		{
			return getValueCtor.EndInvoke(result);
		}
        
		public IAsyncResult BeginGetValueFromConstructorAsRefAndOut(
			ref int refValue, AsyncCallback callback, object asyncState)
		{
			int outValue;
			return getValueCtorRefOut.BeginInvoke(ref refValue, out outValue, callback, asyncState);
		}

		public int EndGetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue, IAsyncResult result)
		{
			return getValueCtorRefOut.EndInvoke(ref refValue, out outValue, result);
		}

		public IAsyncResult BeginUnitOfWorkIsInitialized(AsyncCallback callback, object asyncState)
		{
			return uow.BeginInvoke(callback, asyncState);			
		}

		public bool EndUnitOfWorkIsInitialized(IAsyncResult result)
		{
			return uow.EndInvoke(result);
		}
	}
}