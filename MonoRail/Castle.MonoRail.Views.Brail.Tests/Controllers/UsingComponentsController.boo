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
namespace Castle.MonoRail.Views.Brail.Tests.Controllers

import System
import Castle.MonoRail.Framework

class UsingComponentsController(SmartDispatcherController):

	def Index1():
		pass
	
	def Index2():
		pass
	
	def Index3():
		pass

	def Index4():
		pass

	def Index5():
		pass
	
	def Index8():
		items = [1,2]
		PropertyBag.Add("items",items)
	
	def Index9():
		pass
