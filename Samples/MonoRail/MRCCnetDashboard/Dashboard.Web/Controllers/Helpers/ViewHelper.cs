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

namespace Dashboard.Web.Controllers
{
	using System;

	public class ViewHelper
	{
		/// <summary>
		/// Converts a name like <c>log20051102223414Lbuild.9.xml </c>
		/// to 
		/// </summary>
		public String BuildName(String logName)
		{
			String[] pieces = logName.Split('.');

			DateTime dt = ObtainDateTime(pieces[0]);

			return String.Format("Build {2} - {0} {1}", dt.ToShortDateString(), dt.ToShortTimeString(), pieces[1]);
		}

		/// <summary>
		/// Extracts the date from a string like 
		/// <c>log20051102223414Lbuild</c>
		/// </summary>
		private DateTime ObtainDateTime(string dateTime)
		{
			dateTime = dateTime.Substring(3);

			int year = Convert.ToInt32( dateTime.Substring(0, 4) );
			int month = Convert.ToInt32( dateTime.Substring(4, 2) );
			int day = Convert.ToInt32( dateTime.Substring(6, 2) );
			int hour = Convert.ToInt32( dateTime.Substring(8, 2) );
			int min = Convert.ToInt32( dateTime.Substring(10, 2) );
			int sec = Convert.ToInt32( dateTime.Substring(12, 2) );

			return new DateTime(year, month, day, hour, min, sec);
		}
	}
}
