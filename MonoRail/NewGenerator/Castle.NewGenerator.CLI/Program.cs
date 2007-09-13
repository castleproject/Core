namespace Castle.NewGenerator.CLI
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using Castle.Components.Common.TemplateEngine;
	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;
	using Castle.NewGenerator.Core.MR;
	using Core;
	using Mono.GetOptions;

	public class GeneratorDriver
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

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

			string workingDir = AppDomain.CurrentDomain.BaseDirectory;
			string templateDir = @"C:\dev\DotNet\castle\svn\trunk\MonoRail\NewGenerator\GeneratorTemplates\";

			GeneratorContext context = new GeneratorContext(workingDir, templateDir);

			ITemplateEngine engine = new NVelocityTemplateEngine(templateDir);

			if (engine is ISupportInitialize)
			{
				((ISupportInitialize)engine).BeginInit();
			}

			generator.Generate(context, new DefaultGeneratorService(context, engine));
		}

		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			// C:\dev\castle\svn\trunk\build\net-2.0\debug
			return Assembly.LoadFile(@"C:\dev\DotNet\castle\svn\trunk\build\net-2.0\debug\" + args.Name);
		}

		private static void ShowUsage()
		{
			Console.WriteLine("CastleGen project|controller -property name=value");
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
