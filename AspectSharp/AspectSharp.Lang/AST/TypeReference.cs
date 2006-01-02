// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Lang.AST
{
	using System;

	public enum TargetTypeEnum
	{
		Undefined,
		Link,
		Type
	}

	/// <summary>
	/// Summary description for TypeReference.
	/// </summary>
	[Serializable]
	public class TypeReference : NodeBase
	{
		private String _typeName;
		private String _linkRef;
		private AssemblyReference _assembly;
		private TargetTypeEnum _targetType = TargetTypeEnum.Undefined;
		private Type _resolvedType;

		public TypeReference()
		{
		}

		public TypeReference(LexicalInfo info) : base(info)
		{
		}

		public TypeReference(LexicalInfo info, String typeName) : this(info)
		{
			TypeName = typeName;
		}

		public TypeReference(LexicalInfo info, String value, TargetTypeEnum targetType) : this(info)
		{
			if (targetType == TargetTypeEnum.Link)
			{
				LinkRef = value;
			}
			else
			{
				TypeName = value;
			}
		}

		public TargetTypeEnum TargetType
		{
			get { return _targetType; }
		}

		public string TypeName
		{
			get { return _typeName; }
			set { _typeName = value; _targetType = TargetTypeEnum.Type; }
		}

		public string LinkRef
		{
			get { return _linkRef; }
			set { _linkRef = value; _targetType = TargetTypeEnum.Link; }
		}

		public AssemblyReference AssemblyReference
		{
			get { return _assembly; }
			set { _assembly = value; }
		}

		public Type ResolvedType
		{
			get { return _resolvedType; }
			set { _resolvedType = value; }
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnTypeReferenceDefinition(this);
		}

		public override String ToString()
		{
			return _typeName != null ? _typeName : _linkRef;
		}
	}
}
