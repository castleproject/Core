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
namespace Castle.MonoRail.Views.Brail

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem

# This is a custom type implementation which allows to use common idioms such as 
# list & date as identifiers
class CustomTypeSystem(TypeSystemServices):
	def constructor(context as CompilerContext):
		super(context)
		
	override def PreparePrimitives():
		self.AddPrimitiveType("string", self.StringType)
		self.AddPrimitiveType("void", self.VoidType)
		self.AddPrimitiveType("int", self.IntType)
		self.AddPrimitiveType("bool", self.BoolType)

class InitializeCustomTypeSystem(AbstractCompilerStep):
	override def Run():
		self.Context.TypeSystemServices = CustomTypeSystem(self.Context)
