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

	public interface IXmlKnownTypeMap
	{
		IXmlKnownType Default { get; }

		bool TryGet(IXmlIdentity xmlNode, out IXmlKnownType knownType);
		bool TryGet(Type         clrType, out IXmlKnownType knownType);
	}

	public static class XmlKnownTypeMapExtensions
	{
		public static IXmlKnownType Require(this IXmlKnownTypeMap map, Type clrType)
		{
			IXmlKnownType knownType;
			if (map.TryGet(clrType, out knownType))
				return knownType;

			throw Error.NotXmlKnownType(clrType);
		}
	}
}
#endif
