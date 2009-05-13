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

namespace Castle.Facilities.AutomaticTransactionManagement.Tests
{
	using System;
	using Castle.MicroKernel;
	using Castle.Services.Transaction;
	using NUnit.Framework;

	/// <summary>
	/// Summary description for CustomerService.
	/// </summary>
	[Transactional]
	public class CustomerService
	{
		private readonly IKernel kernel;

		public CustomerService(IKernel kernel)
		{
			this.kernel = kernel;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void Insert( String name, String address )
		{
			
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void Delete( int id )
		{
			throw new ApplicationException("Whopps. Problems!");
		}

		[Transaction]
		public virtual void Update(int id)
		{
			ITransactionManager tm = (ITransactionManager) kernel[typeof(ITransactionManager)];

			Assert.IsNotNull(tm.CurrentTransaction);
			Assert.AreEqual(TransactionStatus.Active, tm.CurrentTransaction.Status);
			
			tm.CurrentTransaction.SetRollbackOnly();
			
			Assert.AreEqual(TransactionStatus.Active, tm.CurrentTransaction.Status);
		}
	}
}
