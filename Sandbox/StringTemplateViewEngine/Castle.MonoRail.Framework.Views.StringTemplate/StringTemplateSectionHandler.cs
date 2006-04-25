using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	using System.Configuration;
	using System.Xml;
	using antlr.stringtemplate.language;

	public class StringTemplateSectionHandler : IConfigurationSectionHandler
	{

		public StringTemplateSectionHandler()
		{
		}

		public object Create(object parent, object configContext, XmlNode section)
		{
			StringTemplateSettings settings = new StringTemplateSettings();

			XmlNodeList renderers = section.SelectNodes("renderers/renderer");
			foreach (XmlNode renderer in renderers)
			{
				string classToRender = renderer.Attributes["targetClass"].Value;
				string rendererClass = renderer.Attributes["rendererClass"].Value;

				settings.AddRenderer(classToRender, rendererClass);
			}

			XmlNode writer = section.Attributes["templateWriterClass"];
			if (writer != null)
				settings.SetTemplateWriter(writer.Value);

			XmlNode delimiter = section.Attributes["delimiterClass"];
			if (delimiter != null)
				settings.SetDelimiter(delimiter.Value);
			
			return settings;
		}

	}
}
