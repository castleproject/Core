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

namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Parser.Node;


	public class Component : Directive
	{
		public Component()
		{
		}

		public override void init(RuntimeServices rs, InternalContextAdapter context, INode node)
		{
			base.init(rs, context, node);
		}

		public override bool render(InternalContextAdapter context, TextWriter writer, INode node)
		{

			return true;
		}

		public override String Name
		{
			get { return "component"; }
			set {  }
		}

		public override int Type
		{
			get { return DirectiveConstants_Fields.BLOCK; }
		}
	}
}
