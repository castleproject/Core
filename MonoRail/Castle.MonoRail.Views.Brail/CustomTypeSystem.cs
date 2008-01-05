// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Compiler.TypeSystem;

	// This is a custom type implementation which allows to use common idioms such as 
	// list & date as identifiers
	public class CustomTypeSystem : TypeSystemServices
	{
		public CustomTypeSystem(CompilerContext context) : base(context)
		{
		}

		protected override void PreparePrimitives()
		{
			AddPrimitiveType("string", StringType);
			AddPrimitiveType("void", VoidType);
			AddPrimitiveType("int", IntType);
			AddPrimitiveType("bool", BoolType);
		}
	}

	public class InitializeCustomTypeSystem : AbstractCompilerStep
	{
		public override void Run()
		{
			Context.TypeSystemServices = new CustomTypeSystem(Context);
		}
	}
}