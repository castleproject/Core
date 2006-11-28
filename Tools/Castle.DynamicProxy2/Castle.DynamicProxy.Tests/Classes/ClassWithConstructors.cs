namespace Castle.DynamicProxy.Tests.Classes
{
	using System;

	public class ClassWithConstructors
	{
		private readonly string name;
		private readonly int x;

		public ClassWithConstructors(String name)
		{
			this.name = name;
		}

		public ClassWithConstructors(String name, int x)
		{
			this.name = name;
			this.x = x;
		}

		public string Name
		{
			get { return name; }
		}

		public int X
		{
			get { return x; }
		}
	}
}
