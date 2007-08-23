namespace Castle.NewGenerator.Core
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class GeneratorOptionsAttribute : Attribute
	{
		private readonly Type optionClass;

		public GeneratorOptionsAttribute(Type optionClass)
		{
			this.optionClass = optionClass;
		}

		public Type OptionClass
		{
			get { return optionClass; }
		}
	}
}
