// Copyright 2004-2005 Castle Proje\ct - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Adapters
{
	using System;
	using System.IO;
	using System.Web;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	public class ResponseAdapter : IResponse
	{
		private readonly String _appPath;
		private readonly String _extension;
		private readonly HttpResponse _response;
		private bool _redirected;

		public ResponseAdapter( HttpResponse response, String url, String appPath )
		{
			_response = response;
			_extension = UrlTokenizer.GetExtension( url );
			_appPath = appPath;
		}

		/// <summary>
		/// Gets the caching policy (expiration time, privacy, 
		/// vary clauses) of a Web page.
		/// </summary>
		public HttpCachePolicy CachePolicy
		{
			get { return _response.Cache; }
		}

		/// <summary>
		/// Sets the Cache-Control HTTP header to Public or Private.
		/// </summary>
		public String CacheControlHeader
		{
			get { return _response.CacheControl; }
			set { _response.CacheControl = value; }
		}

		/// <summary>
		/// Gets or sets the HTTP character set of the output stream.
		/// </summary>
		public String Charset
		{
			get { return _response.Charset; }
			set { _response.Charset = value; }
		}

		public int StatusCode
		{
			get { return _response.StatusCode; }
			set { _response.StatusCode = value; }
		}

		public String ContentType
		{
			get { return _response.ContentType; }
			set { _response.ContentType = value; }
		}

		public void AppendHeader( String name, String headerValue )
		{
			_response.AppendHeader( name, headerValue );
		}

		public System.IO.TextWriter Output
		{
			get { return _response.Output; }
		}

		public System.IO.Stream OutputStream
		{
			get { return _response.OutputStream; }
		}

		public void BinaryWrite( byte[] buffer )
		{
			_response.BinaryWrite( buffer );
		}

		public void BinaryWrite( Stream stream )
		{
			byte[] buffer = new byte[ stream.Length ];

			stream.Read( buffer, 0, buffer.Length );

			BinaryWrite( buffer );
		}

		public void Clear()
		{
			_response.Clear();
		}

		public void ClearContent()
		{
			_response.ClearContent();
		}

		public void Write( String s )
		{
			_response.Write(s);
		}

		public void Write( object obj )
		{
			_response.Write( obj );
		}

		public void Write( char ch )
		{
			_response.Write( ch );
		}

		public void Write( char[] buffer, int index, int count )
		{
			_response.Write( buffer, index, count );
		}

		public void WriteFile( String fileName )
		{
			_response.WriteFile( fileName );
		}

		public void Redirect( String url )
		{
			_redirected = true;
			
			_response.Redirect( url, false );
		}

		public void Redirect( String url, bool endProcess )
		{
			_redirected = true;
			
			_response.Redirect( url, endProcess );
		}

		public void Redirect( String controller, String action )
		{
			_redirected = true;
			
			_response.Redirect( 
				UrlInfo.CreateAbsoluteRailsUrl( _appPath, controller, action, _extension ), false );
		}

		public void Redirect( String area, String controller, String action )
		{
			_redirected = true;

			if ( area == null || area.Length == 0 )
			{
				_response.Redirect(
					UrlInfo.CreateAbsoluteRailsUrl( _appPath, controller, action, _extension ), false );
			}
			else
			{
				_response.Redirect( 
					UrlInfo.CreateAbsoluteRailsUrl( _appPath, area, controller, action, _extension ), false );
			}
		}

		public bool WasRedirected
		{
			get { return _redirected; }
		}

		public void CreateCookie( String name, String cookieValue )
		{
			CreateCookie( new HttpCookie( name, cookieValue ) );
		}

		public void CreateCookie( String name, String cookieValue, DateTime expiration )
		{
			HttpCookie cookie = new HttpCookie( name, cookieValue );
			
			cookie.Expires = expiration;
			cookie.Path = "/";

			CreateCookie( cookie );
		}

		public void CreateCookie( HttpCookie cookie )
		{
			_response.Cookies.Add( cookie );
		}
	}
}
