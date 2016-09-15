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

#if FEATURE_DICTIONARYADAPTER_XML
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
				if (a.Node.IsReal && b.Node.IsReal)
					return a.Node.UnderlyingPositionEquals(b.Node);
				if (!a.MoveNext() || !b.MoveNext())
					return false;
				if (!comparer.Equals(a.Name, b.Name))
					return false;
			}
		}

		private struct ComparandIterator
		{
			public IXmlNode Node;
			public XmlName  Name;
			public CompiledXPathNode Step;

			public bool MoveNext()
			{
				return
					Step != null ? ConsumeStep() :
					Node != null ? ConsumeNode() :
					Stop();
			}

			private bool ConsumeNode()
			{
				var result = true;
				var path = Node.Path;
				if (path != null)
					result = ConsumeFirstStep(path);
				else
					Name = Node.Name;

				Node = Node.Parent;
				return result;
			}

			private bool Stop()
			{
				Name = XmlName.Empty;
				return false;
			}

			private bool ConsumeFirstStep(CompiledXPath path)
			{
				if (!path.IsCreatable)
					return false;

				Step = path.LastStep;
				return ConsumeStep();
			}

			private bool ConsumeStep()
			{
				Name = new XmlName
				(
					Step.LocalName,
					Node.LookupNamespaceUri(Step.Prefix)
				);

				Step = Step.PreviousNode;
				return true;
			}
		}
	}
}
#endif
