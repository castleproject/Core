// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Model.Resource
{
	using System;
	using System.Text;

	[Serializable]
	public sealed class CustomUri
	{
		public static readonly String SchemeDelimiter = "://";
		public static readonly String UriSchemeFile = "file";
		public static readonly String UriSchemeAssembly = "assembly";

		private String scheme, host, path;
		private bool isUnc, isFile, isAssembly;

		public CustomUri(String resourceIdentifier)
		{
			SanityCheck(resourceIdentifier);

			ParseIdentifier(resourceIdentifier);
		}

		public bool IsUnc
		{
			get { return isUnc; }
		}

		public bool IsFile
		{
			get { return isFile; }
		}

		public bool IsAssembly
		{
			get { return isAssembly; }
		}

		public String AbsolutePath
		{
			get { throw new NotImplementedException(); }
		}

		public string Scheme
		{
			get { return scheme; }
		}

		public string Host
		{
			get { return host; }
		}

		public String Path
		{
			get { return path; }
		}

		private void ParseIdentifier(String identifier)
		{
			int comma = identifier.IndexOf(':');

			if (comma == -1 && !(identifier[0] == '\\' && identifier[1] == '\\'))
			{
				throw new ArgumentException("Invalid Uri: no scheme delimiter found on " + identifier);
			}

			bool translateSlashes = true;

			if (identifier[0] == '\\' && identifier[1] == '\\')
			{
				// Unc

				isUnc = true;
				isFile = true;
				scheme = UriSchemeFile;
				translateSlashes = false;
			}
			else if (identifier[comma + 1] == '/' && identifier[comma + 2] == '/')
			{
				// Extract scheme

				scheme = identifier.Substring(0, comma);

				isFile = (scheme == UriSchemeFile);
				isAssembly = (scheme == UriSchemeAssembly);

				identifier = identifier.Substring(comma + SchemeDelimiter.Length);
			}
			else
			{
				isFile = true;
				scheme = UriSchemeFile;
			}

			StringBuilder sb = new StringBuilder();

			foreach(char ch in identifier.ToCharArray())
			{
				if (translateSlashes && (ch == '\\' || ch == '/'))
				{
					if (host == null && !IsFile)
					{
						host = sb.ToString();
						sb.Length = 0;
					}

					sb.Append('/');
				}
				else
				{
					sb.Append(ch);
				}
			}

			path = sb.ToString();
		}

		private static void SanityCheck(String resourceIdentifier)
		{
			if (resourceIdentifier == null)
			{
				throw new ArgumentNullException("resourceIdentifier");
			}
			if (resourceIdentifier == String.Empty)
			{
				throw new ArgumentException("Empty resource identifier is not allowed", "resourceIdentifier");
			}
		}
	}
}