namespace Castle.MonoRail.Views.Brail.Macros
{
	using System;

	/// <summary>
	/// we need to have a unique name, because of MR 371, nested components should not use the same name.
	/// </summary>
	public class ComponentNaming
	{
		public static string GetComponentNameFor(object obj)
		{
			return "component" + obj.GetHashCode();
		}

		public static string GetComponentContextName(object obj)
		{
			return "componentContext" + obj.GetHashCode();
		}

		public static string GetComponentFactoryName(object obj)
		{
			return "viewComponentFactory" + obj;
		}
	}
}