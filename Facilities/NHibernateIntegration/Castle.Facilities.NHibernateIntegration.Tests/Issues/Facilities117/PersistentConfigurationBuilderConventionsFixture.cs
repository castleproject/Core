using NUnit.Framework;
using Castle.Core.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities117
{
	using Builders;
	using Is=Rhino.Mocks.Constraints.Is;
	using List=Rhino.Mocks.Constraints.List;

	[TestFixture]
	public class PersistentConfigurationBuilderConventionsFixture
	{
		private IConfiguration facilityCfg;

		[SetUp]
		public void SetUp()
		{
			var configurationStore = new DefaultConfigurationStore();
			var resource = new AssemblyResource("Castle.Facilities.NHibernateIntegration.Tests/Issues/Facilities117/facility.xml");
			var xmlInterpreter = new XmlInterpreter(resource);
			xmlInterpreter.ProcessResource(resource, configurationStore);
			facilityCfg = configurationStore.GetFacilityConfiguration("nhibernatefacility").Children["factory"];
		}

		[Test]
		public void Derives_valid_filename_from_session_factory_ID_when_not_explicitly_specified()
		{
			var configurationPersister = MockRepository.GenerateMock<IConfigurationPersister>();
			configurationPersister.Expect(x => x.IsNewConfigurationRequired(null, null))
				.IgnoreArguments()
				.Constraints(Is.Equal("sessionFactory1.dat"), Is.Anything())
				.Return(false);

			var builder = new PersistentConfigurationBuilder(configurationPersister);
			builder.GetConfiguration(facilityCfg);

			configurationPersister.VerifyAllExpectations();
		}

		[Test]
		public void Includes_mapping_assemblies_in_dependent_file_list()
		{
			var configurationPersister = MockRepository.GenerateMock<IConfigurationPersister>();
			configurationPersister.Expect(x => x.IsNewConfigurationRequired(null, null))
				.IgnoreArguments()
				.Constraints(Is.Anything(),
				             List.ContainsAll(new[] {"Castle.Facilities.NHibernateIntegration.Tests.dll" }))
				.Return(false);

			var builder = new PersistentConfigurationBuilder(configurationPersister);
			builder.GetConfiguration(facilityCfg);

			configurationPersister.VerifyAllExpectations();
		}
	
	}
}
