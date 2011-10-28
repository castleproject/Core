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

namespace Castle.Components.DictionaryAdapter.Xml
{
	public class XmlPositionComparer
	{
		public static readonly XmlPositionComparer
			Instance = new XmlPositionComparer();

		public bool Equals(IXmlNode nodeA, IXmlNode nodeB)
		{
			var comparer = XmlNameComparer.Default;
			var a = new ComparandIterator { Node = nodeA };
			var b = new ComparandIterator { Node = nodeB };

			for (;;)
			{
				var hasA = a.MoveNext();
				var hasB = b.MoveNext();

				if (hasA && hasB)
					if (comparer.Equals(a.Name, b.Name)) continue;
					else return false;
				else if (hasA || hasB)
					return false;
				else // (!hasA && !hasB)
					return a.Node.UnderlyingPositionEquals(b.Node);
			}
		}

		private struct ComparandIterator
		{
			public IXmlNode          Node;
			public CompiledXPathNode Path;
			public XmlName           Name;

			public bool MoveNext()
			{
				if (Path != null)
					return ConsumePath();
				else if (Node == null || Node.Exists)
					return Fail();
				else
					return ConsumeNode();
			}

			private bool ConsumePath()
			{
				Name = new XmlName
				(
					Path.LocalName,
					Node.LookupNamespaceUri(Path.Prefix)
				);
				Path = Path.PreviousNode;
				return true;
			}

			private bool ConsumeNode()
			{
				Path = GetXPathLastStep(Node);
				if (Path != null)
					ConsumePath();
				else
					Name = Node.Name;
				Node = Node.Parent;
				return true;
			}

			private bool Fail()
			{
				Name = XmlName.Empty;
				return false;
			}

			private static CompiledXPathNode GetXPathLastStep(IXmlNode node)
			{
				var source = node as IHasXPath;
				if (source == null)
					return null;

				var path = source.Path;
				return path.IsCreatable
					? path.LastStep
					: null;
			}
		}
	}

	public interface IHasXPath
	{
		CompiledXPath Path { get; }
	}
}
