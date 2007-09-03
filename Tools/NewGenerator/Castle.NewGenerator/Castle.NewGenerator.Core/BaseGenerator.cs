namespace Castle.NewGenerator.Core
{
	using System;

	public abstract class BaseGenerator : IGenerator
	{
		private string targetPath;

		protected BaseGenerator()
		{
			targetPath = AppDomain.CurrentDomain.BaseDirectory;
		}

		[Param]
		public string TargetPath
		{
			get { return targetPath; }
			set { targetPath = value; }
		}

		public abstract void Generate(GeneratorContext context, IGeneratorService generator);
	}
}
