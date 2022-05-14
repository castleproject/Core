// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class ExplicitlyImplementedPropertyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Can_proxy_type_having_two_identically_named_properties_with_different_signatures()
		{
			_ = generator.CreateInterfaceProxyWithoutTarget<IDerived>();
		}

		public interface IBase
		{
			Action Property { get; }
		}

		public interface IDerived : IBase
		{
			new Action<bool> Property { get; }
		}
	}
}
