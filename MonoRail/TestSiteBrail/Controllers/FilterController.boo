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
namespace Castle.MonoRail.Views.Brail.TestSite.Controllers

import System
import Castle.MonoRail.Framework

[Filter(ExecuteEnum.BeforeAction, typeof(Castle.MonoRail.Views.Brail.TestSite.Controllers.FilterBadHeader) )]
class FilterController(Controller):
	
	def Index():
		pass

class FilterBadHeader(IFilter):
	
	def Perform(exec as ExecuteEnum, context as IRailsEngineContext, controller as Controller) as bool:
		if context.Request.Headers["mybadheader"] is not null:
			context.Response.Write("Denied!")
			return false
		return true
