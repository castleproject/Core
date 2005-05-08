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

namespace Castle.CastleOnRails.Framework.Views.CompositeView
{
	using System;
	using System.IO;

	using Castle.CastleOnRails.Framework.Views.Aspx;
	using Castle.CastleOnRails.Framework.Views.NVelocity;

	/// <summary>
	/// Composition of view engines that dispatch to 
	/// one or other based on the view file extesion.
	/// </summary>
	public class CompositeViewEngine : ViewEngineBase
	{
		private AspNetViewEngine _aspxViewEngine;
		private NVelocityViewEngine _nvelocityViewEngine;

		public CompositeViewEngine()
		{
		}

		#region IViewEngine Members

		public override void Init()
		{
			_aspxViewEngine = new AspNetViewEngine();
			_aspxViewEngine.ViewRootDir = ViewRootDir;
			_aspxViewEngine.Init();

			_nvelocityViewEngine = new NVelocityViewEngine();
			_nvelocityViewEngine.ViewRootDir = ViewRootDir;
			_nvelocityViewEngine.Init();
		}

		public override void Process(IRailsEngineContext context, Controller controller, String viewName)
		{
			bool aspxProcessed = ProcessAspx(context, controller, viewName);

			bool vmProcessed = ProcessVm(context, controller, viewName);

			if (!aspxProcessed && !vmProcessed)
			{
				String message = String.Format("No view file (aspx or vm) found for {0}", viewName);

				throw new RailsException(message);
			}
		}

		#endregion

		protected virtual bool ProcessVm(IRailsEngineContext context, Controller controller, string viewName)
		{
			FileInfo vmFile = new FileInfo(Path.Combine( ViewRootDir, viewName + ".vm" ));

			if (vmFile.Exists)
			{
				_nvelocityViewEngine.Process(context, controller, viewName);

				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual bool ProcessAspx(IRailsEngineContext context, Controller controller, string viewName)
		{
			FileInfo aspxFile = new FileInfo(Path.Combine( ViewRootDir, viewName + ".aspx" ));
			
			if (aspxFile.Exists)
			{
				_aspxViewEngine.Process(context, controller, viewName);

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
