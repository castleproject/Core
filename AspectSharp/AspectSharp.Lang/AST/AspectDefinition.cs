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

	/// <summary>
	/// Summary description for AspectDefinition.
	/// </summary>
	[Serializable]
	public class AspectDefinition : NodeBase
	{
		private String _name;
		private TargetTypeDefinition _targetType;
		private PointCutDefinitionCollection _pointcuts = new PointCutDefinitionCollection();
		private MixinDefinitionCollection _mixins = new MixinDefinitionCollection();

		public AspectDefinition(LexicalInfo info, String name) : base(info)
		{
			Name = name;
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public TargetTypeDefinition TargetType
		{
			get { return _targetType; }
			set { _targetType = value; }
		}

		public PointCutDefinitionCollection PointCuts
		{
			get { return _pointcuts; }
		}

		public MixinDefinitionCollection Mixins
		{
			get { return _mixins; }
		}

		public override void Accept(IVisitor visitor)
		{
			visitor.OnAspectDefinition(this);
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			return _name.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}
	}
}
