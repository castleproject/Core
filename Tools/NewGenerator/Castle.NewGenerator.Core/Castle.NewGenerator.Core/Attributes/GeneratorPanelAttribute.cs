namespace Castle.NewGenerator.Core
{
	using System;

	public class GeneratorPanelAttribute : Attribute
	{
		private readonly Type panelType;

		public GeneratorPanelAttribute(Type panelType)
		{
			this.panelType = panelType;
		}
	}
}
