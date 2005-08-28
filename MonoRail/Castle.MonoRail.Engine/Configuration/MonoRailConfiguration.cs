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

namespace Castle.MonoRail.Engine.Configuration
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.Text.RegularExpressions;


	public class MonoRailConfiguration
	{
		private static readonly string DefaultScaffoldType = "Castle.MonoRail.ActiveRecordScaffold.ScaffoldingSupport, Castle.MonoRail.ActiveRecordScaffold";

		public static readonly string SectionName = "monoRail";

		private IList _controllers = new ArrayList();
		private IList _components = new ArrayList();
		private IList _routingRules = new ArrayList();
		private bool _viewsXhtmlRendering;
		private String _viewsPhysicalPath;
		private String _customControllerFactory;
		private String _customViewComponentFactory;
		private String _customFilterFactory;
		private String _customResourceFactory;
		private String _customEngineTypeName;
		private String _scaffoldingTypeName = DefaultScaffoldType;

		public MonoRailConfiguration()
		{
		}

		public IList ControllerAssemblies
		{
			get { return _controllers; }
		}

		public IList ComponentsAssemblies
		{
			get { return _components; }
		}

		public IList RoutingRules
		{
			get { return _routingRules; }
		}

		public String ViewsPhysicalPath
		{
			get { return _viewsPhysicalPath; }
			set { _viewsPhysicalPath = value; }
		}

		public bool ViewsXhtmlRendering
		{
			get { return _viewsXhtmlRendering; }
			set { _viewsXhtmlRendering = value; }
		}

		public String CustomViewComponentFactory
		{
			get { return _customViewComponentFactory; }
			set { _customViewComponentFactory = value; }
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

		public String CustomResourceFactory
		{
			get { return _customResourceFactory; }
			set { _customResourceFactory = value; }
		}

		public String CustomEngineTypeName
		{
			get { return _customEngineTypeName; }
			set { _customEngineTypeName = value; }
		}

		public String ScaffoldingTypeName
		{
			get { return _scaffoldingTypeName; }
			set { _scaffoldingTypeName = value; }
		}

		public Type CustomViewEngineType
		{
			get
			{
				return _customEngineTypeName != null ?
					Type.GetType(_customEngineTypeName, false, false) : null;
			}
		}

		public Type CustomViewComponentFactoryType
		{
			get
			{
				return _customViewComponentFactory != null ?
					Type.GetType(_customViewComponentFactory, false, false) : null;
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

		public Type CustomResourceFactoryType
		{
			get
			{
				return _customResourceFactory != null ?
					Type.GetType(_customResourceFactory, false, false) : null;
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

		public Type ScaffoldingType
		{
			get
			{
				return _scaffoldingTypeName != null ? Type.GetType(_scaffoldingTypeName, false, false) : null;
			}
		}

		internal static MonoRailConfiguration GetConfig()
		{
			MonoRailConfiguration config = (MonoRailConfiguration) ConfigurationSettings.GetConfig(MonoRailConfiguration.SectionName);

			if (config == null)
			{
				throw new ApplicationException("Unfortunately, you have to provide " + 
					"a small configuration to use MonoRail. Check the samples or the documentation.");
			}

			return config;
		}
	}

	public class RoutingRule
	{
		private String _pattern, _replace;
		private Regex _rule;

		public RoutingRule(String pattern, String replace)
		{
			_pattern = pattern;
			_replace = replace;

			_rule = new Regex(pattern, RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.Singleline);
		}

		public string Pattern
		{
			get { return _pattern; }
		}

		public string Replace
		{
			get { return _replace; }
		}

		public Regex CompiledRule
		{
			get { return _rule; }
		}
	}
}
