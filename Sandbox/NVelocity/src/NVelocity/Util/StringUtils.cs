using System;
using System.Collections;
using NVelocity;

namespace NVelocity.Util {
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */


    /// <summary> This class provides some methods for dynamically
    /// invoking methods in objects, and some string
    /// manipulation methods used by torque. The string
    /// methods will soon be moved into the turbine
    /// string utilities class.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
    /// </author>
    /// <version> $Id: StringUtils.cs,v 1.3 2003/10/27 13:54:12 corts Exp $
    ///
    /// </version>
    public class StringUtils {
	public StringUtils() {
	    InitBlock();
	}
	private void  InitBlock() {
	    //UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javalangSystemgetProperty_javalangString"'
	    EOL = Environment.NewLine;
	    EOL_LENGTH = EOL.Length;
	}
	/// <summary> Line separator for the OS we are operating on.
	/// </summary>
	//UPGRADE_NOTE: Final was removed from the declaration of 'EOL '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
	//UPGRADE_NOTE: The initialization of  'EOL' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private static System.String EOL;

	/// <summary> Length of the line separator.
	/// </summary>
	//UPGRADE_NOTE: Final was removed from the declaration of 'EOL_LENGTH '. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1003"'
	//UPGRADE_NOTE: The initialization of  'EOL_LENGTH' was moved to method 'InitBlock'. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1005"'
	private static int EOL_LENGTH;

	/// <summary> Concatenates a list of objects as a String.
	/// *
	/// </summary>
	/// <param name="list">The list of objects to concatenate.
	/// </param>
	/// <returns>    A text representation of the concatenated objects.
	///
	/// </returns>
	public virtual System.String concat(IList list) {
	    System.Text.StringBuilder sb = new System.Text.StringBuilder();
	    int size = list.Count;

	    for (int i = 0; i < size; i++) {
		sb.Append(list[i].ToString());
	    }
	    return sb.ToString();
	}

	/// <summary> Return a package name as a relative path name
	/// *
	/// </summary>
	/// <param name="String">package name to convert to a directory.
	/// </param>
	/// <returns>String directory path.
	///
	/// </returns>
	static public System.String getPackageAsPath(System.String pckge) {
	    return pckge.Replace('.', System.IO.Path.DirectorySeparatorChar.ToString()[0]) + System.IO.Path.DirectorySeparatorChar.ToString();
	}

	/// <summary> <p>
	/// Remove underscores from a string and replaces first
	/// letters with capitals.  Other letters are changed to lower case.
	/// </p>
	/// *
	/// <p>
	/// For example <code>foo_bar</code> becomes <code>FooBar</code>
	/// but <code>foo_barBar</code> becomes <code>FooBarbar</code>.
	/// </p>
	/// *
	/// </summary>
	/// <param name="data">string to remove underscores from.
	/// </param>
	/// <returns>String
	/// </returns>
	/// <deprecated>Use the org.apache.commons.util.StringUtils class
	/// instead.  Using its firstLetterCaps() method in conjunction
	/// with a StringTokenizer will achieve the same result.
	///
	/// </deprecated>
	static public System.String removeUnderScores(System.String data) {
	    System.String temp = null;
	    System.Text.StringBuilder out_Renamed = new System.Text.StringBuilder();
	    temp = data;

	    SupportClass.Tokenizer st = new SupportClass.Tokenizer(temp, "_");

	    while (st.HasMoreTokens()) {
		System.String element = (System.String) st.NextToken();
		out_Renamed.Append(firstLetterCaps(element));
	    }

	    return out_Renamed.ToString();
	}

	/// <summary> <p>
	/// 'Camels Hump' replacement of underscores.
	/// </p>
	/// *
	/// <p>
	/// Remove underscores from a string but leave the capitalization of the
	/// other letters unchanged.
	/// </p>
	/// *
	/// <p>
	/// For example <code>foo_barBar</code> becomes <code>FooBarBar</code>.
	/// </p>
	/// *
	/// </summary>
	/// <param name="data">string to hump
	/// </param>
	/// <returns>String
	///
	/// </returns>
	static public System.String removeAndHump(System.String data) {
	    return removeAndHump(data, "_");
	}

