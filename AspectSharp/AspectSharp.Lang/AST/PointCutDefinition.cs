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

	[Flags]
	public enum PointCutFlags
	{
		Unspecified,
		Method = 1,
		Property = 2,
		PropertyRead = 4,
		PropertyWrite = 8,
	}

	/// <summary>
	/// Summary description for PointCutDefinition.
	/// </summary>
	[Serializable]
	public class PointCutDefinition : NodeBase
	{
		private MethodSignature _targetMethod = AllMethodSignature.Instance;
		private InterceptorDefinitionCollection _advices = new InterceptorDefinitionCollection();
		private PointCutFlags _flags;

		public PointCutDefinition(LexicalInfo info, PointCutFlags flags) : base(info)
		{
			_flags = flags;
		}

		public MethodSignature Method
		{
			get { return _targetMethod; }
			set { _targetMethod = value; }
		}

		public PointCutFlags Flags
		{
			get { return _flags; }
		}

		public InterceptorDefinitionCollection Advices
		{
			get { return _advices; }
		}
	
		public override bool Equals(object other)
		{
			PointCutDefinition otherCut = other as PointCutDefinition;

			if (otherCut == null)
			{
				return false;
			}

			if (otherCut.Flags.Equals( Flags ) && 
				otherCut.Method.Equals( Method ))
			{
				return true;
			}

			return false;
		}
	
		public override int GetHashCode()
		{
			// Doh!!
			return ((int) _flags) ^ Method.GetHashCode();
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnPointCutDefinition(this);
		}
	}
}
