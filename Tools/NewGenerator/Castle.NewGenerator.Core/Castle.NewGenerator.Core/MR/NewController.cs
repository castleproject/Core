namespace Castle.NewGenerator.Core.MR
{
	using Mono.GetOptions;

	[Generator("controller", "New Controller")]
	[GeneratorPanel(typeof(NewControllerDialog))]
	[GeneratorOptions(typeof(NewController.CLIOptions))]
	public class NewController : BaseGenerator
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

		public override void Generate(GeneratorContext context, IGeneratorService generator)
		{
			
		}

		public class CLIOptions : GeneratorConfigurer<NewController>
		{
			[Option("name", 'n')]
			public string name;

			public override void Configure(NewController generator, string[] args)
			{
				ProcessArgs(args);

				generator.Name = name;

				base.Configure(generator, args);
			}
		}
	}
}