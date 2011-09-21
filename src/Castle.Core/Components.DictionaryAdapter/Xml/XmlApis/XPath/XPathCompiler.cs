// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.XPath;

	public static class XPathCompiler
	{
		public static CompiledXPath Compile(string path)
		{
			if (null == path)
				throw new ArgumentNullException("path");

			// Compile whole path to catch errors
			var result = new CompiledXPath();
			result.Path = XPathExpression.Compile(path);

			// Try to split into individual path steps
			var tokenizer = new Tokenizer(path);
			if (!ParsePath(tokenizer, result))
				result.MakeNotCreatable();

			// Finish compilation
			result.Prepare();
			return result;
		}

		private static bool ParsePath(Tokenizer source, CompiledXPath path)
		{
			CompiledXPathStep next;
			if (!ParseStep(source, out next))
				return false;
			path.FirstStep = next;
			var parent = next;
			path.Depth = 1;

			for (;;)
			{
				if (source.Token == Token.EndOfInput)
					return true;
				if (source.Token != Token.StepSeparator)
					return false;
				if (parent.IsAttribute)
					return false;
				source.Consume();

				if (!ParseStep(source, out next))
					return false;
				parent.NextStep = next;
				parent = next;
				path.Depth++;
			}
		}

		private static bool ParseStep(Tokenizer source, out CompiledXPathStep step)
		{
			step = new CompiledXPathStep();
			var start = source.Index;

			if (!ParseNodeCore(source, step))
				return false;

			var path  = source.GetConsumedText(start);
			step.Path = XPathExpression.Compile(path);
			return true;
		}

		private static bool ParseNodeCore(Tokenizer source, CompiledXPathNode node)
		{
			if (Consume(source, Token.AttributeStart))
				node.IsAttribute = true;

			if (!ParseQualifiedName(source, node))
				return false;

			if (!ParsePredicateList(source, node))
				return false;

			return true;
		}

		private static bool ParsePredicateList(Tokenizer source, CompiledXPathNode node)
		{
			var dependencies = node.Dependencies;

			while (source.Token == Token.PredicateStart)
				if (!ParsePredicate(source, dependencies))
					return false;

			return true;
		}

		private static bool ParsePredicate(Tokenizer source, IList<CompiledXPathNode> dependencies)
		{
			// Don't need to check this; caller must have already checked it.
			//if (!Consume(source, Token.PredicateStart))
			//    return false;
			source.Consume();

			if (!ParseAndExpression(source, dependencies))
				return false;

			if (!Consume(source, Token.PredicateEnd))
				return false;

			return true;
		}

		private static bool ParseAndExpression(Tokenizer source, IList<CompiledXPathNode> dependencies)
		{
			for (;;)
			{
				CompiledXPathNode node;
				if (!ParseExpression(source, out node))
					return false;
				dependencies.Add(node);

				if (source.Token != Token.Name || source.Text != "and")
					return true;
				source.Consume();
			}
		}

		private static bool ParseExpression(Tokenizer source, out CompiledXPathNode node)
		{
			return (source.Token == Token.Name || source.Token == Token.AttributeStart)
				? ParseLeftToRightExpression(source, out node)
				: ParseRightToLeftExpression(source, out node);
		}

		private static bool ParseLeftToRightExpression(Tokenizer source, out CompiledXPathNode node)
		{
			if (!ParsePathExpression(source, out node))
			    return false;

			if (!Consume(source, Token.EqualsOperator))
				return true;

			XPathExpression value;
			if (!ParseValue(source, out value))
				return false;

			GetLastNode(node).Value = value;
			return true;
		}

		private static bool ParseRightToLeftExpression(Tokenizer source, out CompiledXPathNode node)
		{
			XPathExpression value;
			if (!ParseValue(source, out value))
				return Try.Failure(out node);

			if (!Consume(source, Token.EqualsOperator))
				return Try.Failure(out node);

			if (!ParsePathExpression(source, out node))
			    return false;

			GetLastNode(node).Value = value;
			return true;
		}

		private static bool ParsePathExpression(Tokenizer source, out CompiledXPathNode node)
		{
			if (!ParseNode(source, out node))
				return false;
			var parent = node;

			for (;;)
			{
				if (!Consume(source, Token.StepSeparator))
					return true;
				if (parent.IsAttribute)
					return false;

				CompiledXPathNode next;
			    if (!ParseNode(source, out next))
			        return false;
				parent.NextNode = next;
				parent = next;
			}
		}

		private static bool ParseNode(Tokenizer source, out CompiledXPathNode node)
		{
			node = new CompiledXPathNode();

			return ParseNodeCore(source, node);
		}

		private static bool ParseValue(Tokenizer source, out XPathExpression value)
		{
			var start = source.Index;

			var parsed =
				Consume(source, Token.StringLiteral) ||
				(
					Consume(source, Token.VariableStart) &&
					ParseQualifiedName(source, null)
				);
			if (!parsed)
				return Try.Failure(out value);

			var xpath = source.GetConsumedText(start);
			value = XPathExpression.Compile(xpath);
			return true;
		}

		private static bool ParseQualifiedName(Tokenizer source, CompiledXPathNode node)
		{
			string nameA, nameB;

			if (!ParseName(source, out nameA))
				return false;

			if (!Consume(source, Token.NameSeparator))
			{
				if (node != null)
					node.LocalName = nameA;
				return true;
			}

			if (!ParseName(source, out nameB))
				return false;

			if (node != null)
			{
				node.Prefix    = nameA;
				node.LocalName = nameB;
			}
			return true;
		}

		private static bool ParseName(Tokenizer source, out string name)
		{
			if (source.Token != Token.Name)
				return Try.Failure(out name);
			name = source.Text;
			source.Consume();
			return true;
		}

		private static bool Consume(Tokenizer source, Token token)
		{
			if (source.Token != token)
				return false;
			source.Consume();
			return true;
		}

		private static CompiledXPathNode GetLastNode(CompiledXPathNode node)
		{
			for (;;)
			{
				var next = node.NextNode;
				if (next == null) return node;
				node = next;
			}
		}

		private enum Token
		{
			Name,

			StepSeparator,
			NameSeparator,
			AttributeStart,
			VariableStart,
			EqualsOperator,

			PredicateStart,
			PredicateEnd,

			StringLiteral,

			EndOfInput,
			Error
		}

		private class Tokenizer
		{
			private readonly string input;

			private State state;
			private Token token;
			private int   index;
			private int   start;
			private int   prior;

			public Tokenizer(string input)
			{
				this.input = input;
				this.state = State.Initial;
				this.index = -1;
				Consume();
			}

			// Type of the current token
			public Token Token
			{
				get { return token; }
			}

			// Text of the current token
			public string Text
			{
				get { return input.Substring(start, index - start + 1); }
			}

			// Gets text that has been consumed previously
			public string GetConsumedText(int start)
			{
				return input.Substring(start, prior - start + 1);
			}

			// Index where current token starts
			public int Index
			{
				get { return start; }
			}

			// Consider the current token consumed, and read next token
			public void Consume()
			{
				prior = index;

			    for(;;)
			    {
					var c = ReadChar();

			        switch (state)
			        {
			            case State.Initial:
							start = index;
							switch (c)
							{
								case '/':  token = Token.StepSeparator;     return;
								case ':':  token = Token.NameSeparator;     return;
								case '@':  token = Token.AttributeStart;    return;
								case '$':  token = Token.VariableStart;     return;
								case '=':  token = Token.EqualsOperator;    return;
								case '[':  token = Token.PredicateStart;    return;
								case ']':  token = Token.PredicateEnd;      return;
								case '\0': token = Token.EndOfInput;        return;
								case '\'': state = State.SingleQuoteString; break;
								case '\"': state = State.DoubleQuoteString; break;
								default:
									if      (IsNameStartChar(c)) { state = State.Name; }
									else if (IsWhitespace(c))    { /* Ignore */ }
									else                         { state = State.Failed; }
									break;
							}
							break;

			            case State.Name:
							if (IsNameChar(c)) { break; }
							RewindChar();
							token = Token.Name;
							state = State.Initial;
							return;

			            case State.SingleQuoteString:
							if (c != '\'') { break; }
						    token = Token.StringLiteral;
						    state = State.Initial;
						    return;
						
			            case State.DoubleQuoteString:
							if (c != '\"') { break; }
						    token = Token.StringLiteral;
						    state = State.Initial;
						    return;

						case State.Failed:
							token = Token.Error;
							return;
			        }
			    }
			}

			private char ReadChar()
			{
				return (++index < input.Length)
					? input[index]
					: default(char); // EOF sentinel
			}

			private void RewindChar()
			{
				--index;
			}

			private static bool IsWhitespace(char c)
			{
				return XmlConvert.IsWhitespaceChar(c);
			}

			private static bool IsNameStartChar(char c)
			{
			    return XmlConvert.IsStartNCNameChar(c);
			}

			private static bool IsNameChar(char c)
			{
			    return XmlConvert.IsNCNameChar(c);
			}

			private enum State
			{
				Initial = 0,
				Name,
				SingleQuoteString,
				DoubleQuoteString,
				Failed
			}
		}
	}
}
#endif
