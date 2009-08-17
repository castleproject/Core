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

namespace Commons.Collections
{
	using System;
	using System.Text;

	public class StringTokenizer
	{
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
		private const string DefaultDelimiters = " \t\n\r";

		private readonly string source;
		private string delimiters;
		private int currentPosition;
		private readonly int length;

		public StringTokenizer(string source) : this(source, DefaultDelimiters)
		{
		}

		public StringTokenizer(string source, string delimiters)
		{
			this.delimiters = delimiters;
			this.source = source;
			this.length = this.source.Length;
		}

		public int Count
		{
			get
			{
				if (!HasMoreTokens())
				{
					return 0;
				}

				int savedPosition = currentPosition;
				int count = 0;
				while (HasMoreTokens())
				{
					NextToken();
					count++;
				}
				currentPosition = savedPosition;
				return count;
			}
		}

		public virtual bool HasMoreTokens()
		{
			return (currentPosition < length);
		}

		public virtual string NextToken()
		{
			if (currentPosition >= length)
			{
				throw new Exception();
			}

			StringBuilder result = new StringBuilder();
			// Go through the current string, adding characters to the result until a delimiter is reached.
			while (currentPosition < length && delimiters.IndexOf(source.Substring(currentPosition, 1)) < 0)
			{
				result.Append(source.Substring(currentPosition, 1));
				currentPosition++;
			}

			// Now skip over the delimiters.
			while (currentPosition < length && delimiters.IndexOf(source.Substring(currentPosition, 1)) >= 0)
			{
				currentPosition++;
			}
			return result.ToString();
		}

		public string NextToken(string delimiters)
		{
			this.delimiters = delimiters;
			return NextToken();
		}
	}
}