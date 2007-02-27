namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
	using System.Text;
	using NVelocity.Context;

	/// <summary> 
	/// ASTStringLiteral support.
	/// </summary>
	public class ASTStringLiteral : SimpleNode
	{
		// begin and end dictionary string markers
		private static readonly String DictStart = "%{";
		private static readonly String DictEnd = "}";

		private bool interpolate = true;
		private SimpleNode nodeTree = null;
		private String image = "";
		private String interpolateimage = "";

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTStringLiteral"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public ASTStringLiteral(int id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ASTStringLiteral"/> class.
		/// </summary>
		/// <param name="p">The p.</param>
		/// <param name="id">The id.</param>
		public ASTStringLiteral(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>  init : we don't have to do much.  Init the tree (there
		/// shouldn't be one) and then see if interpolation is turned on.
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			base.Init(context, data);

			/*
			*  the stringlit is set at template parse time, so we can 
			*  do this here for now.  if things change and we can somehow 
			* create stringlits at runtime, this must
			*  move to the runtime execution path
			*
			*  so, only if interpolation is turned on AND it starts 
			*  with a " AND it has a  directive or reference, then we 
			*  can  interpolate.  Otherwise, don't bother.
			*/

			interpolate = FirstToken.Image.StartsWith("\"") &&
			              ((FirstToken.Image.IndexOf('$') != - 1) ||
			               (FirstToken.Image.IndexOf('#') != - 1));

			/*
			*  get the contents of the string, minus the '/" at each end
			*/

			image = FirstToken.Image.Substring(1, (FirstToken.Image.Length - 1) - (1));
			/*
			* tack a space on the end (dreaded <MORE> kludge)
			*/

			interpolateimage = image + " ";

			if (interpolate)
			{
				/*
				*  now parse and init the nodeTree
				*/
				TextReader br = new StringReader(interpolateimage);

				/*
				* it's possible to not have an initialization context - or we don't
				* want to trust the caller - so have a fallback value if so
				*
				*  Also, do *not* dump the VM namespace for this template
				*/

				nodeTree = rsvc.Parse(br, (context != null) ? context.CurrentTemplateName : "StringLiteral", false);

				/*
				*  init with context. It won't modify anything
				*/

				nodeTree.Init(context, rsvc);
			}

			return data;
		}

		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object Accept(IParserVisitor visitor, Object data)
		{
			return visitor.Visit(this, data);
		}

		/// <summary>  renders the value of the string literal
		/// If the properties allow, and the string literal contains a $ or a #
		/// the literal is rendered against the context
		/// Otherwise, the stringlit is returned.
		/// </summary>
		public override Object Value(IInternalContextAdapter context)
		{
			string result = image;

			if (IsDictionaryString(result))
			{
				return InterpolateDictionaryString(result, context);
			}
			else
			{
				if (interpolate)
				{
					try
					{
						TextWriter writer = new StringWriter();
						nodeTree.Render(context, writer);

						/*
						 * and return the result as a String
						 */

						String ret = writer.ToString();

						/*
						 *  remove the space from the end (dreaded <MORE> kludge)
						 */

						result = ret.Substring(0, (ret.Length - 1) - (0));
					}
					catch(Exception e)
					{
						rsvc.Error("Error in interpolating string literal : " + e);
						result = image;
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Interpolates the dictionary string.
		/// dictionary string is any string in the format
		/// "%{ key='value' [,key2='value2' }"		
		/// "%{ key='value' [,key2='value2'] }"		
		/// </summary>
		/// <param name="str">If valid input a HybridDictionary with zero or more items,
		///	otherwise the input string</param>
		/// <param name="context">NVelocity runtime context</param>
		private HybridDictionary InterpolateDictionaryString(string str, IInternalContextAdapter context)
		{
			HybridDictionary hash = new HybridDictionary(true);

			// key=val, key='val', key=$val, key=${val}, key='id$id'

			char[] contents = str.ToCharArray();
			int lastIndex;

			return RecursiveBuildDictionary(contents, 2, context, out lastIndex);
		}

		private HybridDictionary RecursiveBuildDictionary(char[] contents, int fromIndex, IInternalContextAdapter context,
		                                                  out int lastIndex)
		{
			lastIndex = 0;

			HybridDictionary hash = new HybridDictionary(true);

			bool inKey, valueStarted, expectSingleCommaAtEnd, inTransition, inEvaluationContext;
			inKey = false;
			inTransition = true;
			valueStarted = expectSingleCommaAtEnd = inEvaluationContext = false;
			StringBuilder sbKeyBuilder = new StringBuilder();
			StringBuilder sbValBuilder = new StringBuilder();

			for(int i = fromIndex; i < contents.Length; i++)
			{
				char c = contents[i];

				if (inTransition)
				{
					// Eat all insignificant chars
					if (c == ',' || c == ' ')
					{
						continue;
					}
					else if (c == '}') // Time to stop
					{
						lastIndex = i;
						break;
					}
					else
					{
						inTransition = false;
						inKey = true;
					}
				}

				if (c == '=' && inKey)
				{
					inKey = false;
					valueStarted = true;
					continue;
				}

				if (inKey)
				{
					sbKeyBuilder.Append(c);
				}
				else
				{
					if (valueStarted && c == ' ') continue;

					if (valueStarted)
					{
						valueStarted = false;

						if (c == '\'')
						{
							expectSingleCommaAtEnd = true;
							continue;
						}
						else if (c == '{')
						{
							object nestedHash = RecursiveBuildDictionary(contents, i + 1, context, out i);
							ProcessDictEntry(hash, sbKeyBuilder, nestedHash, context);
							inKey = false;
							valueStarted = false;
							inTransition = true;
							continue;
						}
					}

					if (c == '\\')
					{
						char ahead = contents[i + 1];

						// Within escape

						switch(ahead)
						{
							case 'r':
								i++;
								sbValBuilder.Append('\r');
								continue;
							case '\'':
								i++;
								sbValBuilder.Append('\'');
								continue;
							case '"':
								i++;
								sbValBuilder.Append('"');
								continue;
							case 'n':
								i++;
								sbValBuilder.Append('\n');
								continue;
						}
					}


					if ((c == '\'' && expectSingleCommaAtEnd) ||
						(!expectSingleCommaAtEnd && c == ',') || 
						(!inEvaluationContext && c == '}'))
					{
						ProcessDictEntry(hash, sbKeyBuilder, sbValBuilder, expectSingleCommaAtEnd, context);

						inKey = false;
						valueStarted = false;
						inTransition = true;

						if (!inEvaluationContext && c == '}')
						{
							lastIndex = i;
							break;
						}
					}
					else
					{
						if (!inEvaluationContext && c == '{')
						{
							inEvaluationContext = true;
						}
						else if (inEvaluationContext && c == '}')
						{
							inEvaluationContext = false;
						}

						sbValBuilder.Append(c);
					}
				}

				if (i == contents.Length - 1)
				{
					if (sbKeyBuilder.ToString().Trim() == String.Empty)
					{
						break;
					}

					lastIndex = i;

					ProcessDictEntry(hash, sbKeyBuilder, sbValBuilder, expectSingleCommaAtEnd, context);

					inKey = false;
					valueStarted = false;
					inTransition = true;
				}
			}

			return hash;
		}

		private void ProcessDictEntry(HybridDictionary map, StringBuilder keyBuilder, object value,
		                              IInternalContextAdapter context)
		{
			object key = keyBuilder.ToString().Trim();

			if (key.ToString().StartsWith("$"))
			{
				object keyVal = EvaluateInPlace(key.ToString(), context);

				if (keyVal == null)
				{
					throw new ArgumentException("The dictionary entry " + key +
					                            " evaluated to null, but null is not a valid dictionary key");
				}

				key = keyVal;
			}

			map[key] = value;

			keyBuilder.Length = 0;
		}

		private void ProcessDictEntry(HybridDictionary map,
		                              StringBuilder keyBuilder, StringBuilder value,
		                              bool isTextContent, IInternalContextAdapter context)
		{
			object val = value.ToString().Trim();

			// Is it a reference?
			if (val.ToString().StartsWith("$") || val.ToString().IndexOf('$') != -1)
			{
				val = EvaluateInPlace(val.ToString(), context);
			}
			else if (!isTextContent)
			{
				// Is it a Int32 or Single?

				if (val.ToString().IndexOf('.') == -1)
				{
					try
					{
						val = Convert.ToInt32(val);
					}
					catch(Exception)
					{
						throw new ArgumentException("Could not convert dictionary value for entry " + keyBuilder + " with value " + val +
						                            " to Int32. If the value is supposed to be a string, it must be enclosed with '' (single quotes)");
					}
				}
				else
				{
					try
					{
						val = Convert.ToSingle(val);
					}
					catch(Exception)
					{
						throw new ArgumentException("Could not convert dictionary value for entry " + keyBuilder + " with value " + val +
						                            " to Single. If the value is supposed to be a string, it must be enclosed with '' (single quotes)");
					}
				}
			}

			ProcessDictEntry(map, keyBuilder, val, context);

			// Reset buffers

			value.Length = 0;
		}

		private object EvaluateInPlace(string content, IInternalContextAdapter context)
		{
			try
			{
				SimpleNode inlineNode = rsvc.Parse(new StringReader(content), context.CurrentTemplateName, false);

				inlineNode.Init(context, rsvc);

				return Evaluate(inlineNode, context);
			}
			catch(Exception)
			{
				throw new ArgumentException("Problem evaluating dictionary entry with content " + content);
			}
		}

		private object Evaluate(SimpleNode inlineNode, IInternalContextAdapter context)
		{
			if (inlineNode.ChildrenCount == 1)
			{
				INode child = inlineNode.GetChild(0);
				return child.Value(context);
			}
			else
			{
				StringBuilder result = new StringBuilder();

				for(int i = 0; i < inlineNode.ChildrenCount; i++)
				{
					INode child = inlineNode.GetChild(i);

					if (child.Type == ParserTreeConstants.REFERENCE)
					{
						result.Append(child.Value(context));
					}
					else
					{
						result.Append(child.Literal);
					}
				}

				return result.ToString();
			}
		}

		private bool IsDictionaryString(string str)
		{
			return str.StartsWith(DictStart) && str.EndsWith(DictEnd);
		}
	}
}