	/// <summary> <p>
	/// 'Camels Hump' replacement.
	/// </p>
	/// *
	/// <p>
	/// Remove one string from another string but leave the capitalization of the
	/// other letters unchanged.
	/// </p>
	/// *
	/// <p>
	/// For example, removing "_" from <code>foo_barBar</code> becomes <code>FooBarBar</code>.
	/// </p>
	/// *
	/// </summary>
	/// <param name="data">string to hump
	/// </param>
	/// <param name="replaceThis">string to be replaced
	/// </param>
	/// <returns>String
	///
	/// </returns>
	static public System.String removeAndHump(System.String data, System.String replaceThis) {
	    System.String temp = null;
	    System.Text.StringBuilder out_Renamed = new System.Text.StringBuilder();
	    temp = data;

	    SupportClass.Tokenizer st = new SupportClass.Tokenizer(temp, replaceThis);

	    while (st.HasMoreTokens()) {
		System.String element = (System.String) st.NextToken();
		out_Renamed.Append(capitalizeFirstLetter(element));
	    } //while

	    return out_Renamed.ToString();
	}

	/// <summary> <p>
	/// Makes the first letter caps and the rest lowercase.
	/// </p>
	/// *
	/// <p>
	/// For example <code>fooBar</code> becomes <code>Foobar</code>.
	/// </p>
	/// *
	/// </summary>
	/// <param name="data">capitalize this
	/// </param>
	/// <returns>String
	///
	/// </returns>
	static public System.String firstLetterCaps(System.String data) {
	    System.String firstLetter = data.Substring(0, (1) - (0)).ToUpper();
	    System.String restLetters = data.Substring(1).ToLower();
	    return firstLetter + restLetters;
	}

	/// <summary> <p>
	/// Capitalize the first letter but leave the rest as they are.
	/// </p>
	/// *
	/// <p>
	/// For example <code>fooBar</code> becomes <code>FooBar</code>.
	/// </p>
	/// *
	/// </summary>
	/// <param name="data">capitalize this
	/// </param>
	/// <returns>String
	///
	/// </returns>
	static public System.String capitalizeFirstLetter(System.String data) {
	    System.String firstLetter = data.Substring(0, (1) - (0)).ToUpper();
	    System.String restLetters = data.Substring(1);
	    return firstLetter + restLetters;
	}

	/// <summary> Create a string array from a string separated by delim
	/// *
	/// </summary>
	/// <param name="line">the line to split
	/// </param>
	/// <param name="delim">the delimter to split by
	/// </param>
	/// <returns>a string array of the split fields
	///
	/// </returns>
	public static System.String[] split(System.String line, System.String delim) {
	    ArrayList list = new ArrayList();
	    SupportClass.Tokenizer t = new SupportClass.Tokenizer(line, delim);
	    while (t.HasMoreTokens()) {
		list.Add(t.NextToken());
	    }
	    return (System.String[]) list.ToArray(typeof(String));
	}

	/// <summary> Chop i characters off the end of a string.
	/// This method assumes that any EOL characters in String s
	/// and the platform EOL will be the same.
	/// A 2 character EOL will count as 1 character.
	/// *
	/// </summary>
	/// <param name="string">String to chop.
	/// </param>
	/// <param name="i">Number of characters to chop.
	/// </param>
	/// <returns>String with processed answer.
	///
	/// </returns>
	public static System.String chop(System.String s, int i) {
	    return chop(s, i, EOL);
	}

	/// <summary> Chop i characters off the end of a string.
	/// A 2 character EOL will count as 1 character.
	/// *
	/// </summary>
	/// <param name="string">String to chop.
	/// </param>
	/// <param name="i">Number of characters to chop.
	/// </param>
	/// <param name="eol">A String representing the EOL (end of line).
	/// </param>
	/// <returns>String with processed answer.
	///
	/// </returns>
	public static System.String chop(System.String s, int i, System.String eol) {
	    if (i == 0 || s == null || eol == null) {
		return s;
	    }

	    int length = s.Length;

	    /*
	    * if it is a 2 char EOL and the string ends with
	    * it, nip it off.  The EOL in this case is treated like 1 character
	    */
	    if (eol.Length == 2 && s.EndsWith(eol)) {
		length -= 2;
		i -= 1;
	    }

	    if (i > 0) {
		length -= i;
	    }

	    if (length < 0) {
		length = 0;
	    }

	    return s.Substring(0, (length) - (0));
	}

	//		public static System.Text.StringBuilder stringSubstitution(System.String argStr, System.Collections.Hashtable vars)
	//		{
	//			return stringSubstitution(argStr, (Map) vars);
	//		}

