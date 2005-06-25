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
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Buit in <see cref="Controller"/> containing the files requireds by helpers and other 
	/// parts of MonoRail.
	/// </summary>
	[ControllerDetails(Area="MonoRail")]
	[Resource("Ajax","Castle.MonoRail.Framework.Controllers.Ajax")]
	[Resource("Effects2", "Castle.MonoRail.Framework.Controllers.Effects2")]
	[Resource("EffectsFat", "Castle.MonoRail.Framework.Controllers.EffectsFat")]
	[Resource("Validation", "Castle.MonoRail.Framework.Controllers.Validation")]
	public sealed class FilesController : Controller
	{
		public FilesController()
		{
		}

		private string GetResourceValue(string resName, string resKey)
		{
			return (string)(Resources[resName])[resKey];
		}

		private void RenderFile(string resourceName, string resourceKey)
		{
			RenderText(GetResourceValue(resourceName, resourceKey));
		}

		/// <summary>
		/// Script used by <see cref="AjaxHelper"/>.
		/// </summary>
		public void AjaxScripts()
		{
			RenderFile("Ajax", "jsfunctions");
		}

		/// <summary>
		/// Script used by <see cref="EffectsFatHelper"/>.
		/// </summary>
		public void EffectsFatScripts()
		{
			RenderFile("EffectsFat", "fatfunctions");
		}

		/// <summary>
		/// Script used by <see cref="Effects2Helper"/>.
		/// </summary>
		public void Effects2()
		{
			RenderFile("Effects2", "functions");
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateConfig()
		{
			RenderFile("Validation", "fValidateConfig");
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateCore()
		{
			RenderFile("Validation", "fValidateCore");
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateValidators()
		{
			RenderFile("Validation", "fValidateValidators");
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateLang()
		{
			RenderFile("Validation", "fValidateLang");
		}
	}
}
