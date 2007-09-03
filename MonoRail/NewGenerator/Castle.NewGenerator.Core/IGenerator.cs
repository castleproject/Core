namespace Castle.NewGenerator.Core
{
	public interface IGenerator
	{
		string TargetPath { get; set; }

		void Generate(GeneratorContext context, IGeneratorService generator);
	}
}
