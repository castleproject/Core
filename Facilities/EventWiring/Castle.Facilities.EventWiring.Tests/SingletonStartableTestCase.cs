namespace Castle.Facilities.EventWiring.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class SingletonStartableTestCase : WiringTestBase
	{
		protected override string GetConfigFile()
		{
			return "config/startable.config";
		}
	}
}
