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
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Parses the Xml element that represents the STViewEngine 
	/// configuration information in the configuration file
	/// associated with the AppDomain (usually web.config).
	/// </summary>
	/// <remarks>
	/// <para>
	/// The STViewEngine integration can be customised by specifying options 
	/// in a custom section named 'STViewEngine' in the web.config file. 
	/// The following sample declaration shows what can be customized:
	///
	/// <code escaped="true">
	///		<STViewEngine 
	///			template-writer="Antlr.StringTemplate.AutoIndentWriter, StringTemplate" 
	///			template-lexer="Antlr.StringTemplate.Language.DefaultTemplateLexer, StringTemplate">
	///			
	///			<attribute-renderers>
	///				<attribute-renderer area="articles" renderer-type="Kunle.MyStringRenderer, Kunle.STRenderer" target-type="System.String" />
	///				<attribute-renderer renderer-type="Kunle.MyDateRenderer, Kunle.STRenderer" target-type="System.DateTime" />
	///			</attribute-renderers>
	///			
	///		</STViewEngine>
	/// </code>
	/// </para>
	/// </remarks>
	public class STViewEngineSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler implementation

		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			STViewEngineConfiguration config = new STViewEngineConfiguration();

			config.ConfigSection = section;

			XmlAttribute templateWriter = section.Attributes[ConfigConstants.ATTR_template_writer_type];
			if (templateWriter != null)
			{
				config.TemplateWriterTypeName = templateWriter.Value;
			}

			XmlAttribute templateLexer = section.Attributes[ConfigConstants.ATTR_template_lexer_type];
			if (templateLexer != null)
			{
				config.TemplateLexerTypeName = templateLexer.Value;
			}

			XmlNodeList renderers = section.SelectNodes(
				ConfigConstants.ELEMENT_attrib_renderers + "/" + ConfigConstants.ELEMENT_attrib_renderer
				);
			ProcessAttributeRenderers(renderers, config);

			Validate(config);

			return config;
		}

		#endregion

		#region Private Helpers

		private void ProcessAttributeRenderers(XmlNodeList renderers, STViewEngineConfiguration config)
		{
			foreach(XmlNode renderer in renderers)
			{
				string rendererTypeName = renderer.Attributes[ConfigConstants.ATTR_renderer_type].Value;
				string attributeTypeName = renderer.Attributes[ConfigConstants.ATTR_renderer_target_type].Value;
				XmlAttribute area = renderer.Attributes[ConfigConstants.ATTR_renderer_area];
				if (area == null)
                    config.RegisterAttributeRenderer(rendererTypeName, attributeTypeName);
				else
					config.RegisterAttributeRenderer(rendererTypeName, attributeTypeName, area.Value);
			}
		}

		#endregion

		#region Configuration Validation

		protected void Validate(STViewEngineConfiguration config)
		{
			if (config.TemplateLexerTypeName != null)
			{
				ValidateTypeImplements(config.TemplateLexerTypeName, typeof(antlr.TokenStream));
			}
			
			if (config.TemplateWriterTypeName != null)
			{
				ValidateTypeImplements(config.TemplateWriterTypeName, typeof(Antlr.StringTemplate.IStringTemplateWriter));
			}
			
			foreach(string rendererTypeName in config.AllAttributeRenderers)
			{
				ValidateTypeImplements(rendererTypeName, typeof(Antlr.StringTemplate.IAttributeRenderer));
			}
		}

		private void ValidateTypeImplements(String name, Type expectedType)
		{
			Type type = Type.GetType(name, false, false);
	
			if (type == null)
			{
				string message = string.Format("Type {0} could not be loaded", name);
				throw new ConfigurationException(message);
			}

			ValidateTypeImplements(type, expectedType);
		}

		private void ValidateTypeImplements(Type type, Type expectedType)
		{
			if (!expectedType.IsAssignableFrom(type))
			{
				string message = string.Format("Type {0} does not implement {1}", type.FullName, expectedType.FullName);
				throw new ConfigurationException(message);
			}
		}

		#endregion
	}
}
