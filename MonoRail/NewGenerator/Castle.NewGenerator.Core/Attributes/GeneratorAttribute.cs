namespace Castle.NewGenerator.Core
{
	using System;

	public class GeneratorAttribute : Attribute
	{
		private readonly string id;
		private readonly string fullName;

		public GeneratorAttribute(string id, string fullName)
		{
			this.id = id;
			this.fullName = fullName;
		}

		public string Id
		{
			get { return id; }
		}

		public string FullName
		{
			get { return fullName; }
		}
	}
}
