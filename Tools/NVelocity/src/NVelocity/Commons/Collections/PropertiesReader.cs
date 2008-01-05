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
	using System.IO;
	using System.Text;

	/// <summary>
	/// This class is used to read properties lines.  These lines do
	/// not terminate with new-line chars but rather when there is no
	/// backslash sign a the end of the line.  This is used to
	/// concatenate multiple lines for readability.
	/// </summary>
	internal class PropertiesReader : StreamReader
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader">A Reader.</param>
		public PropertiesReader(StreamReader reader) : base(reader.BaseStream)
		{
		}

		/// <summary>
		/// Read a property.
		/// </summary>
		/// <returns>A String.</returns>
		public String ReadProperty()
		{
			StringBuilder buffer = new StringBuilder();


			while(true)
			{
				String line = ReadLine();

				if (line == null)
				{
					return null;
				}

				line = line.Trim();

				if ((line.Length != 0) && (line[0] != '#'))
				{
					if (line.EndsWith(@"\"))
					{
						line = line.Substring(0, (line.Length - 1) - (0));
						buffer.Append(line);
					}
					else
					{
						buffer.Append(line);
						break;
					}
				}
			}


			return buffer.ToString();
		}
	}
}