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
	using System;

	public class MockXmlIncludedTypeMap : IXmlIncludedTypeMap, IXmlIncludedType
	{
		private readonly XmlIncludedTypeSet includedTypes;

		public MockXmlIncludedTypeMap()
		{
			includedTypes = new XmlIncludedTypeSet();
		}

		public XmlIncludedTypeSet InnerSet { get { return includedTypes; } }
		public Type DefaultClrType { get; set; }

		public  IXmlIncludedType Default { get { return this; } }
		XmlName IXmlIncludedType.XsiType { get { return XmlName.Empty; } }
		Type    IXmlIncludedType.ClrType { get { return DefaultClrType; } }

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			return (xsiType == Default.XsiType)
				? Try.Success(out includedType, Default)
				: includedTypes.TryGet(xsiType, out includedType);
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return (clrType == Default.ClrType)
				? Try.Success(out includedType, Default)
				: includedTypes.TryGet(clrType, out includedType);
		}
	}
}
