// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.AutomaticTransactionManagement.Tests
{
	using System;

	using Castle.Services.Transaction;

	/// <summary>
	/// Summary description for CustomerService.
	/// </summary>
	[Transactional]
	public class CustomerService
	{
		[Transaction(TransactionMode.Requires)]
		public virtual void Insert( String name, String address )
		{
			
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void Delete( int id )
		{
			throw new ApplicationException("Whopps. Problems!");
		}
	}
}
