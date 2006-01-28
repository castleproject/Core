namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Collections.Specialized;
	using System.IO;
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
		private static readonly String dictStart = "%{";
		private static readonly String dictEnd = "}";

		// Regex for key = 'value' pairs
		private static readonly Regex dictPairRegex = new Regex( 
			@" (\w+) \s* = \s* '(.*?)(?<!\\)' ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled );

		// Regex for string between pairs
		private static readonly Regex dictBetweenPairRegex = new Regex( 
			@"^\s*,\s*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled );

		// Regex for string at the begining or end of a dictionary string
		private static readonly Regex dictEdgePairRegex = new Regex( 
			@"^\s*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled );

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

			interpolate = rsvc.GetBoolean(RuntimeConstants.INTERPOLATE_STRINGLITERALS, true) && FirstToken.Image.StartsWith("\"") && ((FirstToken.Image.IndexOf((Char) '$') != - 1) || (FirstToken.Image.IndexOf((Char) '#') != - 1));

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
				//UPGRADE_ISSUE: The equivalent of constructor 'java.io.BufferedReader.BufferedReader' is incompatible with the expected type in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1109"'
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

			if (interpolate)
			{
				try
				{
					/*
		    *  now render against the real context
		    */

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
					/*
		    *  eh.  If anything wrong, just punt 
		    *  and output the literal 
		    */
					rsvc.Error("Error in interpolating string literal : " + e);
					result = image;
				}
			}

			if( IsDictionaryString(result) )
			{
				return InterpolateDictionaryString(result);
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Interpolates the dictionary string.
		/// dictionary string is any string in the format
		/// "%{ key='value' [,key2='value2' }"		
		/// "%{ key='value' [,key2='value2'] }"		
		/// </summary>
		/// <param name="str">
		///	If valid input a HybridDictionary with zero or more items,
		///	otherwise the input string
		/// </param>
		/// <returns></returns>
		private object InterpolateDictionaryString( string str )
		{
			MatchCollection matches = dictPairRegex.Matches( str );

			HybridDictionary hash = new HybridDictionary(matches.Count, true);

			int pos = dictStart.Length;

			Match match = null;

			while( ( match = dictPairRegex.Match(str, pos) ) != Match.Empty )
			{
				// make sure there is no invalid string before this match
				if( !AssertDictionaryString( str.Substring(pos, match.Index - pos ), hash.Count > 0 ) )
				{
					return str;
				}

				string key   = match.Groups[1].Value;
				string value = match.Groups[2].Value;

				// remove any escaped singlequote with a singlequote
				value = value.Replace( @"\'", "'" );

				hash[key] = value;
				pos = match.Index + match.Length;
			}

			int endPos = (str.Length - dictEnd.Length);

			// make sure there is invalid string after the last match
			if( pos < endPos && !AssertDictionaryString( str.Substring( pos, endPos - pos ), false ) )
			{
				return str;
			}
			
			return hash;
		}

		private bool AssertDictionaryString(string str, bool isBetweenPairStr )
		{
			Regex re = isBetweenPairStr ? dictBetweenPairRegex : dictEdgePairRegex;

			if( !re.IsMatch(str) )
			{
				rsvc.Error( String.Format( "Error in interpolating dictionary string, cannot contain str <{0}> {1} dictionary params",
					str, isBetweenPairStr ? "between" :"before" ));	
				
				return false;
			}			

			return true;
		}

		private bool IsDictionaryString( string str )
		{
			return str.StartsWith(dictStart) && str.EndsWith(dictEnd);
		}
	}
}
