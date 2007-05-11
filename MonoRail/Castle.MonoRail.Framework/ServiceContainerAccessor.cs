namespace Castle.MonoRail.Framework
{
	using System.ComponentModel.Design;

	/// <summary>
	/// Exposes a single point to expose MonoRail's Container
	/// </summary>
	public class ServiceContainerAccessor
	{
		private static IServiceContainer container;

		public static IServiceContainer ServiceContainer
		{
			get { return container; }
			set { container = value; }
		}
	}
}
