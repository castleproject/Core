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

namespace Castle.Components.Scheduler.Utilities
{
	using System;

	/// <summary>
	/// Provides utility functions for manipulating dates and times.
	/// </summary>
	public static class DateTimeUtils
	{
		/// <summary>
		/// Converts a nullable date/time value to UTC.
		/// </summary>
		/// <param name="dateTime">The nullable date/time</param>
		/// <returns>The nullable date/time in UTC</returns>
		public static DateTime? ToUniversalTime(DateTime? dateTime)
		{
			return dateTime.HasValue ? dateTime.Value.ToUniversalTime() : (DateTime?) null;
		}

		/// <summary>
		/// Returns a copy of a date/time value with its kind
		/// set to <see cref="DateTimeKind.Utc" /> but does not perform
		/// any time-zone adjustment.
		/// </summary>
		/// <remarks>
		/// This method is useful when obtaining date/time values from sources
		/// that might not correctly set the UTC flag.
		/// </remarks>
		/// <param name="dateTime">The date/time</param>
		/// <returns>The same date/time with the UTC flag set</returns>
		public static DateTime AssumeUniversalTime(DateTime dateTime)
		{
			return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
		}

		/// <summary>
		/// Returns a copy of a nullable date/time value with its kind
		/// set to <see cref="DateTimeKind.Utc" /> but does not perform
		/// any time-zone adjustment.
		/// </summary>
		/// <remarks>
		/// This method is useful when obtaining date/time values from sources
		/// that might not correctly set the UTC flag.
		/// </remarks>
		/// <param name="dateTime">The nullable date/time</param>
		/// <returns>The same nullable date/time with the UTC flag set</returns>
		public static DateTime? AssumeUniversalTime(DateTime? dateTime)
		{
			return dateTime.HasValue ? AssumeUniversalTime(dateTime.Value) : (DateTime?) null;
		}
	}
}