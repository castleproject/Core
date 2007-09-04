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

namespace Castle.MonoRail.Framework.TransformFilters.Formatters
{
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Converts wiki formatted text to html formatted text.
	/// </summary>
	/// <remarks>
	/// The following tags are parsed:
	/// <list type="table">
	/// <item><term>italic</term></item>
	/// <item><term>bold</term></item>
	/// <item><term>bold italic</term></item>
	/// <item><term>underlined</term></item>
	/// <item><term>striked</term></item>
	/// <item><term>code</term></item>
	/// <item><term>pre</term></item>
	/// <item><term>box</term></item>
	/// <item><term>pipe tables</term></item>
	/// </list>
	/// More about style formats <see href="http://en.wikipedia.org/wiki/Help:Editing"/>
	/// More about tables <see href="http://en.wikipedia.org/wiki/Help:Table"/>
	/// </remarks>
	public class WikiFormatter
	{
		private Regex italic = new Regex(@"''(?<content>.+?)''", RegexOptions.Compiled | RegexOptions.Singleline);
		private Regex bold = new Regex(@"'''(?<content>.+?)'''", RegexOptions.Compiled | RegexOptions.Singleline);
		private Regex boldItalic = new Regex(@"'''''(?<content>.+?)'''''", RegexOptions.Compiled | RegexOptions.Singleline);

		private Regex underlined = new Regex(@"__(?<content>.+?)__", RegexOptions.Compiled | RegexOptions.Singleline);
		private Regex striked = new Regex(@"\-\-(?<content>.+?)\-\-", RegexOptions.Compiled | RegexOptions.Singleline);

		private static Regex code = new Regex(@"\{\{(?<content>.+?)\}\}", RegexOptions.Compiled | RegexOptions.Singleline);
		private static Regex pre = new Regex(@"\{\{\{\{(?<content>.+?)\}\}\}\}", RegexOptions.Compiled | RegexOptions.Singleline);
		private static Regex box = new Regex(@"\(\(\((?<content>.+?)\)\)\)", RegexOptions.Compiled | RegexOptions.Singleline);

		private Regex table = new Regex(@"\{\|(\.*[^\n]*)?\n.+?\|\}", RegexOptions.Compiled | RegexOptions.Singleline);
		
		private Match match;

		/// <summary>
		/// Converts a string with wiki formatting to html formatting.
		/// </summary>
		/// <param name="wiki">the wiki formatted code</param>
		/// <returns>the html formatted code</returns>
		public string Format(string wiki)
		{
			StringBuilder sb = new StringBuilder(wiki);

			sb.Replace("\r", "");
			if (!sb.ToString().EndsWith("\n")) sb.Append("\n");
			
			sb.Replace("<!--", "($__castlerocks__^)");
			sb.Replace("-->", "(^__castlerocks__$)");

			MatchTables(sb);

			MatchWithSimpleReplace(sb, boldItalic, "<b><i>", "</i></b>");
			MatchWithSimpleReplace(sb, bold, "<b>", "</b>");
			MatchWithSimpleReplace(sb, italic, "<i>", "</i>");
			MatchWithSimpleReplace(sb, underlined, "<u>", "</u>");
			MatchWithSimpleReplace(sb, striked, "<strike>", "</strike>");
			MatchWithSimpleReplace(sb, pre, "<pre>", "</pre>");
			MatchWithSimpleReplace(sb, code, "<code>", "</code>");
			MatchWithSimpleReplace(sb, box, @"<table class=""box"" cellpadding=""0"" cellspacing=""0""><tr><td>", "</td></tr></table>");
							  
			sb.Replace("\n\n", "<br/><br/>");

			sb.Replace("($__castlerocks__^)", "<!--");
			sb.Replace("(^__castlerocks__$)", "-->");

			return sb.ToString();
		}

		#region matching functions
		
		private void MatchWithSimpleReplace(StringBuilder sb, Regex pattern, string openingTags, string closingTags)
		{
			match = pattern.Match(sb.ToString());
			while (match.Success)
			{
				sb.Remove(match.Index, match.Length);
				sb.Insert(match.Index, openingTags + match.Groups["content"].Value + closingTags);
				match = pattern.Match(sb.ToString());
			}
		}

