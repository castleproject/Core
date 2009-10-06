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
// 
namespace Castle.Components.Binder.Tests
{
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Threading;
	using Models;
	using NUnit.Framework;
	using Validator;

	[TestFixture]
	public class DataBinderWithValidatorTestCase
	{
		private IDataBinder binder;
		private TreeBuilder builder;

		[TestFixtureSetUp]
		public void Init()
		{
			binder = new DataBinder();
			builder = new TreeBuilder();
			binder.Validator = new ValidatorRunner(new CachedValidationRegistry());

			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		[Test]
		public void ValidateInheritedNestedNonEmpty()
		{
			var args = new NameValueCollection
			           	{
			           		{"customer.Email", "test@test.com"},
			           		{"customer.ConfirmEmail", "test@test.com"},
			           		{"customer.Address.Street", string.Empty}
			           	};

			object instance = binder.BindObject(typeof (Customer), "customer", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void ValidateInheritedNotSameAs()
		{
			var args = new NameValueCollection {{"customer.Email", "test1@test.com"}, {"customer.ConfirmEmail", "test@test.com"}};

			object instance = binder.BindObject(typeof (Customer), "customer", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void ValidateNotSameAs()
		{
			var args = new NameValueCollection {{"person.Email", "test1@test.com"}, {"person.ConfirmEmail", "test@test.com"}};

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.AreEqual(1, binder.ErrorList.Count);
		}

		[Test]
		public void ValidateNotSameAsWithNonEmpty()
		{
			var args = new NameValueCollection {{"person.Email", string.Empty}, {"person.ConfirmEmail", "test@test.com"}};

			object instance = binder.BindObject(typeof (Person), "person", builder.BuildSourceNode(args));

			Assert.IsNotNull(instance);
			Assert.AreEqual(2, binder.ErrorList.Count);
		}
	}
}