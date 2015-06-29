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

#if !SILVERLIGHT // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Xml;

    public class XmlStructureComparer : IEqualityComparer<XmlNode>, IComparer<XmlNode>
    {
        private readonly StringComparer comparer;

		public static readonly XmlStructureComparer
			Default = new XmlStructureComparer(StringComparer.Ordinal);

        public XmlStructureComparer(StringComparer comparer)
        {
            if (null == comparer)
                throw new ArgumentNullException("comparer");

            this.comparer = comparer;
        }

        public int GetHashCode(XmlNode node)
        {
            return Traverse<XmlNode, int>(node, 0,
				GetChildElementsAndAttributes, VisitChildForHashCode, VisitPrimitiveForHashCode);
        }

        public bool Equals(XmlNode node1, XmlNode node2)
        {
            return Traverse<Pair<XmlNode>, bool>(new Pair<XmlNode>(node1, node2), true,
                GetChildTuples, VisitChildForEquals, VisitPrimitiveForEquals);
        }

        public int Compare(XmlNode node1, XmlNode node2)
        {
            return Traverse<Pair<XmlNode>, int>(new Pair<XmlNode>(node1, node2), 0,
                GetChildTuples, VisitChildForCompare, VisitPrimitiveForCompare);
        }

        private IEnumerable<Pair<XmlNode>> GetChildTuples(Pair<XmlNode> tuple)
        {
            return ZipOuter(
                GetChildElementsAndAttributes(tuple.Item1)
					.OrderBy(n => n.LocalName,    comparer)
					.ThenBy (n => n.NamespaceURI, comparer),
                GetChildElementsAndAttributes(tuple.Item2)
					.OrderBy(n => n.LocalName,    comparer)
					.ThenBy (n => n.NamespaceURI, comparer),
                (n1, n2) => new Pair<XmlNode>(n1, n2));
        }

        private bool VisitChildForHashCode(XmlNode node, ref int hashCode)
        {
			var part0 = comparer.GetHashCode(node.LocalName);
			var part1 = comparer.GetHashCode(node.NamespaceURI);

            unchecked
            {
                hashCode = (hashCode << 3 | hashCode >> 29) ^ part0;
				hashCode = (hashCode << 7 | hashCode >> 25) ^ part1;
            }
            return false;
        }

        private bool VisitChildForEquals(Pair<XmlNode> tuple, ref bool equal)
        {
            return !
            (
                equal =
                    (null != tuple.Item1) &&
                    (null != tuple.Item2) &&
                    comparer.Equals(tuple.Item1.LocalName,    tuple.Item2.LocalName   ) &&
                    comparer.Equals(tuple.Item1.NamespaceURI, tuple.Item2.NamespaceURI)
            );
        }

        private bool VisitChildForCompare(Pair<XmlNode> tuple, ref int result)
        {
			int r;
            return 0 !=
            (
                result =
                    (null == tuple.Item1) ? -1 :
                    (null == tuple.Item2) ? +1 :
                    0 != (r = comparer.Compare(tuple.Item1.LocalName,    tuple.Item2.LocalName   )) ? r :
					0 != (r = comparer.Compare(tuple.Item1.NamespaceURI, tuple.Item2.NamespaceURI)) ? r :
					0
            );
        }

        private bool VisitPrimitiveForHashCode(XmlNode node, ref int hashCode)
        {
            var text = (null == node) ? string.Empty : node.InnerText;
            var part = comparer.GetHashCode(text);

            unchecked
            {
                hashCode ^= part;
            }
            return false;
        }

        private bool VisitPrimitiveForEquals(Pair<XmlNode> tuple, ref bool equal)
        {
            var text1 = (null == tuple.Item1) ? string.Empty : tuple.Item1.InnerText;
            var text2 = (null == tuple.Item2) ? string.Empty : tuple.Item2.InnerText;

            return !
            (
                // True => Primitive content differs
                equal = comparer.Equals(text1, text2)
            );
        }

        private bool VisitPrimitiveForCompare(Pair<XmlNode> tuple, ref int result)
        {
            var text1 = (null == tuple.Item1) ? string.Empty : tuple.Item1.InnerText;
            var text2 = (null == tuple.Item2) ? string.Empty : tuple.Item2.InnerText;

            return 0 !=
            (
                // !0 => Primitive content differs
                result = comparer.Compare(text1, text2)
            );
        }

        private delegate bool Visitor<TElement, TResult>(TElement element, ref TResult result);

        private static TResult Traverse<TElement, TResult>(
            TElement startElement,
            TResult defaultResult,
            Func<TElement, IEnumerable<TElement>> childSelector,
            Visitor<TElement, TResult> childVisitor,
            Visitor<TElement, TResult> primitiveVisitor)
        {
            var elements = new Queue<TElement>();
            var element = startElement;
            var result = defaultResult;

            for (;;)
            {
                var complex = false;
                var children = childSelector(element);

                foreach (var child in children)
                {
                    if (childVisitor(child, ref result))
                        return result;

                    elements.Enqueue(child);

                    if (!complex)
                        complex = true;
                }

                if (!complex && primitiveVisitor(element, ref result))
                    return result;

                if (!elements.Any())
                    return result;

                element = elements.Dequeue();
            }
        }

        private static IEnumerable<XmlNode> GetChildElementsAndAttributes(XmlNode node)
        {
            return
                (null == node)
                    ? Enumerable.Empty<XmlNode>() :
                (node.NodeType == XmlNodeType.Element)
                    ? GetElementChildNodes(node) :
                (node.NodeType == XmlNodeType.Document)
                    ? GetDocumentChildNodes((XmlDocument) node)
                    : Enumerable.Empty<XmlNode>();
        }


        private static IEnumerable<XmlNode> GetElementChildNodes(XmlNode node)
        {
            return Enumerable.Concat(
                (IEnumerable<XmlNode>) node.Attributes.OfType<XmlAttribute>().Where(a => !IsNamespace(a))
#if DOTNET35
				.Cast<XmlNode>()
#endif
				,
                (IEnumerable<XmlNode>) node.ChildNodes.OfType<XmlElement>()
#if DOTNET35
				.Cast<XmlNode>()
#endif
);
        }

        private static IEnumerable<XmlNode> GetDocumentChildNodes(XmlDocument document)
        {
            var element = document.DocumentElement;
            if (null == element) yield break;
            yield return element;
        }

		private static bool IsNamespace(XmlAttribute attribute)
		{
			return attribute.LocalName == "xmlns"
				|| attribute.Prefix    == "xmlns";
		}

		[DebuggerStepThrough]
		private static IEnumerable<TResult> ZipOuter<TSource1, TSource2, TResult>(
            IEnumerable<TSource1> source1,
			IEnumerable<TSource2> source2,
            Func<TSource1, TSource2, TResult> selector)
        {
            using (var enumerator1 = source1.GetEnumerator())
            using (var enumerator2 = source2.GetEnumerator())
            {
                for (;;)
                {
                    var hasItem1 = enumerator1.MoveNext();
                    var hasItem2 = enumerator2.MoveNext();
                    if (hasItem1 == false && hasItem2 == false)
						yield break;

                    var item1 = hasItem1 ? enumerator1.Current : default(TSource1);
                    var item2 = hasItem2 ? enumerator2.Current : default(TSource2);
                    yield return selector(item1, item2);
                }
            }
        }

		private struct Pair<T>
		{
			public readonly T Item1;
			public readonly T Item2;

			public Pair(T item1, T item2)
			{
				Item1 = item1;
				Item2 = item2;
			}
		}
    }
}
#endif
