namespace Castle.NewGenerator.CLI
{
	using System;
	using System.Reflection;
	using Castle.NewGenerator.Core.MR;
	using Core;
	using Mono.GetOptions;

	public class GeneratorDriver
	{
		static void Main(string[] args)
		{
			MainOptions opts = new MainOptions();
			opts.ProcessArgs(args);

			if (args.Length == 0 || args[0].StartsWith("-"))
			{
				ShowUsage();
			}

			Console.WriteLine("Processing {0} {1}", args[0], opts.verbose);

			IGenerator generator = null;

			if (args[0] == "controller")
			{
				generator = new NewController();
			}
			else if (args[0] == "project")
			{
				generator = new NewProject();
			}
			else
			{
				Console.Error.WriteLine("Not supported");
				return;
			}

			Configure(generator, args);
		}

		private static void ShowUsage()
		{
			
		}

		private static void Configure(IGenerator generator, string[] args)
		{
			Type type = generator.GetType();

			object[] attrs = type.GetCustomAttributes(typeof(GeneratorOptionsAttribute), false);

			if (attrs.Length == 1)
			{
				GeneratorOptionsAttribute attr = (GeneratorOptionsAttribute) attrs[0];

				Type configurerType = typeof(GeneratorConfigurer<>).MakeGenericType(type);

				object configurer = Activator.CreateInstance(attr.OptionClass);

				MethodInfo method = configurerType.GetMethod("Configure", BindingFlags.Instance | BindingFlags.Public);

				method.Invoke(configurer, new object[] { generator, args });
			}
		}

		public class MainOptions : Options
		{
			[Option(1, "Verbose", 'v')]
			public bool verbose;
		}
	}
}
