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

namespace Castle.Applications.PestControl.Services.BuildSystems
{
	using System;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for MSBuildBuildSystem.
	/// </summary>
	public class MSBuildBuildSystem : IBuildSystem
	{
		public MSBuildBuildSystem()
		{
		}

		#region IBuildSystem Members

		public String Name
		{
			get
			{
				return "MSBuild";
			}
		}

		public String Key
		{
			get
			{
				return "msbuild";
			}
		}

		public BuildResult Build(Project project)
		{
			// Work work work

			return new BuildResult();
		}

		#endregion
	}
}
