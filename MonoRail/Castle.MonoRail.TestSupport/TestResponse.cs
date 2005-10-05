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


	[Serializable]
	public class TestResponse// : MarshalByRefObject
	{
		private NameValueCollection headers = new NameValueCollection();
		private int statusCode;
		private String statusDescription;

		public TestResponse()
		{
		}

		public int StatusCode
		{
			get { return statusCode; }
			set { statusCode = value; }
		}

		public string StatusDescription
		{
			get { return statusDescription; }
			set { statusDescription = value; }
		}

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		protected internal void Complete()
		{
			
		}
	}
}
