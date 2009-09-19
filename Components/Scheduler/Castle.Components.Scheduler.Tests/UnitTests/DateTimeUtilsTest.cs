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

namespace Castle.Components.Scheduler.Tests.UnitTests
{
	using System;
	using NUnit.Framework;
	using Scheduler.Utilities;
	using Utilities;

	[TestFixture]
	public class DateTimeUtilsTest
	{
		[Test]
		public void ToUniversalTime_Null()
		{
			Assert.IsNull(DateTimeUtils.ToUniversalTime(null));
		}

		[Test]
		public void ToUniversalTime_NonNull()
		{
			DateTime value = new DateTime(2000, 3, 4);
			DateTimeAssert.AreEqualIncludingKind(value.ToUniversalTime(), DateTimeUtils.ToUniversalTime(value));
		}

		[Test]
		public void AssumeUniversalTime_NotNullable()
		{
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc),
			                                     DateTimeUtils.AssumeUniversalTime(new DateTime(2000, 3, 4)));
		}

		[Test]
		public void AssumeUniversalTime_NonNull()
		{
			DateTimeAssert.AreEqualIncludingKind(new DateTime(2000, 3, 4, 0, 0, 0, DateTimeKind.Utc),
			                                     DateTimeUtils.AssumeUniversalTime((DateTime?) new DateTime(2000, 3, 4)));
		}

		[Test]
		public void AssumeUniversalTime_Null()
		{
			Assert.IsNull(DateTimeUtils.AssumeUniversalTime(null));
		}
	}
}