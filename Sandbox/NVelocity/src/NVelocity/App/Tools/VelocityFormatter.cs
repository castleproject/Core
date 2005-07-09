namespace NVelocity.App.Tools
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Text;
	using NVelocity.Context;

	/// <summary>
	/// Formatting tool for inserting into the Velocity WebContext.  Can
	/// format dates or lists of objects.
	/// 
	/// <p>Here's an example of some uses:
	/// <code><pre>
	/// $formatter.formatShortDate($object.Date)
	/// $formatter.formatLongDate($db.getRecord(232).getDate())
	/// $formatter.formatArray($array)
	/// $formatter.limitLen(30, $object.Description)
	/// </pre></code>
	/// </summary>
	/// <author><a href="mailto:sean@somacity.com">Sean Legassick</a></author>
	/// <author><a href="mailto:dlr@collab.net">Daniel Rall</a></author>
	/// <version>$Id: VelocityFormatter.cs,v 1.5 2003/11/05 04:15:02 corts Exp $</version>
	public class VelocityFormatter
	{
		internal IContext context = null;
		internal SupportClass.TextNumberFormat nf = SupportClass.TextNumberFormat.getTextNumberInstance();

		/// <summary>
		/// Constructor needs a backpointer to the context.
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
		public String formatShortDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, -1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date in 'long' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String formatLongDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, -1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date/time in 'short' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String formatShortDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, 3, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats a date/time in 'long' style.
		/// </summary>
		/// <param name="date">A Date.</param>
		/// <returns>A String.</returns>
		public String formatLongDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, 1, CultureInfo.CurrentCulture), date);
		}

		/// <summary>
		/// Formats an array into the form "A, B and C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <returns>A String.</returns>
		public String formatArray(Object array)
		{
			return formatArray(array, ", ", " and ");
		}

		/// <summary> 
		/// Formats an array into the form
		/// "A&lt;delim&gt;B&lt;delim&gt;C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <param name="delim">A String.</param>
		/// <returns>A String.</returns>
		public String formatArray(Object array, String delim)
		{
			return formatArray(array, delim, delim);
		}

		/// <summary>
		/// Formats an array into the form
		/// "A&lt;delim&gt;B&lt;finaldelim&gt;C".
		/// </summary>
		/// <param name="array">An Object.</param>
		/// <param name="delim">A String.</param>
		/// <param name="finalDelim">A String.</param>
		/// <returns>A String.</returns>
		public String formatArray(Object array, String delim, String finaldelim)
		{
			// TODO: if this is not right - it will blow up
			Array a = (Array) array;

			StringBuilder sb = new StringBuilder();
			int arrayLen = ((double[]) array).Length;
			for (int i = 0; i < arrayLen; i++)
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
					sb.Append(finaldelim);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Formats a list into the form "A, B and C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <returns>A String.</returns>
		public String formatVector(IList list)
		{
			return formatVector(list, ", ", " and ");
		}

		/// <summary>
		/// Formats a list into the form "A&lt;delim&gt;B&lt;delim&gt;C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <param name="delim">A String.</param>
		/// <returns>A String.</returns>
		public String formatVector(IList list, String delim)
		{
			return formatVector(list, delim, delim);
		}

		/// <summary>
		/// Formats a list into the form
		/// "Adelim&gt;B&lt;finaldelim&gt;C".
		/// </summary>
		/// <param name="list">A list.</param>
		/// <param name="delim">A String.</param>
		/// <param name="finalDelim">A String.</param>
		/// <returns>A String.</returns>
		public String formatVector(IList list, String delim, String finaldelim)
		{
			StringBuilder sb = new StringBuilder();
			Int32 size = list.Count;
			for (int i = 0; i < size; i++)
			{
				sb.Append(list[i].ToString());
				if (i < size - 2)
				{
					sb.Append(delim);
				}
				else if (i < size - 1)
				{
					sb.Append(finaldelim);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Limits 'string' to 'maxlen' characters.  If the string gets
		/// curtailed, "..." is appended to it.
		/// </summary>
		/// <param name="maxlen">An int with the maximum length.</param>
		/// <param name="string">A String.</param>
		/// <returns>A String.</returns>
		public String limitLen(int maxlen, String string_Renamed)
		{
			return limitLen(maxlen, string_Renamed, "...");
		}

		/// <summary>
		/// Limits 'string' to 'maxlen' character.  If the string gets
		/// curtailed, 'suffix' is appended to it.
		/// </summary>
		/// <param name="maxlen">An int with the maximum length.</param>
		/// <param name="string">A String.</param>
		/// <param name="suffix">A String.</param>
		/// <returns>A String.</returns>
		public String limitLen(int maxlen, String string_Renamed, String suffix)
		{
			String ret = string_Renamed;
			if (string_Renamed.Length > maxlen)
			{
				ret = string_Renamed.Substring(0, (maxlen - suffix.Length) - (0)) + suffix;
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
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'VelocityAlternator' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
		public class VelocityAlternator
		{
			private void InitBlock(VelocityFormatter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private VelocityFormatter enclosingInstance;

			public VelocityFormatter Enclosing_Instance
			{
				get { return enclosingInstance; }

			}

			protected internal String[] alternates = null;
			protected internal int current = 0;

			/// <summary> Constructor takes an array of Strings.
			/// *
			/// </summary>
			/// <param name="alternates">A String[].
			///
			/// </param>
			public VelocityAlternator(VelocityFormatter enclosingInstance, String[] alternates)
			{
				InitBlock(enclosingInstance);
				this.alternates = alternates;
			}

			/// <summary> Alternates to the next in the list.
			/// *
			/// </summary>
			/// <returns>The current alternate in the sequence.
			///
			/// </returns>
			public String alternate()
			{
				current++;
				current %= alternates.Length;
				return "";
			}

			/// <summary> Returns the current alternate.
			/// *
			/// </summary>
			/// <returns>A String.
			///
			/// </returns>
			public override String ToString()
			{
				return alternates[current];
			}
		}

		/// <summary>
		/// As VelocityAlternator, but calls <code>alternate()</code>
		/// automatically on rendering in a template.
		/// </summary>
		//UPGRADE_NOTE: Field 'EnclosingInstance' was added to class 'VelocityAutoAlternator' to access its enclosing instance. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1019"'
		public class VelocityAutoAlternator : VelocityAlternator
		{
			private void InitBlock(VelocityFormatter enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private VelocityFormatter enclosingInstance;

			new public VelocityFormatter Enclosing_Instance
			{
				get { return enclosingInstance; }

			}

			/// <summary> Constructor takes an array of Strings.
			/// *
			/// </summary>
			/// <param name="alternates">A String[].
			///
			/// </param>
			public VelocityAutoAlternator(VelocityFormatter enclosingInstance, String[] alternates) : base(enclosingInstance, alternates)
			{
				InitBlock(enclosingInstance);
			}

			/// <summary> Returns the current alternate, and automatically alternates
			/// to the next alternate in its sequence (trigged upon
			/// rendering).
			/// *
			/// </summary>
			/// <returns>The current alternate in the sequence.
			///
			/// </returns>
			public override String ToString()
			{
				String s = alternates[current];
				alternate();
				return s;
			}
		}

		/// <summary> Makes an alternator object that alternates between two values.
		/// *
		/// <p>Example usage in a Velocity template:
		/// *
		/// <code><pre>
		/// &lt;table&gt;
		/// $formatter.makeAlternator("rowcolor", "#c0c0c0", "#e0e0e0")
		/// #foreach $item in $items
		/// #begin
		/// &lt;tr&gt;&lt;td bgcolor="$rowcolor"&gt;$item.Name&lt;/td&gt;&lt;/tr&gt;
		/// $rowcolor.alternate()
		/// #end
		/// &lt;/table&gt;
		/// </pre></code>
		/// *
		/// </summary>
		/// <param name="name">The name for the alternator int the context.
		/// </param>
		/// <param name="alt1">The first alternate.
		/// </param>
		/// <param name="alt2">The second alternate.
		/// </param>
		/// <returns>The newly created instance.
		///
		/// </returns>
		public String makeAlternator(String name, String alt1, String alt2)
		{
			String[] alternates = new String[] {alt1, alt2};
			context.Put(name, new VelocityAlternator(this, alternates));
			return "";
		}

		/// <summary> Makes an alternator object that alternates between three
		/// values.
		/// *
		/// </summary>
		/// <seealso cref=" #makeAlternator(String name, String alt1, String alt2)
		///
		/// "/>
		public String makeAlternator(String name, String alt1, String alt2, String alt3)
		{
			String[] alternates = new String[] {alt1, alt2, alt3};
			context.Put(name, new VelocityAlternator(this, alternates));
			return "";
		}

		/// <summary> Makes an alternator object that alternates between four values.
		/// *
		/// </summary>
		/// <seealso cref=" #makeAlternator(String name, String alt1, String alt2)
		///
		/// "/>
		public String makeAlternator(String name, String alt1, String alt2, String alt3, String alt4)
		{
			String[] alternates = new String[] {alt1, alt2, alt3, alt4};
			context.Put(name, new VelocityAlternator(this, alternates));
			return "";
		}

		/// <summary> Makes an alternator object that alternates between two values
		/// automatically.
		/// *
		/// </summary>
		/// <seealso cref=" #makeAlternator(String name, String alt1, String alt2)
		///
		/// "/>
		public String makeAutoAlternator(String name, String alt1, String alt2)
		{
			String[] alternates = new String[] {alt1, alt2};
			context.Put(name, new VelocityAutoAlternator(this, alternates));
			return "";
		}

		/// <summary> Returns a default value if the object passed is null.
		/// </summary>
		public Object isNull(Object o, Object dflt)
		{
			if (o == null)
			{
				return dflt;
			}
			else
			{
				return o;
			}
		}
	}
}