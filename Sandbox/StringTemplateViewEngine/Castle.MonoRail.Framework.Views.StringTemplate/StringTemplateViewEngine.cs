namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	using System;
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using antlr.stringtemplate;

	public class StringTemplateViewEngine : ViewEngineBase
	{
		private static readonly string LAYOUTS_DIRECTORY = "Layouts";
		private static readonly string SITE_ROOT = "siteRoot";
		private static readonly string PRIMARY_GROUP = "app";

		private StringTemplateGroup stringTemplate;
		private StringTemplateSettings settings = null;


		public override void Init(IServiceProvider serviceProvider)
		{
			base.Init(serviceProvider);

			settings = (StringTemplateSettings) ConfigurationSettings.GetConfig("stringTemplateViewEngine");
			if (settings.Delimiter != null)
				stringTemplate = new StringTemplateGroup(PRIMARY_GROUP, base.ViewSourceLoader.ViewRootDir, settings.Delimiter);
			else
				stringTemplate = new StringTemplateGroup(PRIMARY_GROUP, base.ViewSourceLoader.ViewRootDir);
		}

		public override bool HasTemplate(string templateName)
		{
			string dir = base.ViewSourceLoader.ViewRootDir;

			dir = Path.Combine(dir, templateName + ".st");
			return (File.Exists(dir));
		}

		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			TextWriter output = context.Response.Output;
			if (!Helper.IsEmpty(controller.LayoutName))
				output = new StringWriter();

			this.AdjustContentType(context);

			string groupName = controller.AreaName + ":" + controller.Name;

			string dirName = base.ViewSourceLoader.ViewRootDir;
			dirName = Path.Combine(dirName, Path.GetDirectoryName(templateName));

			StringTemplate view = ObtainTemplate(groupName, dirName, templateName);

			RenderTemplate(context, controller, view, output);

			if (!Helper.IsEmpty(controller.LayoutName))
				WrapInLayout(context, controller, (StringWriter) output);
		}

		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			TextWriter output = context.Response.Output;

			if (!Helper.IsEmpty(controller.LayoutName))
				output = new StringWriter();

			this.AdjustContentType(context);

			string groupName = controller.AreaName + ":" + controller.Name;
			string dirName = base.ViewSourceLoader.ViewRootDir;
			if (controller.AreaName != "")
				dirName = Path.Combine(dirName, Path.GetDirectoryName(controller.AreaName));

			dirName = Path.Combine(dirName, Path.GetDirectoryName(controller.Name));


			StringTemplateGroup template = new StringTemplateGroup(groupName, dirName);
			template.setSuperGroup(stringTemplate);
			StringTemplate view = template.createStringTemplate();
            view.setTemplate(contents);

			RenderTemplate(context, controller, view, output);

			if (!Helper.IsEmpty(controller.LayoutName))
				WrapInLayout(context, controller, (StringWriter) output);
		}

		#region Private Methods

		private StringTemplate ObtainTemplate(string groupName, string dirName, string templateName)
		{
			StringTemplateGroup template = new StringTemplateGroup(groupName, dirName);
			template.setSuperGroup(stringTemplate);


			return template.getInstanceOf(Path.GetFileName(templateName));
		}

		private void RenderTemplate(IRailsEngineContext context, Controller controller, StringTemplate view, TextWriter output)
		{
			for (int i = 0; i < context.Request.Params.Count; i++)
				view.setAttribute(controller.Params.Keys[i], controller.Params[i]);

			if (controller.Resources != null)
			{
				foreach (string key in controller.Resources.Keys)
					view.setAttribute(key, controller.Resources[key]);
			}

			foreach (DictionaryEntry entry in context.Flash)
				view.setAttribute(entry.Key.ToString(), entry.Value);

			foreach (DictionaryEntry entry in controller.PropertyBag)
				view.setAttribute(entry.Key.ToString(), entry.Value);

			view.setAttribute(SITE_ROOT, context.ApplicationPath);

			if (settings.TemplateWriter != null)
				view.getGroup().setStringTemplateWriter(settings.TemplateWriter);

			foreach (Renderer renderer in settings.Renderers)
				view.registerRenderer(renderer.ClassToRender, renderer.CreateRenderer());


			this.PreSendView(controller, view);

			if (settings.TemplateWriter != null)
			{
				StringTemplateWriter writer = (StringTemplateWriter) Activator.CreateInstance(settings.TemplateWriter, new object[] {output});
				view.write(writer);
			}
			else
				output.Write(view.ToString());

			this.PostSendView(controller, view);
		}

		private void WrapInLayout(IRailsEngineContext context, Controller controller, StringWriter writer)
		{
			string groupName = controller.LayoutName;
			string dirName = Path.Combine(base.ViewSourceLoader.ViewRootDir, LAYOUTS_DIRECTORY);

			StringTemplate view = ObtainTemplate(groupName, dirName, controller.LayoutName);

			view.registerRenderer(typeof (ContentEncapsulation), new ContentEncapsulationRenderer());
			view.setAttribute("childContent", new ContentEncapsulation(writer.ToString()));

			RenderTemplate(context, controller, view, context.Response.Output);
		}

		#endregion
	}
}