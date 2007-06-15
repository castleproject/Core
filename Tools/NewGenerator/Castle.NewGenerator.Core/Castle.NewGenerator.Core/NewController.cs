namespace Castle.NewGenerator.Core
{
	using System;

	[Generator("controller", "New Controller")]
	[GeneratorPanel(typeof(NewControllerDialog))]
	public class NewController : IGenerator
	{
		private string name;
		private string[] actions;

		[Param(Required=true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Param]
		public string[] Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		public void Generate(GeneratorContext context, IGeneratorService generator)
		{
			
		}
	}
}