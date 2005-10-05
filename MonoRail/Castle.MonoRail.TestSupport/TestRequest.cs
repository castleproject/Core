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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Collections.Specialized;


	public class TestRequest : MarshalByRefObject
	{
		private String protocol = "HTTP/1.0";
		private String verb = "GET";
		private String url;
		private String virtualPath;
		private String[] queryStringParams;
		private String[] postParams;
		private String remoteAddress = "127.0.0.1";
		private String localAddress = "127.0.0.1";
		private int remotePort = 81;
		private int localPort = 0;
		private IntPtr userToken = IntPtr.Zero;
		private NameValueCollection headers = new NameValueCollection();
		private NameValueCollection serverVariables = new NameValueCollection();

		public TestRequest()
		{
		}

		public string Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}

		public NameValueCollection Headers
		{
			get { return headers; }
			set { headers = value; }
		}

		public NameValueCollection ServerVariables
		{
			get { return serverVariables; }
			set { serverVariables = value; }
		}

		public string LocalAddress
		{
			get { return localAddress; }
			set { localAddress = value; }
		}

		public int RemotePort
		{
			get { return remotePort; }
			set { remotePort = value; }
		}

		public int LocalPort
		{
			get { return localPort; }
			set { localPort = value; }
		}

		public IntPtr UserToken
		{
			get { return userToken; }
			set { userToken = value; }
		}

		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		public string Verb
		{
			get { return verb; }
			set { verb = value; }
		}

		public string RemoteAddress
		{
			get { return remoteAddress; }
			set { remoteAddress = value; }
		}

		public string VirtualPath
		{
			get { return virtualPath; }
			set { virtualPath = value; }
		}

		public string[] QueryStringParams
		{
			get { return queryStringParams; }
			set { queryStringParams = value; }
		}

		public string[] PostParams
		{
			get { return postParams; }
			set { postParams = value; }
		}
	}
}
