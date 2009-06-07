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
}