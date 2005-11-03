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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;

	/// <summary>
	/// Simple helper for date formatting
	/// </summary>
	public class DateFormatHelper : AbstractHelper
	{
		/// <summary>
		/// Returns the difference from the 
		/// specified <c>date</c> the the current date
		/// in a friendly string like "1 day ago"
		/// <para>
		/// TODO: Think about i18n
		/// </para>
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public String FriendlyFormatFromNow(DateTime date)
		{
			TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan cur = new TimeSpan(date.Ticks);

			TimeSpan diff = cur.Subtract(now);

			if (diff.TotalMilliseconds == 0)
			{
				return "Just now";
			}

			if (diff.Days == 0)
			{
				if (diff.Hours == 0)
				{
					if (diff.Minutes == 0)
					{
						return String.Format("{0} second{1} ago", 
							diff.Seconds, diff.Seconds > 1 ? "s" : "");
					}
					else
					{
						return String.Format("{0} minute{1} ago", 
							diff.Minutes, diff.Minutes > 1 ? "s" : "");
					}
				}
				else
				{
					return String.Format("{0} hour{1} ago", 
						diff.Hours, diff.Hours > 1 ? "s" : "");
				}
			}
			else
			{
				return String.Format("{0} day{1} ago", 
					diff.Days, diff.Days > 1 ? "s" : "");
			}
		}

		/// <summary>
		/// Formats to short date
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public String FormatDate(DateTime date)
		{
			return date.ToShortDateString();
		}

		public String ToShortDateTime(DateTime date)
		{
			return date.ToShortDateString() + " " + date.ToShortTimeString();
		}
	}
}