	/// <summary> Perform a series of substitutions. The substitions
	/// are performed by replacing $variable in the target
	/// string with the value of provided by the key "variable"
	/// in the provided hashtable.
	/// *
	/// </summary>
	/// <param name="String">target string
	/// </param>
	/// <param name="Hashtable">name/value pairs used for substitution
	/// </param>
	/// <returns>String target string with replacements.
	///
	/// </returns>
	public static System.Text.StringBuilder stringSubstitution(System.String argStr, Hashtable vars) {
	    System.Text.StringBuilder argBuf = new System.Text.StringBuilder();

	    for (int cIdx = 0; cIdx < argStr.Length; ) {
		char ch = argStr[cIdx];

		switch (ch) {
		    case '$':
			System.Text.StringBuilder nameBuf = new System.Text.StringBuilder();
			for (++cIdx; cIdx < argStr.Length; ++cIdx) {
			    ch = argStr[cIdx];
			    if (ch == '_' || System.Char.IsLetterOrDigit(ch))
				nameBuf.Append(ch);
			    else
				break;
			}

			if (nameBuf.Length > 0) {
			    System.String value_Renamed = (System.String) vars[nameBuf.ToString()];

			    if (value_Renamed != null) {
				argBuf.Append(value_Renamed);
			    }
			}
			break;



		    default:
			argBuf.Append(ch);
			++cIdx;
			break;

		}
	    }

	    return argBuf;
	}

	/// <summary> Read the contents of a file and place them in
	/// a string object.
	/// *
	/// </summary>
	/// <param name="String">path to file.
	/// </param>
	/// <returns>String contents of the file.
	///
	/// </returns>
	public static System.String fileContentsToString(System.String file) {
	    System.String contents = "";

	    System.IO.FileInfo f = new System.IO.FileInfo(file);

	    bool tmpBool;
	    if (System.IO.File.Exists(f.FullName))
		tmpBool = true;
	    else
		tmpBool = System.IO.Directory.Exists(f.FullName);
	    if (tmpBool) {
		try {
		    System.IO.StreamReader fr = new System.IO.StreamReader(f.FullName);
		    char[] template = new char[(int) SupportClass.FileLength(f)]
		    ;
		    fr.Read((System.Char[]) template, 0, template.Length)
		    ;
		    contents = new String(template)
		    ;
		    fr.Close();
		} catch (System.Exception e) {
		    System.Console.Out.WriteLine(e);
		    SupportClass.WriteStackTrace(e, Console.Error);
		}
	    }

	    return contents;
	}

	/// <summary> Remove/collapse multiple newline characters.
	/// *
	/// </summary>
	/// <param name="String">string to collapse newlines in.
	/// </param>
	/// <returns>String
	///
	/// </returns>
	public static System.String collapseNewlines(System.String argStr) {
	    char last = argStr[0];
	    System.Text.StringBuilder argBuf = new System.Text.StringBuilder();

	    for (int cIdx = 0; cIdx < argStr.Length; cIdx++) {
		char ch = argStr[cIdx];
		if (ch != '\n' || last != '\n') {
		    argBuf.Append(ch);
		    last = ch;
		}
	    }

	    return argBuf.ToString();
	}

	/// <summary> Remove/collapse multiple spaces.
	/// *
	/// </summary>
	/// <param name="String">string to remove multiple spaces from.
	/// </param>
	/// <returns>String
	///
	/// </returns>
	public static System.String collapseSpaces(System.String argStr) {
	    char last = argStr[0];
	    System.Text.StringBuilder argBuf = new System.Text.StringBuilder();

	    for (int cIdx = 0; cIdx < argStr.Length; cIdx++) {
		char ch = argStr[cIdx];
		if (ch != ' ' || last != ' ') {
		    argBuf.Append(ch);
		    last = ch;
		}
	    }

	    return argBuf.ToString();
	}

	/// <summary> Replaces all instances of oldString with newString in line.
	/// Taken from the Jive forum package.
	/// *
	/// </summary>
	/// <param name="String">original string.
	/// </param>
	/// <param name="String">string in line to replace.
	/// </param>
	/// <param name="String">replace oldString with this.
	/// </param>
	/// <returns>String string with replacements.
	///
	/// </returns>
	public static System.String sub(System.String line, System.String oldString, System.String newString) {
	    int i = 0;
	    if ((i = line.IndexOf(oldString, i)) >= 0) {
		char[] line2 = line.ToCharArray();
		char[] newString2 = newString.ToCharArray();
		int oLength = oldString.Length;
		System.Text.StringBuilder buf = new System.Text.StringBuilder(line2.Length);
		buf.Append(line2, 0, i).Append(newString2);
		i += oLength;
		int j = i;
		while ((i = line.IndexOf(oldString, i)) > 0) {
		    buf.Append(line2, j, i - j).Append(newString2);
		    i += oLength;
		    j = i;
		}
		buf.Append(line2, j, line2.Length - j);
		return buf.ToString();
	    }
	    return line;
	}

