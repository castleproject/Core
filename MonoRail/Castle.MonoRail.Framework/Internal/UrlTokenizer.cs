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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Extracts the information from a Url Path into area, controller name and action.
	/// </summary>
	public sealed class UrlTokenizer
	{
		public static UrlInfo ExtractInfo( String url, String virtualDirectory )
		{
			if ( url == null || url.Length == 0 )
			{
				throw new UrlTokenizerException( "Invalid url" );
			}

			if ( url[0] == '/' )
			{
				url = url.ToLower().Substring(1);
			}

			// Strip the virtualDirectory from the Url
			if ( virtualDirectory != null && virtualDirectory != "" )
			{
				virtualDirectory = virtualDirectory.ToLower().Substring(1);

				if ( !url.StartsWith(virtualDirectory) )
				{
					throw new UrlTokenizerException( "Invalid url" );
				}

				url = url.Substring( virtualDirectory.Length );
			}

			String[] parts = url.Split( '/' );

			if ( parts.Length < 2 )
			{
				throw new UrlTokenizerException( "Invalid url" );
			}

			String action = parts[ parts.Length - 1 ];
			
			int fileNameIndex = action.IndexOf( '.' );

			if ( fileNameIndex != -1 )
			{
				action = action.Substring( 0, fileNameIndex );
			}

			String controller = parts[ parts.Length - 2 ];

			String area = String.Empty;
			
			if ( parts.Length - 3 == 0 )
			{
				area = parts[ parts.Length - 3 ];
			}
			else if ( parts.Length - 3 > 0 ) 
			{
				StringBuilder areaSB = new StringBuilder();
				for ( int i=0; i <= parts.Length - 3; i++ )
					if ( parts[i] != null && parts[i].Length > 0 )
						areaSB.Append( parts[i] ).Append( '/' );
				if ( areaSB.Length > 0 ) 
					areaSB.Length -= 1;
				area = areaSB.ToString();
			}

			String extension = GetExtension( url );

			return new UrlInfo( url, area, controller, action, extension );
		}

		/// <summary>
		/// Gets the extension of the requested urls page without the preceding period.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <returns>The page extensino of the provided URL.</returns>
		public static String GetExtension( String url )
		{
			return Path.GetExtension( url ).Substring( 1 );
		}
	}
}
