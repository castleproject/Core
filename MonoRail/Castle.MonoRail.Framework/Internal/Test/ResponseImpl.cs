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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Specialized;
	
	public class ResponseImpl : IResponse
	{
		private int _statusCode;
		private StringWriter _writer;
		private NameValueCollection _headers = new NameValueCollection();
		
		internal StringBuilder _contents = new StringBuilder();

		public ResponseImpl()
		{
			_writer = new StringWriter(_contents);
		}

		#region IResponse

		public int StatusCode
		{
			get { return _statusCode; }
			set { _statusCode = value; }
		}

		public String ContentType
		{
			get
			{
				string[] headers = _headers.GetValues("Content-Type");
				if (headers != null && headers.Length == 1)
				{
					return headers[0];
				}
				else
				{
					return "text/html";
				}
			}
			set 
			{
				if (_headers.GetValues("Content-Type") != null && _headers.GetValues("Content-Type").Length != 0)
				{
					_headers.Remove("Content-Type");
				}
				_headers.Add("Content-Type", value);
			}
		}

		public void AppendHeader(String name, String value)
		{
			_headers.Add(name, value);
		}

		public System.IO.TextWriter Output
		{
			get { return _writer; }
		}

		public System.IO.Stream OutputStream
		{
			get { throw new NotImplementedException(); }
		}

		public void Write(String s)
		{
			_writer.Write(s);
		}

		public void Write(object obj)
		{
			_writer.Write(obj);
		}

		public void Write(char ch)
		{
			_writer.Write(ch);
		}

		public void Write(char[] buffer, int index, int count)
		{
			_writer.Write(buffer, index, count);
		}

		public void WriteFile(string fileName)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String url)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String url, bool endProcess)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String controller, String action)
		{
			throw new NotImplementedException();
		}

		public void Redirect(String area, String controller, String action)
		{
			throw new NotImplementedException();
		}

		public void CreateCookie(String name, String value)
		{
			throw new NotImplementedException();
		}

		public void CreateCookie(String name, String value, DateTime expiration)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
