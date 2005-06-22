// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.IO;

	/// <summary>
	/// Extracts the information from a Url Path into
	/// area, controller name and action.
	/// </summary>
	public sealed class UrlTokenizer
	{
		public static UrlInfo ExtractInfo( String url, String virtualDirectory )
		{
			if ( url == null || url.Length == 0 || url[0] != '/' )
			{
				throw new UrlTokenizerException("Invalid url");
			}

			url = url.ToLower().Substring(1);

			// Strip the virtualDirectory from the Url
			if (virtualDirectory != null && virtualDirectory != "")
			{
				virtualDirectory = virtualDirectory.ToLower().Substring(1);

				if (!url.StartsWith(virtualDirectory))
				{
					throw new UrlTokenizerException("Invalid url");
				}

				url = url.Substring( virtualDirectory.Length );
			}

			String[] parts = url.Split('/');

			if ( url.Length < 2 )
			{
				throw new UrlTokenizerException("Invalid url");
			}

			String action = parts[ parts.Length - 1 ];
			
			int fileNameIndex = action.IndexOf('.');

			if (fileNameIndex != -1)
			{
				action = action.Substring(0, fileNameIndex);
			}

			String controller = parts[ parts.Length - 2 ];

			String area = String.Empty;
			
			if (parts.Length - 3 >= 0 )
			{
				area = parts[ parts.Length - 3 ];
			}

			string extension = GetExtension(url);

			return new UrlInfo(url, area, controller, action, extension);
		}

		public static string GetExtension(string url)
		{
			String extension = Path.GetExtension(url);
			extension = extension.Substring(1);

			return extension;
		}
	}
}
