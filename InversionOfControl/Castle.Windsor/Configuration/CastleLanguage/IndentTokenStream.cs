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

namespace Castle.Windsor.Configuration.CastleLanguage
{
	using System;
	using System.Collections;
	using antlr;


	public class IndentTokenStream : TokenStream
	{
		public static readonly int MAX_INDENTS = 100;
		public static readonly int FIRST_COLUMN = 1;

		/** The stack of indent levels (column numbers) */
		int[] indentStack = new int[MAX_INDENTS];

		/** stack pointer */
		int sp = -1; // grow upwards

		ArrayList tokens = new ArrayList();

		private WindsorConfLanguageLexer _lexer;

		public IndentTokenStream(WindsorConfLanguageLexer lexer)
		{
			_lexer = lexer;
			Push(FIRST_COLUMN);
		}

		public IToken nextToken()
		{
			// if something in queue, just remove and return it
			if (tokens.Count > 0)
			{
				IToken t = (IToken) tokens[0];
				tokens.RemoveAt(0);
				// System.out.println(t);
				return t;
			}

			insertImaginaryIndentDedentTokens();

			return nextToken();
		}

		protected void insertImaginaryIndentDedentTokens()
		{
			IToken t = _lexer.nextToken();

			// if not a NEWLINE, doesn't signal indent/dedent work; just enqueue
			if (t.Type != WindsorConfLanguageLexer.NEWLINE)
			{
				tokens.Add(t);
				return;
			}
			
			// save NEWLINE in the queue
			tokens.Add(t);
			
			// grab first token of next line
			t = _lexer.nextToken();

			// compute col as the column number of next non-WS token in line
			int col = t.getColumn(); // column dictates indent/dedent
			
			if (t.Type == WindsorConfLanguageLexer.EOF)
			{
				col = 1; // pretend EOF always happens at left edge
			}
			else if (t.Type == WindsorConfLanguageLexer.LEADING_WS)
			{
				col = t.getText().Length + 1; // col is 1 spot after size
			}

			// compare to last indent level
			int lastIndent = Peek();
			
			if (col > lastIndent)
			{ // they indented; track and gen INDENT
				Push(col);
				Token indent = new CommonToken(WindsorLanguageParser.INDENT, "");
				indent.setColumn(t.getColumn());
				indent.setLine(t.getLine());
				tokens.Add(indent);
			}
			else if (col < lastIndent)
			{ // they dedented
				
				// how far back did we dedent?
				int prevIndex = FindPreviousIndent(col);
				
				// generate DEDENTs for each indent level we backed up over
				for (int d = sp - 1; d >= prevIndex; d--)
				{
					Token dedent = new CommonToken(WindsorLanguageParser.DEDENT, "");
					dedent.setColumn(t.getColumn());
					dedent.setLine(t.getLine());
					tokens.Add(dedent);
				}
				sp = prevIndex; // pop those off indent level
			}
			if (t.Type != WindsorConfLanguageLexer.LEADING_WS)
			{ // discard WS
				tokens.Add(t);
			}
		}


		protected void Push(int i)
		{
			if (sp >= MAX_INDENTS)
			{
				throw new ApplicationException("stack overflow");
			}
			sp++;
			indentStack[sp] = i;
		}

		protected int Pop()
		{
			if (sp < 0)
			{
				throw new ApplicationException("stack underflow");
			}
			int top = indentStack[sp];
			sp--;
			return top;
		}

		protected int Peek()
		{
			return indentStack[sp];
		}

		/** Return the index on stack of previous indent level == i else -1 */

		protected int FindPreviousIndent(int i)
		{
			for (int j = sp - 1; j >= 0; j--)
			{
				if (indentStack[j] == i)
				{
					return j;
				}
			}
			return -1;
		}
	}
}