		private void MatchTables(StringBuilder sb)
		{
			match = table.Match(sb.ToString());

			while (match.Success)
			{
				sb.Remove(match.Index, match.Length);
				sb.Insert(match.Index, BuildTable(match.Value));
				match = table.Match(sb.ToString());
			}
		}
		
		#endregion
		
		#region Building functions
		
		/// <summary>
		/// Converts a wiki pipe formatting to an HTML table
		/// </summary>
		/// <param name="markup">the pipe markup</param>
		/// <returns>the html table</returns>
		private string BuildTable(string markup)
		{
			bool hasOpenTR = false;
			string[] lines = markup.Split(new char[] {'\n'});

			// make sure nothing stupid happens
			if (lines.Length < 3)
				return "invalid table code";

			StringBuilder result = new StringBuilder();

			// start the table
			string header = lines[0].Trim();
			result.Append("<table" + (header.Length == 2 ? "" : header.Substring(2)) + ">");

			for (int i = 1; i < lines.Length - 1; i++)
			{
				string currentLine = lines[i].Trim();
				string firstCharacter = currentLine.Substring(0, 1);

				if (currentLine.Substring(0, 2) == "|-") // row
				{
					string after = currentLine.Substring(1).Trim();
					// check if we've hit a new line
					if (hasOpenTR) result.Append("</tr>");

					// strip |------------- notation
					while (after != string.Empty && after.Substring(0, 1) == "-")
						after = after.Substring(1);

					// start a fresh one
					result.Append(after != string.Empty ? "<tr " + after.Trim() + ">" : "<tr>");
					hasOpenTR = true;
				}
				else if (firstCharacter == "!" || firstCharacter == "|" || currentLine.Substring(0, 2) == "|+")
				{
					// perpare for caption
					if (currentLine.Substring(0, 2) == "|+")
					{
						firstCharacter = "+";
						currentLine = currentLine.Substring(1);
					}

					string tag;
					switch (firstCharacter)
					{
						case "|":
							tag = "td";
							break;
						case "!":
							tag = "th";
							currentLine = currentLine.Replace("!!", "||");
							break;
						case "+":
							tag = "caption";
							break;
						default:
							return "unknow indicator in table";
					}

					// if this is a caption tag, don't open a row.
					if (hasOpenTR == false && (firstCharacter != "+"))
					{
						result.Append("<tr>");
						hasOpenTR = true;
					}

					//string[] columns = currentLine.Substring(1).Split(new char[] {'|','|'});
					string[] columns = SplitByString(currentLine.Substring(1), "||");

					foreach (string column in columns)
					{
						// test for cell parameters
						string[] parameters = column.Split('|');

						if (parameters.Length == 1)
						{
							result.Append("<" + tag + ">" + column.Trim() + "</" + tag + ">");
						}
						else
						{
							string attributes = parameters[0].Trim();
							string content = parameters[1].Trim();

							result.Append("<" + tag + " " + attributes + ">" + content + "</" + tag + ">");
						}
					}
				}
			}

			if (result.ToString().EndsWith("</caption>"))
			{
				result.Append("</table>");
			}
			else
			{
				result.Append("</tr></table>");
			}

			return result.ToString();
		}
		
		#endregion	

		private string[] SplitByString(string testString, string split) 
		{ 
			int offset = 0; 
			int index = 0; 
			int[] offsets = new int[testString.Length + 1]; 

			while(index < testString.Length) 
			{
				int indexOf = testString.IndexOf(split, index); 
				if ( indexOf != -1 ) 
				{ 
					offsets[offset++] = indexOf; 
					index = (indexOf+split.Length); 
				} else 
				{ 
					index = testString.Length; 
				} 
			}
			string[] final = new string[offset+1]; 
			if ( offset < 1 ) 
			{ 
				final[0] = testString; 
			} 
			else 
			{ 
				offset--; 

				final[0] = testString.Substring(0, offsets[0]); 
				for(int i = 0; i < offset; i++) 
				{ 
					final[i+1] = testString.Substring(offsets[i]+split.Length, offsets[i+1]-offsets[i]-split.Length); 
				} 
			
				final[offset+1] = testString.Substring(offsets[offset]+split.Length); 
			} 

			return final; 
		}	
	}
}
