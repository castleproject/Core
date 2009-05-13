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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class NullablesTestCase
	{
		private bool convSucceed;
		
		[TestFixtureSetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;
		}

		[Test]
		public void NullableIntConversion()
		{
			Assert.AreEqual(new int?(10), Convert(typeof(int?), "10"));
			Assert.IsTrue(convSucceed);

			int? val = (int?) Convert(typeof(int?), "");
			Assert.IsFalse(val.HasValue);
			Assert.IsTrue(convSucceed);
		}

		private object Convert(Type desiredType, string input)
		{
			return new DefaultConverter().Convert(desiredType, input, out convSucceed);
		}
	}
}
