namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using NVelocity.Context;

	/// <summary> ASTStringLiteral support.  Will interpolate!
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <version> $Id: ASTStringLiteral.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class ASTStringLiteral : SimpleNode
	{
		// begin and end dictionary string markers
		private static readonly String DictStart = "%{";
		private static readonly String DictEnd = "}";

		// Regex for key = 'value' pairs
		// private static readonly Regex dictPairRegex = new Regex(
		//	@" (\w+) \s* = \s* '(.*?)(?<!\\)' ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

		// Regex for string between pairs
		// private static readonly Regex dictBetweenPairRegex = new Regex(
		//	@"^\s*,\s*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

		// Regex for string at the begining or end of a dictionary string
		// private static readonly Regex dictEdgePairRegex = new Regex(
		//	@"^\s*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

		/* cache the value of the interpolation switch */
		private bool interpolate = true;
		private SimpleNode nodeTree = null;
		private String image = "";
		private String interpolateimage = "";

		public ASTStringLiteral(int id) : base(id)
		{
		}

		public ASTStringLiteral(Parser p, int id) : base(p, id)
		{
		}

		/// <summary>  init : we don't have to do much.  Init the tree (there
		/// shouldn't be one) and then see if interpolation is turned on.
		/// </summary>
		public override Object Init(IInternalContextAdapter context, Object data)
		{
			/*
			*  simple habit...  we prollie don't have an AST beneath us
			*/

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

			interpolate = rsvc.GetBoolean(RuntimeConstants.INTERPOLATE_STRINGLITERALS, true) && 
			              FirstToken.Image.StartsWith("\"") &&
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
					catch (Exception e)
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

			bool inKey, valueStarted, expectSingleCommaAtEnd, inTransition;
			inKey = true;
			inTransition = false;
			valueStarted = expectSingleCommaAtEnd = false;
			StringBuilder sbKeyBuilder = new StringBuilder();
			StringBuilder sbValBuilder = new StringBuilder();
			
			for(int i = 2; i < contents.Length - 1; i++)
			{
				char c = contents[i];

				if (inTransition)
				{
					// Eat all insignificant chars
					if (c == ',' || c == ' ') 
					{
						continue;
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
					}
					
					if (!valueStarted)
					{
						if ((c == '\'' && expectSingleCommaAtEnd) || 
						    (!expectSingleCommaAtEnd && c == ','))
						{
							ProcessDictEntry(hash, sbKeyBuilder, sbValBuilder, expectSingleCommaAtEnd, context);
							
							inKey = false;
							valueStarted = false;
							inTransition = true;
						}
						else
						{
							sbValBuilder.Append(c);
						}
					}
				}

				if (i == contents.Length - 2)
				{
					if (sbKeyBuilder.ToString().Trim() == String.Empty)
					{
						break;
					}

					ProcessDictEntry(hash, sbKeyBuilder, sbValBuilder, expectSingleCommaAtEnd, context);
				}
			}

			return hash;
		}

		private void ProcessDictEntry(HybridDictionary map, 
		                              StringBuilder keyBuilder, StringBuilder value, 
		                              bool isTextContent, IInternalContextAdapter context)
		{
			object key = keyBuilder.ToString().Trim();
			object val = value.ToString().Trim();

			if (key.ToString().StartsWith("$"))
			{
				object keyVal = EvaluateInPlace(key.ToString(), context);
				
				if (keyVal == null)
				{
					throw new ArgumentException("The dictionary entry " + key + " evaluated to null, but null is not a valid dictionary key");
				}

				key = keyVal;
			}
			
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
						map[key] = Convert.ToInt32(val);
					}
					catch (Exception)
					{
						throw new ArgumentException("Could not convert dictionary value for entry " + key + " with value " + val +
													" to Int32. If the value is supposed to be a string, it must be enclosed with '' (single quotes)");
					}
				}
				else
				{
					try
					{
						map[key] = Convert.ToSingle(val);
					}
					catch (Exception)
					{
						throw new ArgumentException("Could not convert dictionary value for entry " + key + " with value " + val +
													" to Single. If the value is supposed to be a string, it must be enclosed with '' (single quotes)");
					}
				}
			}

			map[key] = val;
			
			// Reset buffers
			
			keyBuilder.Length = value.Length = 0;
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

		private bool IsDictionaryString(string str)
		{
			return str.StartsWith(DictStart) && str.EndsWith(DictEnd);
		}
	}
}