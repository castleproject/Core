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

namespace Castle.MonoRail.Framework.Views.StringTemplateView.Configuration
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;

	using IAttributeRenderer = Antlr.StringTemplate.IAttributeRenderer;


	public class STViewEngineConfiguration
	{
		private static readonly string GLOBAL_RENDERER_KEY = "::global_renderers::";

		private string _templateLexerTypeName;
		private string _templateWriterTypeName;
		private IList _attributeRenderers = new ArrayList();
		private IDictionary _area2renderers = new HybridDictionary();

		private XmlNode _section;

		internal class RendererInfo
		{
			protected string rendererTypeName;
			protected string attributeTypeName;

			public RendererInfo(string rendererTypeName, string attributeTypeName)
			{
				this.rendererTypeName = rendererTypeName;
				this.attributeTypeName = attributeTypeName;
			}

			public IAttributeRenderer GetRendererInstance()
			{
				return (IAttributeRenderer) Activator.CreateInstance(STViewEngineConfiguration.GetType(rendererTypeName));
			}

			public Type AttributeType
			{
				get { return STViewEngineConfiguration.GetType(attributeTypeName); }
			}

		}

		public STViewEngineConfiguration()
		{
		}

		public string TemplateLexerTypeName
		{
			get { return _templateLexerTypeName != null ? _templateLexerTypeName : typeof(Antlr.StringTemplate.Language.DefaultTemplateLexer).AssemblyQualifiedName;  }
			set { _templateLexerTypeName = value; }
		}
		public Type TemplateLexerType
		{
			get { return _templateLexerTypeName != null ? GetType(_templateLexerTypeName) : typeof(Antlr.StringTemplate.Language.DefaultTemplateLexer); }
		}

		public string TemplateWriterTypeName
		{
			get { return _templateWriterTypeName;  }
			set { _templateWriterTypeName = value; }
		}
		public Type TemplateWriterType
		{
			get { return _templateWriterTypeName != null ? GetType(_templateWriterTypeName) : null; }
		}

		internal IList AllAttributeRenderers
		{
			get { return _attributeRenderers; }
		}

		public void RegisterAttributeRenderer(string rendererTypeName, string attributeTypeName)
		{
			InternalRegisterAttributeRenderer(rendererTypeName, attributeTypeName, null);
		}

		public void RegisterAttributeRenderer(string rendererTypeName, string attributeTypeName, string area)
		{
			InternalRegisterAttributeRenderer(rendererTypeName, attributeTypeName, area);
		}

		public IEnumerator GetAttributeRenderersForArea(string area)
		{
			IList renderers = InternalGetAttributeRenderers(area);
			if (renderers != null)
			{
				return renderers.GetEnumerator();
			}
			return new ArrayList().GetEnumerator();
		}

		public IEnumerator GetGlobalAttributeRenderers()
		{
			IList renderers = InternalGetAttributeRenderers(null);
			if (renderers != null)
			{
				return renderers.GetEnumerator();
			}
			return new ArrayList().GetEnumerator();
		}

		private void InternalRegisterAttributeRenderer(string rendererTypeName, string attributeTypeName, string area)
		{
			string key = area;
			if ((key == null) || (key.Trim().Length == 0))
			{
				key = GLOBAL_RENDERER_KEY;
			}

			IList rendererList = (IList)_area2renderers[key];
			if (rendererList == null)
			{
				rendererList = new ArrayList();
				_area2renderers[key] = rendererList;
			}
			RendererInfo rendererInfo = new RendererInfo(rendererTypeName, attributeTypeName);
			rendererList.Add(rendererInfo);

			if (!_attributeRenderers.Contains(rendererTypeName))
			{
				_attributeRenderers.Add(rendererTypeName);
			}
		}

		private IList InternalGetAttributeRenderers(string area)
		{
			string key = area;
			if ((key == null) || (key.Trim().Length == 0))
			{
				key = GLOBAL_RENDERER_KEY;
			}

			return (IList)_area2renderers[key];
		}

		public XmlNode ConfigSection
		{
			get { return _section;  }
			set { _section = value; }
		}

		internal static STViewEngineConfiguration GetConfig(string sectionName)
		{
			STViewEngineConfiguration config = (STViewEngineConfiguration)
				ConfigurationSettings.GetConfig(sectionName);

			if (config == null)
				return new STViewEngineConfiguration();
			return config;
		}

		public static Type GetType( String typeName )
		{
			return GetType( typeName, false );
		}

		public static Type GetType( String typeName, bool ignoreError )
		{
			Type loadedType = Type.GetType(typeName, false, false);

			if ( loadedType == null && !ignoreError )
			{
				throw new ConfigurationException( String.Format("The type {0} could not be found", typeName) );
			}
			return loadedType;
		}
	}
}