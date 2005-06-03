// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	public enum TargetStrategyEnum
	{
		Undefined,
		SingleType,
		Namespace,
		Assignable,
		Custom
	}

	/// <summary>
	/// Summary description for TargetTypeDefinition.
	/// </summary>
	[Serializable]
	public class TargetTypeDefinition : NodeBase
	{
		private TypeReference _singleType;
		private String _namespaceRoot;
		private TypeReferenceCollection _excludes;
		private TargetStrategyEnum _strategy = TargetStrategyEnum.Undefined;
		private TypeReference _customMatcherType;
		private TypeReference _assignType;
		private bool _includeSubNamespace = false;

		public TargetTypeDefinition( TypeReference typeRef ) : this()
		{
			SingleType = typeRef;
		}

		public TargetTypeDefinition()
		{
			_strategy = TargetStrategyEnum.SingleType;
		}

		public TypeReference SingleType
		{
			get { return _singleType; }
			set { _singleType = value; }
		}

		public TypeReference CustomMatcherType
		{
			get { return _customMatcherType; }
			set { _customMatcherType = value; }
		}

		public TypeReference AssignType
		{
			get { return _assignType; }
			set { _assignType = value; }
		}

		public TargetStrategyEnum TargetStrategy
		{
			get { return _strategy; }
			set { _strategy = value; }
		}

		public String NamespaceRoot
		{
			get { return _namespaceRoot; }
			set { _namespaceRoot = value; }
		}

		public bool IncludeSubNamespace
		{
			get { return _includeSubNamespace; }
			set { _includeSubNamespace = value; }
		}

		public TypeReferenceCollection Excludes
		{
			get
			{
				if (_excludes == null)
				{
					_excludes = new TypeReferenceCollection();
				}
				return _excludes;
			}
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnTargetTypeDefinition(this);
		}
	}
}
