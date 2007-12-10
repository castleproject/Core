// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	public class StringTokenizer
	{
		private List<string> elements;
		private string source;
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character
		private string delimiters = " \t\n\r";

		public StringTokenizer(string source)
		{
			elements = new List<string>();
			elements.AddRange(source.Split(delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public StringTokenizer(string source, string delimiters)
		{
			elements = new List<string>();
			this.delimiters = delimiters;
			elements.AddRange(source.Split(this.delimiters.ToCharArray()));
			RemoveEmptyStrings();
			this.source = source;
		}

		public int Count
		{
			get { return (elements.Count); }
		}

		public virtual bool HasMoreTokens()
		{
			return (elements.Count > 0);
		}

		public virtual string NextToken()
		{
			if (source == string.Empty)
			{
				throw new Exception();
			}
			else
			{
				elements = new List<string>();
				elements.AddRange(source.Split(delimiters.ToCharArray()));
				RemoveEmptyStrings();
				string result = elements[0];
				elements.RemoveAt(0);
				source = source.Replace(result, string.Empty);
				source = source.TrimStart(delimiters.ToCharArray());
				return result;
			}
		}

		public string NextToken(string delimiters)
		{
			this.delimiters = delimiters;
			return NextToken();
		}

		private void RemoveEmptyStrings()
		{
			//VJ++ does not treat empty strings as tokens
			for(int index = 0; index < elements.Count; index++)
				if (elements[index] == string.Empty)
				{
					elements.RemoveAt(index);
					index--;
				}
		}
	}
}