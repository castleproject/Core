// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.App.Tools
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Text;
	using Context;

	/// <summary>
	/// Formatting tool for inserting into the Velocity WebContext.  Can
	/// format dates or lists of objects.
	/// 
	/// <para>Here's an example of some uses:
	/// <code><pre>
	/// $formatter.formatShortDate($object.Date)
	/// $formatter.formatLongDate($db.getRecord(232).getDate())
	/// $formatter.formatArray($array)
	/// $formatter.limitLen(30, $object.Description)
	/// </pre></code>
	/// </para>
	/// </summary>
	/// <author><a href="mailto:sean@somacity.com">Sean Legassick</a></author>
	/// <author><a href="mailto:dlr@collab.net">Daniel Rall</a></author>
	/// <version>$Id: VelocityFormatter.cs,v 1.5 2003/11/05 04:15:02 corts Exp $</version>
	public class VelocityFormatter
	{
		internal IContext context = null;
		internal SupportClass.TextNumberFormat textNumberFormat = SupportClass.TextNumberFormat.GetTextNumberInstance();

		/// <summary>
		/// Constructor needs a backPointer to the context.
		/// </summary>
		/// <param name="context">A Context.</param>
		public VelocityFormatter(IContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Formats a date in 'short' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String FormatShortDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, -1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date in 'long' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String FormatLongDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, -1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date/time in 'short' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String FormatShortDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, 3, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date/time in 'long' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String FormatLongDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, 1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats an array into the form "A, B and C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <returns>A String.</returns>
		public String FormatArray(Object array)
		{
			return FormatArray(array, ", ", " and ");
		}

		/// <summary> 
		/// Formats an array into the form
		/// "A&lt;delim&gt;B&lt;delim&gt;C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <param name="delim">A String.</param>
		/// <returns>A String.</returns>
		public String FormatArray(Object array, String delim)
		{
			return FormatArray(array, delim, delim);
		}

		/// <summary>
		/// Formats an array into the form
		/// "A&lt;delim&gt;B&lt;finalDelimiter&gt;C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <param name="delim">A String.</param>
		/// <param name="finalDelimiter">A String.</param>
		/// <returns>A String.</returns>
		public String FormatArray(Object array, String delim, String finalDelimiter)
		{
			// TODO: if this is not right - it will blow up
			Array a = (Array) array;

			StringBuilder sb = new StringBuilder();
			int arrayLen = ((double[]) array).Length;
			for(int i = 0; i < arrayLen; i++)
			{
				// Use the Array.get method as this will automatically
				// wrap primitive types in a suitable Object-derived
				// wrapper if necessary.
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				//UPGRADE_ISSUE: Method 'java.lang.reflect.Array.get' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangreflectArrayget_javalangObject_int"'

				//TODO: not sure if this is right
				//sb.Append(Array.get(array, i).ToString());
				sb.Append(a.GetValue(i).ToString());

				if (i < arrayLen - 2)
				{
					sb.Append(delim);
				}
				else if (i < arrayLen - 1)
				{
					sb.Append(finalDelimiter);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Formats a list into the form "A, B and C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <returns>A String.</returns>
		public String FormatVector(IList list)
		{
			return FormatVector(list, ", ", " and ");
		}

		/// <summary>
		/// Formats a list into the form "A&lt;delim&gt;B&lt;delim&gt;C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <param name="delim">A String.</param>
		/// <returns>A String.</returns>
		public String FormatVector(IList list, String delim)
		{
			return FormatVector(list, delim, delim);
		}

		/// <summary>
		/// Formats a list into the form
		/// "Adelim&gt;B&lt;finalDelimiter&gt;C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <param name="delim">A String.</param>
		/// <param name="finalDelimiter">A String.</param>
		/// <returns>A String.</returns>
		public String FormatVector(IList list, String delim, String finalDelimiter)
		{
			StringBuilder sb = new StringBuilder();
			Int32 size = list.Count;
			for(int i = 0; i < size; i++)
			{
				sb.Append(list[i].ToString());
				if (i < size - 2)
				{
					sb.Append(delim);
				}
				else if (i < size - 1)
				{
					sb.Append(finalDelimiter);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Limits 'string' to 'maximumLength' characters.  If the string gets
		/// curtailed, "..." is appended to it.
		/// </summary>
		/// <param name="maximumLength">An int with the maximum length.</param>
		/// <param name="value">A String.</param>
		/// <returns>A String.</returns>
		public String LimitLen(int maximumLength, String value)
		{
			return LimitLen(maximumLength, value, "...");
		}

		/// <summary>
		/// Limits 'string' to 'maximumLength' character.  If the string gets
		/// curtailed, 'suffix' is appended to it.
		/// </summary>
		/// <param name="maximumLength">An int with the maximum length.</param>
		/// <param name="value">A String.</param>
		/// <param name="suffix">A String.</param>
		/// <returns>A String.</returns>
		public String LimitLen(int maximumLength, String value, String suffix)
		{
			String ret = value;
			if (value.Length > maximumLength)
			{
				ret = value.Substring(0, (maximumLength - suffix.Length) - (0)) + suffix;
			}
			return ret;
		}

		/// <summary>
		/// Class that returns alternating values in a template.  It stores
		/// a list of alternate Strings, whenever alternate() is called it
		/// switches to the next in the list.  The current alternate is
		/// retrieved through toString() - i.e. just by referencing the
		/// object in a Velocity template.  For an example of usage see the
		/// makeAlternator() method below.
		/// </summary>
		public class VelocityAlternator
		{
			protected internal String[] alternates = null;
			protected internal int current = 0;

			/// <summary>
			/// Constructor takes an array of Strings.
			/// </summary>
			/// <param name="alternates">A String[].
			/// </param>
			public VelocityAlternator(params String[] alternates)
			{
				this.alternates = alternates;
			}

			/// <summary>
			/// Alternates to the next in the list.
			/// </summary>
			/// <returns>The current alternate in the sequence.</returns>
			public String Alternate()
			{
				current++;
				current %= alternates.Length;
				return string.Empty;
			}

			/// <summary>
			/// Returns the current alternate.
			/// </summary>
			/// <returns>A String.</returns>
			public override String ToString()
			{
				return alternates[current];
			}
		}

		/// <summary>
		/// As VelocityAlternator, but calls <code>alternate()</code>
		/// automatically on rendering in a template.
		/// </summary>
		public class VelocityAutoAlternator : VelocityAlternator
		{
			/// <summary>
			/// Constructor takes an array of Strings.
			/// </summary>
			/// <param name="alternates">A String[].
			///
			/// </param>
			public VelocityAutoAlternator(params String[] alternates)
				: base(alternates)
			{
			}

			/// <summary>
			/// Returns the current alternate, and automatically alternates
			/// to the next alternate in its sequence (triggered upon
			/// rendering).
			/// </summary>
			/// <returns>The current alternate in the sequence.</returns>
			public override String ToString()
			{
				String s = alternates[current];
				Alternate();
				return s;
			}
		}

		/// <summary>
		/// Makes an alternator object that alternates between two values.
		/// 
		/// <para>Example usage in a Velocity template:
		/// 
		/// <code>
		/// &lt;table&gt;
		/// $formatter.makeAlternator("rowColor", "#c0c0c0", "#e0e0e0")
		/// #foreach $item in $items
		/// &lt;tr&gt;&lt;td bgcolor="$rowColor"&gt;$item.Name&lt;/td&gt;&lt;/tr&gt;
		/// $rowColor.alternate()
		/// #end
		/// &lt;/table&gt;
		/// </code>
		/// </para>
		/// </summary>
		/// <param name="name">The name for the alternator int the context.</param>
		/// <param name="alt1">The first alternate.</param>
		/// <param name="alt2">The second alternate.</param>
		/// <returns>The newly created instance.</returns>
		public String MakeAlternator(String name, String alt1, String alt2)
		{
			context.Put(name, new VelocityAlternator(alt1, alt2));
			return string.Empty;
		}

		/// <summary>
		/// Makes an alternator object that alternates between three values.
		/// </summary>
		public String MakeAlternator(String name, String alt1, String alt2, String alt3)
		{
			context.Put(name, new VelocityAlternator(alt1, alt2, alt3));
			return string.Empty;
		}

		/// <summary>
		/// Makes an alternator object that alternates between four values.
		/// </summary>
		public String MakeAlternator(String name, String alt1, String alt2, String alt3, String alt4)
		{
			context.Put(name, new VelocityAlternator(alt1, alt2, alt3, alt4));
			return string.Empty;
		}

		/// <summary>
		/// Makes an alternator object that alternates between two values
		/// automatically.
		/// </summary>
		public String MakeAutoAlternator(String name, String alt1, String alt2)
		{
			context.Put(name, new VelocityAutoAlternator(alt1, alt2));
			return string.Empty;
		}

		/// <summary>
		/// Returns a default value if the object passed is null.
		/// </summary>
		public Object IsNull(Object o, Object defaultValue)
		{
			if (o == null)
				return defaultValue;
			else
				return o;
		}
	}
}