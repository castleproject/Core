// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator.Tests
{
	using System.Globalization;
	using System.Threading;
	using Castle.Components.Validator.Tests.Models;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorsExecutionOrderTestCase
	{
		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
		}

		[Test]
		public void ValidationOrderTest()
		{
			ValidatorRunner runner = new ValidatorRunner(new CachedValidationRegistry());

			Supplier supplier = new Supplier();
			supplier.Password = "123";

			runner.IsValid(supplier);

			ErrorSummary summary = runner.GetErrorSummary(supplier);
			Assert.IsNotNull(summary);

			Assert.AreEqual("This is a required field", summary.ErrorMessages[0]);
			Assert.AreEqual("Fields do not match", summary.ErrorMessages[1]);
			Assert.AreEqual("This is a required field", summary.ErrorMessages[2]);
		}
	}
}