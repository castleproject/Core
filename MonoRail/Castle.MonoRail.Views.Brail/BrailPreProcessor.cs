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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Castle.MonoRail.Framework;

	public class BrailPreProcessor : AbstractCompilerStep
	{
		public const string ClosingQuoteReplacement = "`^`";
		public const string DoubleQuote = "\"";
		private static IDictionary Seperators = CreateSeperators();
		private BooViewEngine booViewEngine;
		private IDictionary inputToCode = new Hashtable();

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
			foreach (ICompilerInput input in Parameters.Input)
			{
				//if input.Name.Contains("empty"):
				//	System.Diagnostics.Debugger.Break()
				using (TextReader reader = input.Open())
				{
					string code = reader.ReadToEnd();
					if (this.booViewEngine.ConditionalPreProcessingOnly(input.Name) == false ||
						ShouldPreProcess(code))
						code = Booify(code);
					StringInput newInput = new StringInput(input.Name, code);
					inputToCode.Add(input, code);
					processed.Add(newInput);
				}
			}
			Parameters.Input.Clear();
			foreach (StringInput input in processed)
			{
				Parameters.Input.Add(input);
			}
		}

		private bool ShouldPreProcess(string code)
		{
			foreach (DictionaryEntry entry in Seperators)
			{
				if (code.Contains(entry.Key.ToString()))
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

			while (index != -1)
			{
				index = code.IndexOf(start, lastIndex);
				if (index == -1)
					break;
				Output(buffer, code.Substring(lastIndex, index - lastIndex));
				int startReading = index + start.Length;
				lastIndex = code.IndexOf(end, startReading);
				if (lastIndex == -1)
					throw new RailsException("expected " + end);
				buffer.WriteLine(code.Substring(startReading, lastIndex - startReading));
				lastIndex += end.Length;
			}
			Output(buffer, code.Substring(lastIndex));
			return buffer.ToString();
		}

		private static void Output(StringWriter buffer, string code)
		{
			if (code.Length == 0)
				return;
			code = EscapeInitialAndClosingDoubleQuotes(code);
			IList<ExpressionPosition> expressions = GetExpressionsPositions(code);
			if (expressions.Count == 0)
			{
				OutputText(buffer, code);
				return;
			}

			int start = 0;
			foreach (ExpressionPosition position in expressions)
			{
				string text = code.Substring(start, position.Start - start);
				OutputText(buffer, text);
				string expression = code.Substring(position.Start + 2, position.End - (position.Start + 2));
				OutputExpression(buffer, expression);
				start = position.End + 1;
			}
			string remainingText = code.Substring(start, code.Length - start);
			OutputText(buffer, remainingText);
		}

		private static void OutputText(StringWriter buffer, string code)
		{
			buffer.Write("output \"\"\"");
			buffer.Write(code);
			buffer.WriteLine("\"\"\"");
		}

		private static void OutputExpression(StringWriter buffer, string code)
		{
			buffer.Write("output ");
			buffer.WriteLine(code);
		}

		private static string EscapeInitialAndClosingDoubleQuotes(string code)
		{
			if (code.StartsWith(DoubleQuote))
				code = ClosingQuoteReplacement + code.Substring(DoubleQuote.Length);
			if (code.EndsWith(DoubleQuote))
				code = code.Substring(0, code.Length - DoubleQuote.Length) + ClosingQuoteReplacement;
			return code;
		}

		/// <summary>
		/// Will find all the (outer most ${} expressions in the code, and return their positions).
		/// Smart enough to figure out $${} escaping, but not much more
		/// </summary>
		private static IList<ExpressionPosition> GetExpressionsPositions(string code)
		{
			List<ExpressionPosition> bracesPositions = new List<ExpressionPosition>();
			bool prevCharWasDollar = false;
			for (int index = 0; index < code.Length; index++)
			{
				if (code[index] == '{')
				{
					bracesPositions.Add(new ExpressionPosition(index - 1, -1, prevCharWasDollar));
				}
				if (code[index] == '}' && bracesPositions.Count > 0)
				{
					if (ParentExpressionIsNotValid(bracesPositions))
					{
						bracesPositions.RemoveAt(bracesPositions.Count - 1);
					}
					else
					{
						bracesPositions[bracesPositions.Count - 1].End = index;
					}
				}
				//handles escaping expressions with $$ as well
				prevCharWasDollar = code[index] == '$' && !prevCharWasDollar;
			}
			return bracesPositions;
		}

		private static bool ParentExpressionIsNotValid(List<ExpressionPosition> bracesPositions)
		{
			if(bracesPositions.Count == 1)
				return false;
			ExpressionPosition parentExpression = bracesPositions[bracesPositions.Count - 2];
			return parentExpression.End == -1 || parentExpression.PrevCharWasDollar;
		}

		private class ExpressionPosition
		{
			private int start, end;
			private readonly bool prevCharWasDollar;

			public int Start
			{
				get { return start; }
			}

			public int End
			{
				get { return end; }
				set { end = value; }
			}

			public bool PrevCharWasDollar
			{
				get { return prevCharWasDollar; }
			}

			public ExpressionPosition(int start, int end, bool prevCharWasDollar)
			{
				this.start = start;
				this.end = end;
				this.prevCharWasDollar = prevCharWasDollar;
			}
		}

		private static DictionaryEntry GetSeperators(string code)
		{
			string start = null, end = null;
			foreach (DictionaryEntry entry in Seperators)
			{
				if (code.IndexOf(entry.Key as string, 0) != -1)
				{
					if (start != null && code.IndexOf(entry.Key as string) != -1)
						continue; //handle a shorthanded seperator.
					// handle long seperator
					if (start != null && entry.Key.ToString().IndexOf(start as string) == -1)
					{
						throw new RailsException("Can't mix seperators in one file. Found both " + start + " and " + entry.Key);
					}
					start = entry.Key.ToString();
					end = entry.Value.ToString();
				}
			}

			if (start == null) //default, doesn't really matter, since it won't be used.
			{
				foreach (DictionaryEntry entry in Seperators)
				{
					return entry;
				}
			}
			return new DictionaryEntry(start, end);
		}

		public static string UnescapeInitialAndClosingDoubleQuotes(string code)
		{
			if (code.StartsWith(ClosingQuoteReplacement))
				code = DoubleQuote + code.Substring(ClosingQuoteReplacement.Length);
			if (code.EndsWith(ClosingQuoteReplacement))
				code = code.Substring(0, code.Length - ClosingQuoteReplacement.Length) +
						 DoubleQuote;
			return code;
		}
	}
}