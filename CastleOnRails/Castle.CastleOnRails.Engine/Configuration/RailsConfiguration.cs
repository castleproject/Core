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
	using System.Collections;

	public class RailsConfiguration
	{
		private IList _assemblies = new ArrayList();
		private String _viewsPhysicalPath;
		private String _customControllerFactory;
		private String _customFilterFactory;
		private String _customEngineTypeName;
		private String _VirtualRootDir;

		public RailsConfiguration()
		{
		}

		public IList Assemblies
		{
			get { return _assemblies; }
		}

		public String ViewsPhysicalPath
		{
			get { return _viewsPhysicalPath; }
			set { _viewsPhysicalPath = value; }
		}

		public String CustomControllerFactory
		{
			get { return _customControllerFactory; }
			set { _customControllerFactory = value; }
		}

		public String CustomFilterFactory
		{
			get { return _customFilterFactory; }
			set { _customFilterFactory = value; }
		}

		public String VirtualRootDir
		{
			get { return _VirtualRootDir; }
			set { _VirtualRootDir = value; }
		}

		public String CustomEngineTypeName
		{
			get { return _customEngineTypeName; }
			set { _customEngineTypeName = value; }
		}

		public Type CustomViewEngineType
		{
			get
			{
				return _customEngineTypeName != null ?
					Type.GetType(_customEngineTypeName, false, false) : null;
			}
		}

		public Type CustomFilterFactoryType
		{
			get
			{
				return _customFilterFactory != null ?
					Type.GetType(_customFilterFactory, false, false) : null;
			}
		}

		public Type CustomControllerFactoryType
		{
			get
			{
				return _customControllerFactory != null ?
					Type.GetType(_customControllerFactory, false, false) : null;
			}
		}
	}
}