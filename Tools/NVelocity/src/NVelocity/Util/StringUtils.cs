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

namespace NVelocity.Util
{
	using System;
	using System.IO;

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
	public class StringUtils
	{
		/// <summary> Line separator for the OS we are operating on.
		/// </summary>
		private static readonly String EOL = Environment.NewLine;

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
		public static String FirstLetterCaps(String data)
		{
			String firstLetter = data.Substring(0, (1) - (0)).ToUpper();
			String restLetters = data.Substring(1).ToLower();
			return firstLetter + restLetters;
		}

		/// <summary> Read the contents of a file and place them in
		/// a string object.
		/// *
		/// </summary>
		/// <param name="file">path to file.
		/// </param>
		/// <returns>String contents of the file.
		///
		/// </returns>
		public static String FileContentsToString(String file)
		{
			String contents = string.Empty;

			FileInfo f = new FileInfo(file);

			bool tmpBool;
			if (File.Exists(f.FullName))
			{
				tmpBool = true;
			}
			else
			{
				tmpBool = Directory.Exists(f.FullName);
			}
			if (tmpBool)
			{
				try
				{
					StreamReader fr = new StreamReader(f.FullName);
					char[] template = new char[(int) SupportClass.FileLength(f)];
					fr.Read((Char[]) template, 0, template.Length);
					contents = new String(template);
					fr.Close();
				}
				catch(Exception e)
				{
					Console.Out.WriteLine(e);
					SupportClass.WriteStackTrace(e, Console.Error);
				}
			}

			return contents;
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
		public static String NormalizePath(String path)
		{
			// Normalize the slashes and add leading slash if necessary
			string normalized = path;
			if (normalized.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
			{
				normalized = normalized.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			if (!normalized.StartsWith(Path.DirectorySeparatorChar.ToString()))
			{
				normalized = Path.DirectorySeparatorChar + normalized;
			}

			// Resolve occurrences of "//" in the normalized path
			while(true)
			{
				int index = normalized.IndexOf("//");
				if (index < 0) break;
				normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 1);
			}

			// Resolve occurrences of "%20" in the normalized path
			while(true)
			{
				int index = normalized.IndexOf("%20");
				if (index < 0) break;
				normalized = string.Format("{0} {1}", normalized.Substring(0, (index) - (0)), normalized.Substring(index + 3));
			}

			// Resolve occurrences of "/./" in the normalized path
			while(true)
			{
				int index = normalized.IndexOf("/./");
				if (index < 0)
				{
					break;
				}
				normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 2);
			}

			// Resolve occurrences of "/../" in the normalized path
			while(true)
			{
				int index = normalized.IndexOf("/../");
				if (index < 0)
				{
					break;
				}
				if (index == 0)
				{
					return (null);
				}
				// Trying to go outside our context
				int index2 = normalized.LastIndexOf((Char) '/', index - 1);
				normalized = normalized.Substring(0, (index2) - (0)) + normalized.Substring(index + 3);
			}

			// Return the normalized path that we have completed
			return (normalized);
		}
	}
}