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

namespace Castle.Components.Scheduler.Tests.Utilities
{
	using System;
	using NUnit.Framework;

	/// <summary>
	/// Assertions for dates and times.
	/// </summary>
	public static class DateTimeAssert
	{
		/// <summary>
		/// Asserts that two dates are equal, including their <see cref="DateTime.Kind" /> properties.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		public static void AreEqualIncludingKind(DateTime expected, DateTime actual)
		{
			Assert.AreEqual(expected, actual);
			Assert.AreEqual(expected.Kind, actual.Kind);
		}

		/// <summary>
		/// Asserts that two nullable dates are equal, including their <see cref="DateTime.Kind" /> properties.
		/// </summary>
		/// <param name="expected"></param>
		/// <param name="actual"></param>
		public static void AreEqualIncludingKind(DateTime? expected, DateTime? actual)
		{
			Assert.AreEqual(expected, actual);

			if (expected.HasValue)
				Assert.AreEqual(expected.Value.Kind, actual.Value.Kind);
		}
	}
}