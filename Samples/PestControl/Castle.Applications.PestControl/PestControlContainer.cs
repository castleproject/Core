// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Applications.PestControl
{
	using System;

	using Castle.MicroKernel;
	using Castle.Windsor;

	using Castle.Facilities.Prevalence;

	using Castle.Applications.PestControl.Services.BuildSystems;
	using Castle.Applications.PestControl.Services.SourceControl;

	/// <summary>
	/// Summary description for PestControlContainer.
	/// </summary>
	public class PestControlContainer : WindsorContainer
	{
		public PestControlContainer( IConfigurationStore configStore ) : base()
		{
			Kernel.ConfigurationStore = configStore;

			AddFacility("prevalence", new PrevalenceFacility() );

			// BS Manager

			AddComponent("buildsystemmanager", typeof(BuildSystemManager));

			// Build Systems

			AddComponent("nant", typeof(IBuildSystem), typeof(NAntBuildSystem));
			AddComponent("msbuild", typeof(IBuildSystem), typeof(MSBuildBuildSystem));

			// SC Manager

			AddComponent("sourcecontrolmanager", typeof(SourceControlManager));

			// Source Controls

			AddComponent("svnsc", typeof(ISourceControl), typeof(SvnSourceControl));
			AddComponent("cvssc", typeof(ISourceControl), typeof(CvsSourceControl));
			AddComponent("vsssc", typeof(ISourceControl), typeof(VssSourceControl));
		}
	}
}
