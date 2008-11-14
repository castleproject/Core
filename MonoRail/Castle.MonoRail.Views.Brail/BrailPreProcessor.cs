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
	using Framework;

	public class BrailPreProcessor : AbstractCompilerStep
	{
		public const string ClosingQuoteReplacement = "`^`";
		public const string DoubleQuote = "\"";
        const string TripleDoubleQuote = "\"\"\"";
		private readonly static IDictionary separators = CreateSeparators();
		private readonly BooViewEngine booViewEngine;
		private readonly IDictionary inputToCode = new Hashtable();
		private static readonly Regex escapeParametersStartingWithQuestionMarkRegEx
			= new Regex(@"(?<!<)\?([_\w][_\w\d]*)", RegexOptions.Compiled);

		public BrailPreProcessor(BooViewEngine booViewEngine)
		{
			this.booViewEngine = booViewEngine;
		}

		private static IDictionary CreateSeparators()
		{
			Hashtable seperators = new Hashtable();
			seperators.Add("<?brail", "?>");
			seperators.Add("<%", "%>");
			return seperators;
		}

		public string GetInputCode(ICompilerInput input)
		{
			return (string) inputToCode[input];
		}

		public override void Run()
		{
			ArrayList processed = new ArrayList();
			foreach(ICompilerInput input in Parameters.Input)
			{
				using(TextReader reader = input.Open())
				{
					string code = reader.ReadToEnd();
					if (booViewEngine.ConditionalPreProcessingOnly(input.Name) == false ||
					    ShouldPreProcess(code))
					{
					    code = Booify(code);
					}
                    else
					{
					    code = EscapeNullPropagationsInOutputExpression(code);
					}
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

		private static bool ShouldPreProcess(string code)
		{
			foreach(DictionaryEntry entry in separators)
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
			DictionaryEntry seperators = GetSeperators(code);
			string start = seperators.Key.ToString();
			string end = seperators.Value.ToString();

			while(index != -1)
			{
				index = code.IndexOf(start, lastIndex);
				if (index == -1)
					break;
				Output(buffer, code.Substring(lastIndex, index - lastIndex));
				int startReading = index + start.Length;
				lastIndex = code.IndexOf(end, startReading);
				if (lastIndex == -1)
					throw new MonoRailException("expected " + end);
				int lastIndexOffset = end.Length;
				if (code[lastIndex - 1] == '-')
				{
					--lastIndex;

					if (EndTagEndsWithNewline(code, lastIndex + lastIndexOffset))
					{
						lastIndexOffset += 2;
					}
					++lastIndexOffset;
				}
				string line = code.Substring(startReading, lastIndex - startReading);
				line = EscapeNullPropagationsInCode(line);
				buffer.WriteLine(line);
				lastIndex += lastIndexOffset;
			}
			string endingLine = code.Substring(lastIndex);

			Output(buffer, endingLine);
			return buffer.ToString();
		}

		private static bool EndTagEndsWithNewline(string code, int endIndex)
		{
			return code.Length > endIndex + 2 && code.Substring(endIndex + 1, 2) == "\r\n";
		}

		private static void Output(TextWriter buffer, string code)
		{
			if (code.Length == 0)
				return;
			IList<ExpressionPosition> expressions = GetExpressionsPositions(code);
			if (expressions.Count == 0)
			{
				OutputText(buffer, code);
				return;
			}

			int start = 0;
			foreach(ExpressionPosition position in expressions)
			{
				string text = code.Substring(start, position.Start - start);
				OutputText(buffer, text);
				string expression = code.Substring(position.Start + 2, position.End - (position.Start + 2));
				OutputExpression(buffer, expression, position.ShouldEscape);
				start = position.End + 1;
			}
			string remainingText = code.Substring(start, code.Length - start);
			OutputText(buffer, remainingText);
		}

		private static void OutputText(TextWriter buffer, string code)
		{
			code = EscapeInitialAndClosingDoubleQuotes(code);
			buffer.Write("output \"\"\"");
			buffer.Write(code);
			buffer.WriteLine("\"\"\"");
		}

        private static void OutputExpression(TextWriter buffer, string code, bool shouldEscape) {
            if (shouldEscape)
                buffer.Write("OutputEscaped ");
            else
                buffer.Write("output ");

            buffer.WriteLine(EscapeNullPropagationsInOutputExpression(code));
        }

        private static string EscapeNullPropagationsInCode(string escapedCode) {
            return escapeParametersStartingWithQuestionMarkRegEx.Replace(escapedCode, "TryGetParameter('$1')");
        }

        /// <summary>
        /// Given a brail output expression: ${...}
        /// replace any null propagations, while respecting quoted strings and escaped characters such as ("\"\"").
        /// </summary>
        private static string EscapeNullPropagationsInOutputExpression(string code)
		{
            StringBuilder escapedCode = new StringBuilder();
		    int position = 0;

            while (position < code.Length)
            {                
                int nextOpeningQuotePosition = FindStartOfNextQuotedString(code, position);

                // is there a quoted string in the remainder of the code fragment?
                if (nextOpeningQuotePosition >= 0)
                {
                    // append text up to string
                    int positionAfterEndOfClosingQuote = FindPositionAfterClosingQuote(code, nextOpeningQuotePosition);                    
                    var textBeforeQuotedString = code.Substring(position, nextOpeningQuotePosition - position);
                    escapedCode.Append(EscapeNullPropagationsInCode(textBeforeQuotedString));

                    // append the quoted string
                    escapedCode.Append(code.Substring(nextOpeningQuotePosition, positionAfterEndOfClosingQuote - nextOpeningQuotePosition));

                    // advance our index
                    position = positionAfterEndOfClosingQuote;
                }
                else
                {                    
                    escapedCode.Append(EscapeNullPropagationsInCode(code.Substring(position)));
                    break;
                }
            }

            return escapedCode.ToString();
		}

        /// <summary>
        /// Find the position after the end of the closing quote token for the quoted 
        /// substring that started at the specified position        
        /// 
        /// for example, given the following string x:
        ///  "abc" blah 
        /// 01234567890
        /// FindPositionAfterClosingQuote(x, 1) will return 6
        /// 
        /// handles triple-double-quotes, double, and single quotes, and escaped quotes within the string
        /// </summary>
        /// <param name="code">code fragment</param>
        /// <param name="openingQuotePosition">index of opening quote within the code fragment</param>
        /// <returns>position after closing quote, or -1 if not found</returns>
	    private static int FindPositionAfterClosingQuote(string code, int openingQuotePosition)
	    {          
	        if (IsTripleQuote(code, openingQuotePosition))
	            return code.IndexOf(TripleDoubleQuote, openingQuotePosition + 3) + 3;

            char quote = code[openingQuotePosition];

            // read each character, so we can ignore escapes
            for (int position = openingQuotePosition + 1; position < code.Length; position++)
            {
                if (code[position] == '\\')
                {
                    // skip
                    position++;
                    continue;
                }

                if (code[position] == quote)
                    return position + 1;

            }
            
	        return -1;
	    }

	    private static bool IsTripleQuote(string code, int startOfString)
	    {
	        return code[startOfString] =='"' && code.IndexOf(TripleDoubleQuote, startOfString) == startOfString;
	    }

        /// <summary>
        /// Returns the character position of the next instance of any boo string-quoting tokens: ', ", """
        /// Note that this returns the position of the beginning of the quote token, not the text inside the quote       
        /// </summary>
        /// <param name="code"></param>
        /// <param name="position">position to start search</param>        
	    private static int FindStartOfNextQuotedString(string code, int position)
	    {
	        int tripleDoubleQuotePosition = code.IndexOf(TripleDoubleQuote, position);
            int doubleQuotePosition = code.IndexOf(DoubleQuote, position);
            int singleQuotePosition = code.IndexOf("'", position);

            if (singleQuotePosition >= 0 && (doubleQuotePosition == -1 || singleQuotePosition < doubleQuotePosition))
                return singleQuotePosition;

            if (tripleDoubleQuotePosition >= 0 && tripleDoubleQuotePosition < doubleQuotePosition)
                return tripleDoubleQuotePosition;

	        return doubleQuotePosition;
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
			bool prevCharWasBang = false;
			for(int index = 0; index < code.Length; index++)
			{
				if (code[index] == '{')
				{
					bracesPositions.Add(new ExpressionPosition(index - 1, -1, prevCharWasDollar || prevCharWasBang, prevCharWasBang));
				}
				if (code[index] == '}' && bracesPositions.Count > 0)
				{
					ExpressionPosition position = bracesPositions[bracesPositions.Count - 1];
					if (ParentExpressionIsNotValid(bracesPositions, bracesPositions.Count))
					{
						bracesPositions.RemoveAt(bracesPositions.Count - 1);
					}
					else if (position.End == -1)
					{
						position.End = index;
					}
				}
				//handles escaping expressions with $$ as well
				prevCharWasDollar = code[index] == '$' && !prevCharWasDollar;
				prevCharWasBang = code[index] == '!' && !prevCharWasBang;
			}
			bracesPositions.RemoveAll(delegate(ExpressionPosition obj) { return !obj.PrevCharWasDollarOrBang; });
			return bracesPositions;
		}

		private static bool ParentExpressionIsNotValid(List<ExpressionPosition> bracesPositions,
		                                               int index)
		{
			if (index - 2 < 0) return false;
			ExpressionPosition parentExpression = bracesPositions[index - 2];
			if (parentExpression.PrevCharWasDollarOrBang == false)
				return ParentExpressionIsNotValid(bracesPositions, index - 1);
			return parentExpression.End == -1;
		}

		private static DictionaryEntry GetSeperators(string code)
		{
			string start = null, end = null;
			foreach(DictionaryEntry entry in separators)
			{
				if (code.IndexOf(entry.Key as string, 0) != -1)
				{
					if (start != null && code.IndexOf(entry.Key as string) != -1)
						continue; //handle a shorthanded seperator.
					// handle long seperator
					if (start != null && entry.Key.ToString().IndexOf(start as string) == -1)
					{
						throw new MonoRailException("Can't mix seperators in one file. Found both " + start + " and " + entry.Key);
					}
					start = entry.Key.ToString();
					end = entry.Value.ToString();
				}
			}

			if (start == null) //default, doesn't really matter, since it won't be used.
			{
				foreach(DictionaryEntry entry in separators)
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

		#region Nested type: ExpressionPosition

		private class ExpressionPosition
		{
			private readonly bool prevCharWasDollarOrBang;
			private readonly bool shouldEscape;
			private readonly int start;
			private int end;

			public ExpressionPosition(int start, int end, bool prevCharWasDollarOrBang, bool shouldEscape)
			{
				this.start = start;
				this.end = end;
				this.prevCharWasDollarOrBang = prevCharWasDollarOrBang;
				this.shouldEscape = shouldEscape;
			}

			public int Start
			{
				get { return start; }
			}

			public int End
			{
				get { return end; }
				set { end = value; }
			}

			public bool PrevCharWasDollarOrBang
			{
				get { return prevCharWasDollarOrBang; }
			}

			public bool ShouldEscape
			{
				get { return shouldEscape; }
			}
		}

		#endregion
	}    
}