	/// <summary> Returns the output of printStackTrace as a String.
	/// *
	/// </summary>
	/// <param name="e">A Throwable.
	/// </param>
	/// <returns>A String.
	///
	/// </returns>
	//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
	public static System.String stackTrace(System.Exception e) {
	    System.String foo = null;
	    try {
		// And show the Error Screen.
		System.IO.MemoryStream ostr = new System.IO.MemoryStream();
		System.IO.StreamWriter temp_writer;
		temp_writer = new System.IO.StreamWriter(ostr);
		temp_writer.AutoFlush = true;
		SupportClass.WriteStackTrace(e, temp_writer);
		char[] tmpChar;
		byte[] tmpByte;
		tmpByte = ostr.GetBuffer();
		tmpChar = new char[tmpByte.Length];
		tmpByte.CopyTo(tmpChar, 0);
		foo = new System.String(tmpChar);
	    } catch (System.Exception f) {
		// Do nothing.
	    }
	    return foo;
	}

	/// <summary> Return a context-relative path, beginning with a "/", that represents
	/// the canonical version of the specified path after ".." and "." elements
	/// are resolved out.  If the specified path attempts to go outside the
	/// boundaries of the current context (i.e. too many ".." path elements
	/// are present), return <code>null</code> instead.
	/// *
	/// </summary>
	/// <param name="path">Path to be normalized
	/// </param>
	/// <returns>String normalized path
	///
	/// </returns>
	public static System.String normalizePath(System.String path) {
	    // Normalize the slashes and add leading slash if necessary
	    System.String normalized = path;
	    if (normalized.IndexOf((System.Char) '\\') >= 0) {
		normalized = normalized.Replace('\\', '/');
	    }

	    if (!normalized.StartsWith("/")) {
		normalized = "/" + normalized;
	    }

	    // Resolve occurrences of "//" in the normalized path
	    while (true) {
		int index = normalized.IndexOf("//");
		if (index < 0)
		    break;
		normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 1);
	    }

	    // Resolve occurrences of "%20" in the normalized path
	    while (true) {
		int index = normalized.IndexOf("%20");
		if (index < 0)
		    break;
		normalized = normalized.Substring(0, (index) - (0)) + " " + normalized.Substring(index + 3);
	    }

	    // Resolve occurrences of "/./" in the normalized path
	    while (true) {
		int index = normalized.IndexOf("/./");
		if (index < 0)
		    break;
		normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 2);
	    }

	    // Resolve occurrences of "/../" in the normalized path
	    while (true) {
		int index = normalized.IndexOf("/../");
		if (index < 0)
		    break;
		if (index == 0)
		    return (null);
		// Trying to go outside our context
		int index2 = normalized.LastIndexOf((System.Char) '/', index - 1);
		normalized = normalized.Substring(0, (index2) - (0)) + normalized.Substring(index + 3);
	    }

	    // Return the normalized path that we have completed
	    return (normalized);
	}

	/// <summary> If state is true then return the trueString, else
	/// return the falseString.
	/// *
	/// </summary>
	/// <param name="boolean">
	/// </param>
	/// <param name="String">trueString
	/// </param>
	/// <param name="String">falseString
	///
	/// </param>
	public virtual System.String select(bool state, System.String trueString, System.String falseString) {
	    if (state) {
		return trueString;
	    } else {
		return falseString;
	    }
	}

	/// <summary> Check to see if all the string objects passed
	/// in are empty.
	/// *
	/// </summary>
	/// <param name="list">A list of {@link java.lang.String} objects.
	/// </param>
	/// <returns>    Whether all strings are empty.
	///
	/// </returns>
	public virtual bool allEmpty(IList list) {
	    int size = list.Count;

	    for (int i = 0; i < size; i++) {
		if (list[i] != null && list[i].ToString().Length > 0) {
		    return false;
		}
	    }
	    return true;
	}
    }
}
