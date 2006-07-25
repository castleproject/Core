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

namespace TestSiteNVelocity.Components
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;

	public class ComponentAndParams : ViewComponent
	{
		public override void Render()
		{
			object param1 = Context.ComponentParameters["intParamLiteral"];
			object param2 = Context.ComponentParameters["intParam"];
			object param3 = Context.ComponentParameters["dictParam"];
			object param4 = Context.ComponentParameters["strParamLiteral"];
			object param5 = Context.ComponentParameters["strParam"];
			
			RenderText(String.Format("{0} {1} {2} {3} {4}", param1, param2, param3 is IDictionary, param4, param5));
		}
	}
}
