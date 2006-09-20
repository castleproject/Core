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


class IgnoreNull (IQuackFu):

	target as object

	def constructor(obj):
		self.target = obj
		
	def QuackGet(name as string):
		return null if target is null
		return target.GetType().GetProperty(name).GetValue(target,null)
	
	def QuackSet(name as string, obj as object):
		return if target is null
		target.GetType().GetProperty(name).SetValue(target,null,obj)
	
	def QuackInvoke(name as string, args as (object)):
		return null if target is null
		return target.GetType().GetMethod(name).Invoke(target,args)