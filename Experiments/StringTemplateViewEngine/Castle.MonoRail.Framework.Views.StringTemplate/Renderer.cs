using System;

namespace Castle.MonoRail.Framework.Views.StringTemplate
{
	public class Renderer
	{
		private readonly string classToRender;
		private readonly string rendererClass;

		public Renderer(string classToRender, string rendererClass)
		{
			this.classToRender = classToRender;
			this.rendererClass = rendererClass;
		}

		public Type ClassToRender
		{
			get { return Type.GetType(classToRender); }
		}

		public object CreateRenderer()
		{
			return Activator.CreateInstance(Type.GetType(rendererClass));
		}
	}
}
