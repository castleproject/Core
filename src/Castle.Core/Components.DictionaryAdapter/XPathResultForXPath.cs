// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Xml;

	public class XPathResultForXPath : XPathResult
	{
		//private XPathAttribute attrib;

		private readonly XPathNavigator root;
		private readonly ICompiledPath path;

		public XPathResultForXPath(PropertyDescriptor property, string key, XPathContext context, XPathAttribute attrib, XPathNavigator root)
			: base(property, key, null, context, attrib, null)
		{
			object result;

			this.root   = root;
			this.path   = attrib.Path;
			base.create = InternalCreate;

			Context.TryEvaluate(attrib.Path.Expression, root, out result);

			Result = result;
			CanWrite = true; // result is XPathNavigator;
		}

		private XPathNavigator InternalCreate()
		{
			var iterator = new XPathMutableIterator(root, path, false);

			if (!iterator.MoveNext())
				iterator.Create();
			return iterator.Current;

//			return Context.AppendElement(Key, null, root);
		}

		public override bool GetNavigator(bool demand, bool nillable, out XPathNavigator result)
		{
			return base.GetNavigator(demand, nillable, out result);
		}
	}
#endif
}
