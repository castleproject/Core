using System.Collections;
using System.Collections.Specialized;
// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Engine.Adapters
{
	using System;
	using System.Web;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for RequestAdapter.
	/// </summary>
	public class RequestAdapter : IRequest
	{
		private HttpRequest _request;

		public RequestAdapter(HttpRequest request)
		{
			_request = request;
		}

		public byte[] BinaryRead(int count)
		{
			throw new NotImplementedException();
		}

		public String this[String key]
		{
			get { throw new NotImplementedException(); }
		}

		public IDictionary Files
		{
			get { throw new NotImplementedException(); }
		}

		public NameValueCollection Params
		{
			get { return _request.Params; }
		}
	}
}
