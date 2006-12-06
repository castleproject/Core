namespace Castle.Facilities.ActiveRecordIntegration.Tests
{
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.Windsor;
	using NUnit.Framework;

	[TestFixture]
	public class BugTestCase : AbstractActiveRecordTest
	{
		/// <summary>
		/// <seealso cref="http://support.castleproject.org//browse/FACILITIES-66" />
		/// </summary>
		[Test]
		[ExpectedException(typeof(FacilityException),
			"You need to specify at least one assembly that contains the ActiveRecord classes. For example, <assemblies><item>MyAssembly</item></assemblies>"
			)]
		public void FACILITIES66()
		{
			container = new WindsorContainer();

			IConfiguration confignode = new MutableConfiguration("facility");
			confignode.Children.Add(new MutableConfiguration("arfacility"));
			container.Kernel.ConfigurationStore.AddFacilityConfiguration("arfacility", confignode);

			container.AddFacility("arfacility", new ActiveRecordFacility());
		}
	}
}