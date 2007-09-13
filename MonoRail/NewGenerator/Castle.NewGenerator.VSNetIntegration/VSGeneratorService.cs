namespace Castle.NewGenerator.VSNetIntegration
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;
	using Castle.NewGenerator.Core;
	using Castle.NewGenerator.Core.MR;
	using EnvDTE;

	[Guid("9FF77D9F-E4FC-47EE-8E8B-0079FC2F247D")]
	[ProgId("Castle.TestWizard")]
	[ComDefaultInterface(typeof(IDTWizard))]
	[ComVisible(true)]
	public class VSGeneratorService : DefaultGeneratorService, IDTWizard
	{
		public VSGeneratorService() : base(null, null)
		{
		}

		public void Execute(object Application, int hwndOwner, ref object[] ContextParams, ref object[] CustomParams,
		                    ref wizardResult retval)
		{
			DTE dte = (DTE) Application;

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			
			string workingDir =  (string) ContextParams[3];
			string templateDir = @"C:\dev\DotNet\castle\svn\trunk\MonoRail\NewGenerator\GeneratorTemplates\";

			context = new GeneratorContext(workingDir, templateDir);

			//Using NVelocityTemplateEngine. Since we must resolve the lib dir...
			templateEngine = new NVelocityTemplateEngine(templateDir);
			((ISupportInitialize) templateEngine).BeginInit();

			NewProject generator = CreateNewProject(ContextParams);

			generator.Generate(context, this);

			dte.Solution.Open(generator.SolutionFilePath);
		}

		private static NewProject CreateNewProject(object[] contextParams)
		{
			NewProject gen = new NewProject();

			gen.SolutionName = (string) contextParams[5];
			gen.Name = (string) contextParams[1];
			gen.SolutionFolder = ((string) contextParams[2]).Replace("\\"+ gen.Name, "");

			return gen;
		}

		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			// C:\dev\castle\svn\trunk\build\net-2.0\debug
			return Assembly.LoadFile(@"C:\dev\DotNet\castle\svn\trunk\build\net-2.0\debug\" + args.Name);
		}
	}
}
