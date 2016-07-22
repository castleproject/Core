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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public static class XPathExtensions
	{
		public static XPathNavigator CreateNavigatorSafe(this IXPathNavigable source)
		{
            if (source == null)
                throw Error.ArgumentNull("source");
			return source.CreateNavigator();
		}

		public static bool MoveToLastChild(this XPathNavigator navigator)
		{
			if (!navigator.MoveToFirstChild())
				return false;

			while (navigator.MoveToNext()) { }

			return true;
		}

		public static bool MoveToLastAttribute(this XPathNavigator navigator)
		{
			if (!navigator.MoveToFirstAttribute())
				return false;

			while (navigator.MoveToNextAttribute()) { }

			return true;
		}

		public static XPathNavigator GetRootElement(this XPathNavigator navigator)
		{
			navigator = navigator.Clone();
			navigator.MoveToRoot();
			if (navigator.NodeType == XPathNodeType.Root)
				if (!navigator.MoveToFirstChild())
					throw Error.InvalidOperation();
			return navigator;
		}

		public static XPathNavigator GetParent(this XPathNavigator navigator)
		{
			navigator = navigator.Clone();
			if (!navigator.MoveToParent())
				throw Error.InvalidOperation();
			return navigator;
		}

		public static void DeleteChildren(this XPathNavigator node)
		{
			while (node.MoveToFirstChild())
				node.DeleteSelf();
			while (node.MoveToFirstAttribute())
				node.DeleteSelf();
		}
	}
}
#endif
