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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Text;

	/// <summary>
	/// Provides methods for working with strings and grammar. At the moment,
	/// it contains the ToSentence overloads.
	/// </summary>
	public class TextHelper : AbstractHelper
	{
		const string DefaultConnector = "and";

		/// <summary>
		/// Converts a camelized text to words. For instance:
		/// <c>FileWriter</c> is converted to <c>File Writer</c>
		/// </summary>
		/// <param name="pascalText">Content in pascal case</param>
		/// <returns></returns>
		public static string PascalCaseToWord(string pascalText)
		{
			if (pascalText == null) throw new ArgumentNullException("pascalText");

			if (pascalText == string.Empty) return string.Empty;

			StringBuilder sbText = new StringBuilder(pascalText.Length + 4);

			char[] chars = pascalText.ToCharArray();

			sbText.Append(chars[0]);

			for(int i=1; i < chars.Length; i++)
			{
				char c = chars[i];

				if (Char.IsUpper(c))
				{
					sbText.Append(' ');
				}

				sbText.Append(c);
			}

			return sbText.ToString();
		}

		/// <summary>
		/// Builds a phrase listing a series of strings with with proper sentence semantics,
		/// i.e. separating elements with &quot;, &quot; and prefacing the last element with
		/// the specified <paramref name="connector"/>.
		/// </summary>
		/// <param name="elements">Collection with items to use in the sentence.</param>
		/// <param name="connector">String to preface the last element.</param>
		/// <returns>String suitable for use in a sentence.</returns>
		/// <remarks>Calling <c>ToSentence( elements, "y" )</c> results in:
		/// <code>
		/// element1, element2 y element3
		/// </code>
		/// <para>If <paramref name="elements"/> is not an array of strings, each element will be
		/// converted to string through <see cref="object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>ToSentence</b>:
		/// <code>
		/// $TextHelper.ToSentence( elements, "y" )
		/// </code>
		/// </example>
		public string ToSentence(ICollection elements, string connector)
		{
			return ToSentence(elements, connector, true);
		}

		/// <summary>
		/// Builds a phrase listing a series of strings with with proper sentence semantics,
		/// i.e. separating elements with &quot;, &quot; and prefacing the last element with
		/// &quot; and &quot;.
		/// </summary>
		/// <param name="elements">Collection with items to use in the sentence.</param>
		/// <param name="skipLastComma">True to skip the comma before the connector, false to include it.</param>
		/// <returns>String suitable for use in a sentence.</returns>
		/// <remarks>Calling <c>ToSentence( elements, false )</c> results in:
		/// <code>
		/// element1, element2, and element3
		/// </code>
		/// <para>If <paramref name="elements"/> is not an array of strings, each element will be
		/// converted to string through <see cref="object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>ToSentence</b>:
		/// <code>
		/// $TextHelper.ToSentence( elements, false )
		/// </code>
		/// </example>
		public string ToSentence(ICollection elements, bool skipLastComma)
		{
			return ToSentence(elements, DefaultConnector, skipLastComma);
		}

		/// <summary>
		/// Builds a phrase listing a series of strings with with proper sentence semantics,
		/// i.e. separating elements with &quot;, &quot; and prefacing the last element with
		/// &quot; and &quot;.
		/// </summary>
		/// <param name="elements">Collection with items to use in the sentence.</param>
		/// <returns>String suitable for use in a sentence.</returns>
		/// <remarks>Calling <c>ToSentence( elements )</c> results in:
		/// <code>
		/// element1, element2 and element3
		/// </code>
		/// <para>If <paramref name="elements"/> is not an array of strings, each element will be
		/// converted to string through <see cref="object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>ToSentence</b>:
		/// <code>
		/// $TextHelper.ToSentence( elements )
		/// </code>
		/// </example>
		public string ToSentence(ICollection elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}

			return ToSentence(elements, DefaultConnector, true);
		}

		/// <summary>
		/// Builds a phrase listing a series of strings with with proper sentence semantics,
		/// i.e. separating elements with &quot;, &quot; and prefacing the last element with
		/// the specified <paramref name="connector"/>.
		/// </summary>
		/// <param name="elements">Collection with items to use in the sentence.</param>
		/// <param name="connector">String to preface the last element.</param>
		/// <param name="skipLastComma">True to skip the comma before the <paramref name="connector"/>, false to include it.</param>
		/// <returns>String suitable for use in a sentence.</returns>
		/// <remarks>Calling <c>ToSentence( elements, "y", false )</c> results in:
		/// <code>
		/// element1, element2, y element3
		/// </code>
		/// <para>If <paramref name="elements"/> is not an array of strings, each element will be
		/// converted to string through <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>ToSentence</b>:
		/// <code>
		/// $TextHelper.ToSentence( elements, "y", false )
		/// </code>
		/// </example>
		public string ToSentence(ICollection elements, string connector, bool skipLastComma)
		{
			string[] array = elements as string[];
			if (array == null)
			{
				array = new string[elements.Count];
				IEnumerator enumerator = elements.GetEnumerator();
				for(int i = 0; i < elements.Count; i++)
				{
					enumerator.MoveNext();
					array[i] = enumerator.Current.ToString();
				}
			}
			return ToSentence(array, connector, skipLastComma);
		}

		/// <summary>
		/// Builds a phrase listing a series of strings with with proper sentence semantics,
		/// i.e. separating elements with &quot;, &quot; and prefacing the last element with
		/// the specified <paramref name="connector"/>.
		/// </summary>
		/// <param name="elements">Array of strings with items to use in the sentence.</param>
		/// <param name="connector">String to preface the last element.</param>
		/// <param name="skipLastComma">True to skip the comma before the <paramref name="connector"/>, false to include it.</param>
		/// <returns>String suitable for use in a sentence.</returns>
		/// <remarks>Calling <c>ToSentence( elements, "y", false )</c> results in:
		/// <code>
		/// element1, element2, y element3
		/// </code>
		/// </remarks>
		/// <example>This example shows how to use <b>ToSentence</b>:
		/// <code>
		/// $TextHelper.ToSentence( elements, "y", false )
		/// </code>
		/// </example>
		public string ToSentence(string[] elements, string connector, bool skipLastComma)
		{
			switch(elements.Length)
			{
				case 0:
					{
						return String.Empty;
					}
				case 1:
					{
						return elements[0];
					}
				case 2:
					{
						return String.Format("{0} {1} {2}", elements[0], connector, elements[1]);
					}
				default:
					{
						String[] allButLast = new String[elements.Length - 1];

						Array.Copy(elements, allButLast, elements.Length - 1);

						return String.Format("{0}{1} {2} {3}",
						                     String.Join(", ", allButLast),
						                     skipLastComma ? "" : ",",
						                     connector,
						                     elements[elements.Length - 1]);
					}
			}
		}

		/// <summary>
		/// Shortens a text to the specified length and wraps it into a span-
		/// element that has the title-property with the full text associated.
		/// This is convenient for displaying properties in tables that might
		/// have very much content (desription fields etc.) without destroying
		/// the table's layout.
		/// Due to the title-property of the surrounding span-element, the full
		/// text is displayed in the browser while hovering over the shortened
		/// text.
		/// </summary>
		/// <param name="text">The text to display</param>
		/// <param name="maxLength">The maximum number of character to display</param>
		/// <returns>The generated HTML</returns>
		public string Fold(string text, int maxLength)
		{
			// Empty text
			if (text == null) return "";

			// maxLenght <= 0 switches off folding
			// Determine whether text must be cut
			if (maxLength <= 0 || text.Length < maxLength) return text;

			StringBuilder caption = new StringBuilder();
			foreach(string word in text.Split())
			{
				if (caption.Length + word.Length + 1 > maxLength - 1) break;
				if (caption.Length > 0) caption.Append(" "); // Adding space
				caption.Append(word);
			}

			caption.Append("&hellip;");
			return string.Format("<span title=\"{1}\">{0}</span>", caption, text.Replace("\"", "&quot;"));
		}
	}
}