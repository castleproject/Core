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

using System;

namespace Castle.MonoRail.Views.Brail
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Castle.MonoRail.Framework;

	public class BrailPreProcessor : AbstractCompilerStep
	{
		private static IDictionary Seperators = CreateSeperators();
        private BooViewEngine booViewEngine;
		IDictionary inputToCode = new Hashtable();

	    public BrailPreProcessor(BooViewEngine booViewEngine)
	    {
	        this.booViewEngine = booViewEngine;
	    }

	    private static IDictionary CreateSeperators()
		{
			Hashtable seperators = new Hashtable();
			seperators.Add("<?brail", "?>");
			seperators.Add("<%", "%>");
			return seperators;
		}

		public string GetInputCode(ICompilerInput input)
		{
			return (string)inputToCode[input];
		}
		
		public override void Run()
		{
			ArrayList processed = new ArrayList();
			foreach(ICompilerInput input in Parameters.Input)
			{
				//if input.Name.Contains("empty"):
				//	System.Diagnostics.Debugger.Break()
				using(TextReader reader = input.Open())
				{
				    string code = reader.ReadToEnd();
                    if(this.booViewEngine.ConditionalPreProcessingOnly(input.Name) == false || 
						ShouldPreProcess(code))
                        code = Booify(code);
					StringInput newInput = new StringInput(input.Name, code);
					inputToCode.Add(input, code);
					processed.Add(newInput);
				}
			}
			Parameters.Input.Clear();
			foreach(StringInput input in processed)
			{
				Parameters.Input.Add(input);
			}
		}

		private bool ShouldPreProcess(string code)
		{
			foreach (DictionaryEntry entry in Seperators)
			{
				if(code.Contains(entry.Key.ToString()))
					return true;
			}
			return false;
		}

		public static string Booify(string code)
		{
			if (code.Length == 0)
			{
				return "output string.Empty\r\n";
			}
			StringWriter buffer = new StringWriter();
			int index = 0;
			int lastIndex = 0;
			string start, end;
			DictionaryEntry seperators = GetSeperators(code);
			start = seperators.Key.ToString();
			end = seperators.Value.ToString();

			while(index != -1)
			{
				index = code.IndexOf(start, lastIndex);
				if (index == -1)
					break;
                Output(buffer, code.Substring(lastIndex, index - lastIndex));
				int startReading = index + start.Length;
				lastIndex = code.IndexOf(end, startReading);
				if (lastIndex == -1)
					throw new RailsException("expected " + end);
				buffer.Write(code.Substring(startReading, lastIndex - startReading));
				lastIndex += end.Length;
			}
			Output(buffer, code.Substring(lastIndex));
			return buffer.ToString();
		}

		private static void Output(StringWriter buffer, string code)
		{
			if (code.Length == 0)
				return;
			code = RemoveDoubleQuotesFromExpressions(code);
			buffer.WriteLine();
			buffer.Write("output \"\"\"");
			buffer.Write(code);
			buffer.WriteLine("\"\"\"");
		}

		/// <summary>
		/// This will replace any " inside a ${ } expressions with a ', because it breaks the parser
		/// otherwise.
		/// This is a very stupid scanner, but it is replacing a regex that had performance issues.
		/// </summary>
		/// <param name="code"></param>
		private static string RemoveDoubleQuotesFromExpressions(string code)
		{
			Stack<BrachMatchingInfo> bracesPositions = new Stack<BrachMatchingInfo>();
			bool prevCharWasDollary = false;
			for (int index = 0; index < code.Length;index++ )
			{
				if (code[index] == '{')
				{
					bracesPositions.Push(new BrachMatchingInfo(index, -1, prevCharWasDollary));
				}
				// Note that here there is an implicit check for ${   }   }
				// it will match the last }
				if (code[index] == '}' && bracesPositions.Count > 0)
				{
					bracesPositions.Peek().End = index;
				}
				prevCharWasDollary = code[index] == '$';
			}
			if(bracesPositions.Count==0)
				return code;
			StringBuilder sb = new StringBuilder(code);
			foreach(BrachMatchingInfo matchingInfo in bracesPositions)
			{
				//probably a malf-formed expression, or not part of an ${ }, we will ignore that and let
				// the parser shout at the user
				if (matchingInfo.End== -1 || matchingInfo.PrevCharWasDollar==false)
					continue;
				sb.Replace('"', '\'', matchingInfo.Start, matchingInfo.End - matchingInfo.Start);
			}
			return sb.ToString();
		}

		private class BrachMatchingInfo
		{
			private int start, end;

			public int Start
			{
				get { return start; }
				set { start = value; }
			}

			public int End
			{
				get { return end; }
				set { end = value; }
			}

			public bool PrevCharWasDollar
			{
				get { return prevCharWasDollar; }
				set { prevCharWasDollar = value; }
			}

			public BrachMatchingInfo(int start, int end, bool prevCharWasDollar)
			{
				this.start = start;
				this.end = end;
				this.prevCharWasDollar = prevCharWasDollar;
			}

			private bool prevCharWasDollar;
		}

		private static DictionaryEntry GetSeperators(string code)
		{
			string start = null, end = null;
			foreach(DictionaryEntry entry in Seperators)
			{
				if (code.IndexOf(entry.Key as string, 0) != -1)
				{
					if (start != null && code.IndexOf(entry.Key as string) != -1)
						continue; //handle a shorthanded seperator.
					// handle long seperator
					if (start != null &&  entry.Key.ToString().IndexOf(start as string) == -1)
					{
						throw new RailsException("Can't mix seperators in one file. Found both " + start + " and " + entry.Key);
					}
					start = entry.Key.ToString();
					end = entry.Value.ToString();
				}
			}

			if (start == null) //default, doesn't really matter, since it won't be used.
			{
				foreach(DictionaryEntry entry in Seperators)
				{
					return entry;
				}
			}
			return new DictionaryEntry(start, end);
		}
	}
}