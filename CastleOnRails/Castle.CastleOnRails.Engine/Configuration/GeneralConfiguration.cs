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

namespace Castle.CastleOnRails.Engine.Configuration
{
	using System;

	/// <summary>
	/// Summary description for GeneralConfiguration.
	/// </summary>
	public class GeneralConfiguration
	{
		private String _controllersAssembly;
		private String _viewsPhysicalPath;
		private String _customControllerFactory;

		public GeneralConfiguration()
		{
		}

		public string ControllersAssembly
		{
			get { return _controllersAssembly; }
			set { _controllersAssembly = value; }
		}

		public string ViewsPhysicalPath
		{
			get { return _viewsPhysicalPath; }
			set { _viewsPhysicalPath = value; }
		}

		public string CustomControllerFactory
		{
			get { return _customControllerFactory; }
			set { _customControllerFactory = value; }
		}
	}
}
