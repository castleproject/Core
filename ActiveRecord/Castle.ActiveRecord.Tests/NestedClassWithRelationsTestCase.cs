// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests
{
	using System;
	
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.Nested;
	
	
	[TestFixture]
	public class NestedClassWithRelationsTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void Operations()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), 
				typeof(Operation), typeof(PaymentPlan),
				typeof(Convention), typeof(Payment));
			Recreate();

			Operation.DeleteAll();
			
			Operation operation = new Operation();
			operation.Save();
			
			operation = Operation.Find(operation.Id);
			Assert.AreEqual(1, operation.Id);
			Assert.IsNotNull(operation.PaymentPlan);
			
			operation.PaymentPlan.ExpirationDate = DateTime.Today.AddDays(-10);
			operation.PaymentPlan.NumberOfPayments = 12;
			operation.Save();
			
			Convention convention = new Convention();
			convention.Description = "It works!";
			convention.Save();
			
			operation.PaymentPlan.Convention = convention;
			operation.Save();
			
			operation = Operation.Find(operation.Id);
			Assert.IsNotNull(operation.PaymentPlan.Convention);
			Assert.AreEqual(convention.Description, operation.PaymentPlan.Convention.Description);
			
			Payment payment1 = new Payment();
			payment1.Expiration  =DateTime.Today.AddDays(10);
			payment1.Amount = 10;
			payment1.Save();
			
			operation.PaymentPlan.Payments.Add(payment1);
			operation.Save();
			
			int id = operation.Id;
			operation = null;
			operation = Operation.Find(id);
			
			Assert.AreEqual(1, operation.PaymentPlan.Payments.Count);
		}
	}
}
