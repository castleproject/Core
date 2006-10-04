namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Text;

	using Context = Context.IContext;

	/// <summary> Utilities for dealing with the AST node structure.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: NodeUtils.cs,v 1.4 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class NodeUtils
	{
		/// <summary> Collect all the &lt;SPECIAL_TOKEN&gt;s that
		/// are carried along with a token. Special
		/// tokens do not participate in parsing but
		/// can still trigger certain lexical actions.
		/// In some cases you may want to retrieve these
		/// special tokens, this is simply a way to
		/// extract them.
		/// </summary>
		public static String specialText(Token t)
		{
			String specialText = "";

			if (t.SpecialToken == null || t.SpecialToken.Image.StartsWith("##"))
				return specialText;

			Token tmp_t = t.SpecialToken;

			while (tmp_t.SpecialToken != null)
			{
				tmp_t = tmp_t.SpecialToken;
			}

			while (tmp_t != null)
			{
				String st = tmp_t.Image;

				StringBuilder sb = new StringBuilder();

				for (int i = 0; i < st.Length; i++)
				{
					char c = st[i];

					if (c == '#' || c == '$')
					{
						sb.Append(c);
					}

					/*
		    *  more dreaded MORE hack :)
		    * 
		    *  looking for ("\\")*"$" sequences
		    */

					if (c == '\\')
					{
						bool ok = true;
						bool term = false;

						int j = i;
						for (ok = true; ok && j < st.Length; j++)
						{
							char cc = st[j];

							if (cc == '\\')
							{
								/*
				*  if we see a \, keep going
				*/
								continue;
							}
							else if (cc == '$')
							{
								/*
				*  a $ ends it correctly
				*/
								term = true;
								ok = false;
							}
							else
							{
								/*
				*  nah...
				*/
								ok = false;
							}
						}

						if (term)
						{
							String foo = st.Substring(i, (j) - (i));
							sb.Append(foo);
							i = j;
						}
					}
				}

				specialText += sb.ToString();

				tmp_t = tmp_t.Next;
			}

			return specialText;
		}

		/// <summary>  complete node literal
		/// *
		/// </summary>
		public static String tokenLiteral(Token t)
		{
			return specialText(t) + t.Image;
		}

		/// <summary> Utility method to interpolate context variables
		/// into string literals. So that the following will
		/// work:
		/// *
		/// #set $name = "candy"
		/// $image.getURI("${name}.jpg")
		/// *
		/// And the string literal argument will
		/// be transformed into "candy.jpg" before
		/// the method is executed.
		/// </summary>
		public static String interpolate(String argStr, Context vars)
		{
			StringBuilder argBuf = new StringBuilder();

			for (int cIdx = 0; cIdx < argStr.Length; )
			{
				char ch = argStr[cIdx];

				switch (ch)
				{
					case '$':
						StringBuilder nameBuf = new StringBuilder();
						for (++cIdx; cIdx < argStr.Length; ++cIdx)
						{
							ch = argStr[cIdx];
							if (ch == '_' || ch == '-' || Char.IsLetterOrDigit(ch))
								nameBuf.Append(ch);
							else if (ch == '{' || ch == '}')
								continue;
							else
								break;
						}

						if (nameBuf.Length > 0)
						{
							Object value_ = vars.Get(nameBuf.ToString());

							if (value_ == null)
								argBuf.Append("$").Append(nameBuf.ToString());
							else
							{
								//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
								argBuf.Append(value_.ToString());
							}
						}
						break;


					default:
						argBuf.Append(ch);
						++cIdx;
						break;

				}
			}

			return argBuf.ToString();
		}
	}
}