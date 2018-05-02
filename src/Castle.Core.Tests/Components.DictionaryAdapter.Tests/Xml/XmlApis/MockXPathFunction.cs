// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System.Xml.XPath;
	using System.Xml.Xsl;

	internal class MockXPathFunction : IXsltContextFunction
	{
		private readonly object value;
		private readonly XPathResultType type;

		public MockXPathFunction(string value)
		{
			this.value = value;
			this.type  = XPathResultType.String;
		}

		public int Maxargs
		{
			get { return 0; }
		}

		public int Minargs
		{
			get { return 0; }
		}

		public XPathResultType[] ArgTypes
		{
			get { return new XPathResultType[0]; }
		}

		public XPathResultType ReturnType
		{
			get { return type; }
		}

		public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
		{
			return value;
		}
	}
}
