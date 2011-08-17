// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Xml.XPath;

	internal static class XPath
	{
		//public static string Combine(string path, string subpath)
		//{
		//    if (null == path)
		//        throw new ArgumentNullException("path");
		//    if (null == subpath)
		//        throw new ArgumentNullException("subpath");
		//    if (subpath.Length == 0 || subpath == Separator)
		//        return path;
		//    if (path.Length == 0 || subpath.StartsWith(Separator))
		//        return subpath;
		//    return subpath.StartsWith(SelfToken)
		//        ? string.Concat(path, subpath.Remove(0, SelfToken.Length))
		//        : string.Concat(path, Separator, subpath);
		//}

		public static bool IsNCName(string part)
		{
			return part.Length > 0
				&& XmlConvert.IsStartNCNameChar(part[0])
				&& part.Skip(1).All(XmlConvert.IsNCNameChar);
		}

		public static bool IsCollectionItem(string path)
		{
			return path.Length > 3
				&& path[0]               == WildcardChar
				&& path[1]               == PredicateStartChar
				&& path[path.Length - 1] == PredicateEndChar;
		}

		public static bool MoveToLastChild(this XPathNavigator navigator)
		{
			var uri = navigator.NamespaceURI;

			if (!navigator.MoveToFirstChild())
				return false;

			while (navigator.MoveToNext()) { }

			return true;
		}

		public static XmlElement GetXmlElement(this XPathNavigator navigator)
		{
			var obj = (IHasXmlNode) navigator;
			return (XmlElement) obj.GetNode();
		}

		public static void SetToNil(this XPathNavigator node)
		{
			node.DeleteChildren();
			node.CreateAttribute(XPath.XsiPrefix, "nil", XPath.XsiNamespaceUri, "true");
		}

		public static void DeleteChildren(this XPathNavigator node)
		{
			if (node.HasChildren)
			{
				var n = node.Clone();
				while (n.MoveToFirstChild())
					n.DeleteSelf();
			}
			if (node.HasAttributes)
			{
				var n = node.Clone();
				while (n.MoveToFirstAttribute())
					n.DeleteSelf();
			}
		}

		public const char
			SeparatorChar      = '/',
			WildcardChar       = '*',
			PredicateStartChar = '[',
			PredicateEndChar   = ']',
			SingleQuoteChar    = '\'',
			DoubleQuoteChar    = '\"';

		public const string
			XmlnsPrefix       = "xmlns",
			XmlnsNamespaceUri = "http://www.w3.org/2000/xmlns/",
			XsiPrefix         = "xsi",
			XsiNamespaceUri   = "http://www.w3.org/2001/XMLSchema-instance",
			Separator         = "/",
			SelfToken         = ".";
	}
#endif
}
