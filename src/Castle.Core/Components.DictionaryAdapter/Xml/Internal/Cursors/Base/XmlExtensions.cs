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
	using System;
	using System.Xml;

	internal static class XmlExtensions
	{
		public static bool PositionEquals(this IXmlNode nodeA, IXmlNode nodeB)
		{
			return XmlPositionComparer.Instance.Equals(nodeA, nodeB);
		}

		public static void CopyTo(this IXmlNode source, IXmlNode target)
		{
			using (var reader = source.ReadSubtree())
			{
				if (!reader.Read())
					return;

				using (var writer = target.WriteAttributes())
					writer.WriteAttributes(reader, false);

				if (!reader.Read())
					return;

				using (var writer = target.WriteChildren())
					do writer.WriteNode(reader, false);
					while (!(reader.EOF || reader.NodeType == XmlNodeType.EndElement));
			}
		}
	}
}
#endif
