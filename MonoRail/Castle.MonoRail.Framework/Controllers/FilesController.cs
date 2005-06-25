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

namespace Castle.MonoRail.Framework.Controllers
{
	[ControllerDetails(Area="MonoRail")]
	[Resource("Ajax","Castle.MonoRail.Framework.Controllers.Ajax")]
	[Resource("Effects2", "Castle.MonoRail.Framework.Controllers.Effects2")]
	[Resource("EffectsFat", "Castle.MonoRail.Framework.Controllers.EffectsFat")]
	//[Resource("Validation", "Castle.MonoRail.Framework.Controllers.Validation")]
	public class FilesController : Controller
	{
		public FilesController()
		{
		}

		private string GetResourceValue(string resName, string resKey)
		{
			return (string)((IResource)Resources[resName])[resKey];
		}

		public void AjaxScripts()
		{
			RenderText(GetResourceValue("Ajax", "jsfunctions"));
		}

		public void EffectsFatScripts()
		{
			RenderText(GetResourceValue("EffectsFat", "fatfunctions"));
		}

		public void Effects2()
		{
			RenderText(GetResourceValue("Effects2", "functions"));
		}
	}
}
