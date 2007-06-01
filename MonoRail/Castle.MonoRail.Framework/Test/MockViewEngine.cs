namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.IO;

	public class MockViewEngine : IViewEngine
	{
		private string viewFileExtension;
		private string jsGeneratorFileExtension;
		private bool supportsJSGeneration;
		private bool xHtmlRendering;

		public MockViewEngine(string viewFileExtension, string jsGeneratorFileExtension, bool supportsJSGeneration, bool xHtmlRendering)
		{
			this.viewFileExtension = viewFileExtension;
			this.jsGeneratorFileExtension = jsGeneratorFileExtension;
			this.supportsJSGeneration = supportsJSGeneration;
			this.xHtmlRendering = xHtmlRendering;
		}

		public bool SupportsJSGeneration
		{
			get { return supportsJSGeneration; }
		}

		public bool XHtmlRendering
		{
			get { return xHtmlRendering; }
			set { xHtmlRendering = value; }
		}

		public string ViewFileExtension
		{
			get { return viewFileExtension; }
		}

		public string JSGeneratorFileExtension
		{
			get { return jsGeneratorFileExtension; }
		}

		public bool HasTemplate(string templateName)
		{
			throw new NotImplementedException();
		}

		public void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public void Process(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		public void GenerateJS(IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public void GenerateJS(TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		public void ProcessPartial(TextWriter output, IRailsEngineContext context, Controller controller, string partialName)
		{
			throw new NotImplementedException();
		}

		public void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			throw new NotImplementedException();
		}
	}
}
