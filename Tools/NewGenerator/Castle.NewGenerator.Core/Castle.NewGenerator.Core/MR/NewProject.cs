namespace Castle.NewGenerator.Core.MR
{
	using Mono.GetOptions;

	[Generator("mrproject", "New MonoRail Project")]
	[GeneratorOptions(typeof(NewController.CLIOptions))]
	public class NewProject : BaseGenerator
	{
		private string name, solutionName;
		private bool enableWindsor;

		[Param(Required = true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Param]
		public string SolutionName
		{
			get { return solutionName; }
			set { solutionName = value; }
		}

		[Param]
		public bool EnableWindsor
		{
			get { return enableWindsor; }
			set { enableWindsor = value; }
		}

		public override void Generate(GeneratorContext context, IGeneratorService generator)
		{
			if (solutionName != null)
			{
				// Check for solution folder

				// Change Path
			}

			// Check for target path

			// Create folders
			// Add files

			// -- \Contents
			// --    \css
			// --    \images
			// --    \javascripts
			// -- \Filters
			// -- \Controllers
			// --    BaseController.cs
			// -- \ViewComponents
			// -- \Models
			// -- \Views
			// --    \components
			// --    \layouts
			// --    \rescues
			// -- default.aspx
			// -- web.config

			// Create nant script

			// Create configuration files


		}

		public class CLIOptions : GeneratorConfigurer<NewProject>
		{
			[Option("name", 'n')]
			public string name;

			[Option("solutionname", 's')]
			public string solutionname;

			public override void Configure(NewProject generator, string[] args)
			{
				ProcessArgs(args);

				generator.Name = name;
				generator.SolutionName = solutionname;
			
				base.Configure(generator, args);
			}
		}
	}
}
