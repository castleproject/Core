using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	using System.Collections;
	using System.Xml;

	public class StringTemplateSettings
	{
		private readonly ArrayList renderers = new ArrayList();
		private Type templateWriter = null;
		private Type delimiter = null;

		public StringTemplateSettings()
		{
		}

		public void SetTemplateWriter(string type)
		{
			templateWriter = GetTheType(type);
		}

		private Type GetTheType(string check)
		{
			Type type = Type.GetType(check);
			if (type == null) throw new TypeLoadException(check + " not found.");
			return type;
		}

		public void AddRenderer(string classToRender, string rendererClass)
		{
			GetTheType(classToRender);
			Type rendererType = GetTheType(rendererClass);

			try { Activator.CreateInstance(rendererType); } 
			catch(Exception ex)
			{
				throw new TypeInitializationException(rendererType.FullName, ex);
			}

			renderers.Add(new Renderer(classToRender, rendererClass));
		}

		public Renderer[] Renderers
		{
			get
			{
				return (Renderer[]) renderers.ToArray(typeof (Renderer));
			}
		}

		public void SetDelimiter(string delimiterType)
		{
			delimiter = GetTheType(delimiterType);
		}

		public Type TemplateWriter
		{
			get { return templateWriter; }
		}

		public Type Delimiter
		{
			get { return delimiter; }
		}
	}
}
