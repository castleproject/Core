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

namespace Castle.Facilities.WcfIntegration.Tests
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	[ServiceContract]
	public interface IOperations
	{
		[OperationContract]
		int GetValueFromConstructor();

		[OperationContract]
		int GetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue);

		[OperationContract]
		bool UnitOfWorkIsInitialized();
	}

	[ServiceContract(Name = "IOperations")]
	public interface IAsyncOperations
	{
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetValueFromConstructor(AsyncCallback callback, object asyncState);
		int EndGetValueFromConstructor(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetValueFromConstructorAsRefAndOut(ref int refValue, AsyncCallback callback, object asyncState);
		int EndGetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue, IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnitOfWorkIsInitialized(AsyncCallback callback, object asyncState);
		bool EndUnitOfWorkIsInitialized(IAsyncResult result);
	}

	[ServiceContract]
	public interface IOperationsEx : IOperations
	{
		[OperationContract]
		void Backup(IDictionary<string, object> context);

		[OperationContract]
		void ThrowException();
	}

	[ServiceContract(Name = "IOperations")]
	public interface IOperationsAll
	{
		[OperationContract]
		int GetValueFromConstructor();

		[OperationContract]
		int GetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue);

		[OperationContract]
		bool UnitOfWorkIsInitialized();

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetValueFromConstructor(AsyncCallback callback, object asyncState);
		int EndGetValueFromConstructor(IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetValueFromConstructorAsRefAndOut(ref int refValue, AsyncCallback callback, object asyncState);
		int EndGetValueFromConstructorAsRefAndOut(ref int refValue, out int outValue, IAsyncResult result);

		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUnitOfWorkIsInitialized(AsyncCallback callback, object asyncState);
		bool EndUnitOfWorkIsInitialized(IAsyncResult result);
	}

}