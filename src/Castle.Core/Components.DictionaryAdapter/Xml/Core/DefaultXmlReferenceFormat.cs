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
	using System.Globalization;

	public sealed class DefaultXmlReferenceFormat : IXmlReferenceFormat
	{
		public static readonly DefaultXmlReferenceFormat
			Instance = new DefaultXmlReferenceFormat();

		private DefaultXmlReferenceFormat() { }

		public bool TryGetIdentity(IXmlNode node, out int id)
		{
			var text = node.GetAttribute(XRef.Id);
			return int.TryParse(text, IntegerStyle, Culture, out id);
		}

		public bool TryGetReference(IXmlNode node, out int id)
		{
			var text = node.GetAttribute(XRef.Ref);
			return int.TryParse(text, IntegerStyle, Culture, out id);
		}

		public void SetIdentity(IXmlNode node, int id)
		{
			node.SetAttribute(XRef.Id, id.ToString(Culture));
		}

		public void SetReference(IXmlNode node, int id)
		{
			node.SetAttribute(XRef.Ref, id.ToString(Culture));
		}

		public void ClearIdentity(IXmlNode node)
		{
			node.SetAttribute(XRef.Id, null);
		}

		public void ClearReference(IXmlNode node)
		{
			node.SetAttribute(XRef.Ref, null);
		}

		private const NumberStyles
			IntegerStyle = NumberStyles.Integer;

		private static readonly IFormatProvider
			Culture = CultureInfo.InvariantCulture;
	}
}
#endif
