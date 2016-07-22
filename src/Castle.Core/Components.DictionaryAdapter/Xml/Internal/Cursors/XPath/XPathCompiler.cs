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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public static class XPathCompiler
	{
		public static CompiledXPath Compile(string path)
		{
			if (null == path)
				throw Error.ArgumentNull("path");

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
			for (CompiledXPathStep step = null;;)
			{
				if (!ParseStep(source, path, ref step))
					return false;
				if (source.Token == Token.EndOfInput)
					return true;
				if (!Consume(source, Token.StepSeparator))
					return false;
				if (step.IsAttribute)
					return false;
			}
		}

		private static bool ParseStep(Tokenizer source, CompiledXPath path, ref CompiledXPathStep step)
		{
			var previous = step;
			var start    = source.Index;

			if (!ParseNodeCore(source, StepFactory, ref step))
				return false;

			if (step != previous)
			{
				var text = source.GetConsumedText(start);
				step.Path = XPathExpression.Compile(text);

				if (previous == null)
					path.FirstStep = step;
				else
					LinkNodes(previous, step);

				path.Depth++;
			}
			return true;
		}

		private static bool ParseNodeCore<TNode>(Tokenizer source, Func<TNode> factory, ref TNode node)
			where TNode : CompiledXPathNode
		{
			if (!Consume(source, Token.SelfReference))
			{
				node = factory();

				if (Consume(source, Token.AttributeStart))
					node.IsAttribute = true;

				if (!ParseQualifiedName(source, node))
					return false;
			}

			return node == null
				? source.Token != Token.PredicateStart
				: ParsePredicateList(source, node);
		}

		private static bool ParsePredicateList(Tokenizer source, CompiledXPathNode parent)
		{
			while (Consume(source, Token.PredicateStart))
				if (!ParsePredicate(source, parent))
					return false;

			return true;
		}

		private static bool ParsePredicate(Tokenizer source, CompiledXPathNode parent)
		{
			// Don't need to check this; caller must have already checked it.
			//if (!Consume(source, Token.PredicateStart))
			//    return false;

			if (!ParseAndExpression(source, parent))
				return false;

			if (!Consume(source, Token.PredicateEnd))
				return false;

			return true;
		}

		private static bool ParseAndExpression(Tokenizer source, CompiledXPathNode parent)
		{
			for (;;)
			{
				if (!ParseExpression(source, parent))
					return false;

				if (source.Token != Token.Name || source.Text != "and")
					return true;
				source.Consume();
			}
		}

		private static bool ParseExpression(Tokenizer source, CompiledXPathNode parent)
		{
			var isLeftToRight
				=  source.Token == Token.Name
				|| source.Token == Token.AttributeStart
				|| source.Token == Token.SelfReference;

			return (isLeftToRight)
				? ParseLeftToRightExpression(source, parent)
				: ParseRightToLeftExpression(source, parent);
		}

		private static bool ParseLeftToRightExpression(Tokenizer source, CompiledXPathNode parent)
		{
			CompiledXPathNode node;
			if (!ParseNestedPath(source, parent, out node))
			    return false;

			if (!Consume(source, Token.EqualsOperator))
				return true;

			XPathExpression value;
			if (!ParseValue(source, out value))
				return false;

			node.Value = value;
			return true;
		}

		private static bool ParseRightToLeftExpression(Tokenizer source, CompiledXPathNode parent)
		{
			XPathExpression value;
			if (!ParseValue(source, out value))
				return false;

			if (!Consume(source, Token.EqualsOperator))
				return false;

			CompiledXPathNode node;
			if (!ParseNestedPath(source, parent, out node))
			    return false;

			node.Value = value;
			return true;
		}

		private static bool ParseNestedPath(Tokenizer source, CompiledXPathNode parent, out CompiledXPathNode node)
		{
			for (node = null;;)
			{
				if (!ParseNode(source, parent, ref node))
					return false;
				if (!Consume(source, Token.StepSeparator))
					break;
				if (node.IsAttribute)
					return false;
			}

			if (node == null)
			{
				var dependencies = parent.Dependencies;
				if (dependencies.Count != 0)
					return false;
				dependencies.Add(node = NodeFactory()); // Self-reference
			}
			return true;
		}

		private static bool ParseNode(Tokenizer source, CompiledXPathNode parent, ref CompiledXPathNode node)
		{
			var previous = node;

			if (!ParseNodeCore(source, NodeFactory, ref node))
				return false;

			if (node != previous)
			{
				if (previous == null)
					parent.Dependencies.Add(node);
				else
					LinkNodes(previous, node);
			}
			return true;
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

		private static void LinkNodes(CompiledXPathNode previous, CompiledXPathNode next)
		{
			previous.NextNode = next;
			next.PreviousNode = previous;
		}

		private static readonly Func<CompiledXPathNode>
			NodeFactory = () => new CompiledXPathNode();

		private static readonly Func<CompiledXPathStep>
			StepFactory = () => new CompiledXPathStep();

		private enum Token
		{
			Name,
			SelfReference,

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
								case '.':  token = Token.SelfReference;     return;
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
#if DOTNET40
				return XmlConvert.IsWhitespaceChar(c);
#else
				// Source: http://www.w3.org/TR/REC-xml/#NT-S
				return ' '  == c
					|| '\t' == c
					|| '\r' == c
					|| '\n' == c
					;
#endif
			}

			private static bool IsNameStartChar(char c)
			{
#if DOTNET40
			    return XmlConvert.IsStartNCNameChar(c);
#else
				// Source: http://www.w3.org/TR/REC-xml/#NT-NameStartChar
				return ('A' <= c && c <= 'Z')
					|| ('a' <= c && c <= 'z')
					|| ('_' == c)
					|| ('\u00C0' <= c && c <= '\u00D6')
					|| ('\u00D8' <= c && c <= '\u00F6')
					|| ('\u00F8' <= c && c <= '\u02FF')
					|| ('\u0370' <= c && c <= '\u037D')
					|| ('\u037F' <= c && c <= '\u1FFF')
					|| ('\u200C' <= c && c <= '\u200D')
					|| ('\u2070' <= c && c <= '\u218F')
					|| ('\u2C00' <= c && c <= '\u2FEF')
					|| ('\u3001' <= c && c <= '\uD7FF')
					|| ('\uF900' <= c && c <= '\uFDCF')
					|| ('\uFDF0' <= c && c <= '\uFFFD')
					// [10000-EFFFF] not supported by C#
					;
#endif
			}

			private static bool IsNameChar(char c)
			{
#if DOTNET40
			    return XmlConvert.IsNCNameChar(c);
#else
				// Source: http://www.w3.org/TR/REC-xml/#NT-NameChar
				return IsNameStartChar(c)
					|| ('-' == c)
					|| ('.' == c)
					|| ('0' <= c && c <='9')
					|| ('\u00B7' == c)
					|| ('\u0300' <= c && c <= '\u036F')
					|| ('\u203F' <= c && c <= '\u2040')
					;
#endif
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
