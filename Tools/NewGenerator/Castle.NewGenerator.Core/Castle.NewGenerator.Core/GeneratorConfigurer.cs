namespace Castle.NewGenerator.Core
{
	using Mono.GetOptions;

	public abstract class GeneratorConfigurer<T> : Options where T : IGenerator
	{
		[Option("path", 'p')]
		public string targetPath;

		public virtual void Configure(T generator, string[] args)
		{
			targetPath = generator.TargetPath;
		}
	}
}
