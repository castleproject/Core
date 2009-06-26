using NUnit.Framework;
using System.IO;
using Castle.Core.Configuration;
using System.Xml;
using Castle.Core.Configuration.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities116
{
	using System;
	using System.Threading;
	using Builders;


	[TestFixture]
	public class Fixture : IssueTestCase
	{
		protected override string ConfigurationFile
		{
			get
			{
				return "EmptyConfiguration.xml";
			}
		}
		private const string filename="myconfig.dat";
		private IConfiguration configuration;
		private IConfigurationBuilder configurationBuilder;
		public override void OnSetUp()
		{
			var configurationStore = new DefaultConfigurationStore();
			var resource = new AssemblyResource("Castle.Facilities.NHibernateIntegration.Tests/Issues/Facilities116/facility.xml");
			var xmlInterpreter = new XmlInterpreter(resource);
			xmlInterpreter.ProcessResource(resource, configurationStore);
			this.configuration = configurationStore.GetFacilityConfiguration("nhibernatefacility").Children["factory"];
			this.configurationBuilder = new PersistentConfigurationBuilder();
		}
		public override void OnTearDown()
		{
			File.Delete(filename);
		}


		[Test]
		public void Can_create_serialized_file_in_the_disk()
		{
			Assert.IsFalse(File.Exists(filename));
			Configuration cfg= configurationBuilder.GetConfiguration(configuration);
			Assert.IsTrue(File.Exists(filename));
			BinaryFormatter bf = new BinaryFormatter();
			Configuration nhConfig;
			using(var fileStream=new FileStream(filename,FileMode.Open))
			{
				nhConfig = bf.Deserialize(fileStream) as Configuration;
			}
			Assert.IsNotNull(nhConfig);
			nhConfig.BuildSessionFactory();
		}

		[Test]
		public void Can_deserialize_file_from_the_disk_if_new_enough()
		{
			Assert.IsFalse(File.Exists(filename));
			Configuration nhConfig = configurationBuilder.GetConfiguration(this.configuration);
			Assert.IsTrue(File.Exists(filename));
			DateTime dateTime = File.GetLastWriteTime(filename);
			Thread.Sleep(1000);
			nhConfig = configurationBuilder.GetConfiguration(this.configuration);
			Assert.AreEqual(File.GetLastWriteTime(filename), dateTime);
			Assert.IsNotNull(configuration);
			nhConfig.BuildSessionFactory();
		}

		[Test]
		public void Can_deserialize_file_from_the_disk_if_one_of_the_dependencies_is_newer()
		{
			Assert.IsFalse(File.Exists(filename));
			Configuration nhConfig = configurationBuilder.GetConfiguration(this.configuration);
			Assert.IsTrue(File.Exists(filename));
			DateTime dateTime = File.GetLastWriteTime(filename);
			Thread.Sleep(1000);
			DateTime dateTime2 = DateTime.Now;
			File.Create("SampleDllFile").Dispose();
			File.SetLastWriteTime("SampleDllFile", dateTime2);
			nhConfig = configurationBuilder.GetConfiguration(this.configuration);
			Assert.Greater(File.GetLastWriteTime(filename), dateTime2);
			Assert.IsNotNull(configuration);
			nhConfig.BuildSessionFactory();
		}
	}
}
