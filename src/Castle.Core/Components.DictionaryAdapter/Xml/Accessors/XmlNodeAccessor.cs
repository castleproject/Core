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
	using System;

	public abstract class XmlNodeAccessor : XmlAccessor, IXmlKnownTypeMap, IXmlKnownType
	{
		protected XmlNodeAccessor(Type type, IXmlKnownTypeMap knownTypes)
			: base(type, knownTypes) { }

		Type IXmlKnownTypeMap.BaseType
		{
			get { return ClrType; }
		}

		public abstract string LocalName
		{
			get;
		}

		public virtual string NamespaceUri
		{
			get { return null; }
		}

		string IXmlKnownType.XsiType
		{
			get { return null; }
		}

		public override IXmlKnownTypeMap KnownTypes
		{
			get { return this; }
		}

		protected virtual bool IsMatch(IXmlNode node)
		{
			return node.HasNameLike(LocalName, NamespaceUri);
		}

		protected virtual bool IsMatch(Type type)
		{
			return type == ClrType
				|| (Serializer.IsCollection && ClrType.IsAssignableFrom(type));
		}

		bool IXmlKnownTypeMap.TryRecognizeType(IXmlNode node, out Type type)
		{
			return IsMatch(node)
				? Try.Success(out type, ClrType)
				: base.KnownTypes.TryRecognizeType(node, out type);
		}

		IXmlKnownType IXmlKnownTypeMap.GetXmlKnownType(Type type)
		{
			return IsMatch(type)
				? this
				: base.KnownTypes.GetXmlKnownType(type);
		}
	}
}
