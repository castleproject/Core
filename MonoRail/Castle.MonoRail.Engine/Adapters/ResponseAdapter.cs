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

namespace Castle.MonoRail.Engine.Adapters
{
	using System;
	using System.Web;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	public class ResponseAdapter : IResponse
	{
		private readonly string _appPath;
		private readonly string _extension;
		private HttpResponse _response;

		public ResponseAdapter(HttpResponse response, string url, string appPath)
		{
			_response = response;
			_extension = UrlTokenizer.GetExtension(url);
			_appPath = appPath;
		}

		#region IResponse

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

		public void AppendHeader(String name, String value)
		{
			_response.AppendHeader(name, value);
		}

		public System.IO.TextWriter Output
		{
			get { return _response.Output; }
		}

		public System.IO.Stream OutputStream
		{
			get { return _response.OutputStream; }
		}

		public void Write(String s)
		{
			_response.Write(s);
		}

		public void Write(object obj)
		{
			_response.Write(obj);
		}

		public void Write(char ch)
		{
			_response.Write(ch);
		}

		public void Write(char[] buffer, int index, int count)
		{
			_response.Write(buffer, index, count);
		}

		public void WriteFile(string fileName)
		{
			_response.WriteFile(fileName);
		}

		public void Redirect(String url)
		{
			_response.Redirect(url, false);
		}

		public void Redirect(String url, bool endProcess)
		{
			_response.Redirect(url, endProcess);
		}

		public void Redirect(String controller, String action)
		{
			_response.Redirect( 
				UrlInfo.CreateAbsoluteRailsUrl(_appPath, controller, action, _extension), false );
		}

		public void Redirect(String area, String controller, String action)
		{
			if (area == null || area.Length == 0)
			{
				_response.Redirect(
					UrlInfo.CreateAbsoluteRailsUrl(_appPath, controller, action, _extension), false);
			}
			else
			{
				_response.Redirect( 
					UrlInfo.CreateAbsoluteRailsUrl(_appPath, area, controller, action, _extension), false );
			}
		}

		public void CreateCookie(String name, String value)
		{
			_response.Cookies.Add( new HttpCookie(name, value) );
		}

		public void CreateCookie(String name, String value, DateTime expiration)
		{
			HttpCookie cookie = new HttpCookie(name, value);
			cookie.Expires = expiration;
			cookie.Path = "/";

			_response.Cookies.Add( cookie );
		}

		#endregion
	}
}
