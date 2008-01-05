// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Commons.Collections
{
	using System;
	using System.Text;

	/// <summary>
	/// This class divides into tokens a property value.  Token
	/// separator is "," but commas into the property value are escaped
	/// using the backslash in front.
	/// </summary>
	internal class PropertiesTokenizer : StringTokenizer
	{
		/// <summary>
		/// The property delimiter used while parsing (a comma).
		/// </summary>
		internal const String DELIMITER = ",";

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="str">A String</param>
		public PropertiesTokenizer(String str) : base(str, DELIMITER)
		{
		}


		/// <summary>
		/// Get next token.
		/// </summary>
		/// <returns>A String</returns>
		public override String NextToken()
		{
			StringBuilder buffer = new StringBuilder();

			while(HasMoreTokens())
			{
				String token = base.NextToken();
				if (token.EndsWith(@"\"))
				{
					buffer.Append(token.Substring(0, (token.Length - 1) - (0)));
					buffer.Append(DELIMITER);
				}
				else
				{
					buffer.Append(token);
					break;
				}
			}

			return buffer.ToString().Trim();
		}
	}
}