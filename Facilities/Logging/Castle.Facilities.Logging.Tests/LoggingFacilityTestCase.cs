// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.Logging.Tests
{
    using System;
    using System.IO;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Model.Configuration;
    using Castle.Windsor;
    using NUnit.Framework;

	/// <summary>
	/// Summary description for LoggingFacilityTestCase.
	/// </summary>
    [TestFixture]
    public class LoggingFacilityTestCase {
        private IWindsorContainer log4netContainer;
        private IWindsorContainer nlogContainer;
        private StringWriter outWriter = new StringWriter();
        private StringWriter errorWriter = new StringWriter();

        [SetUp]
        public void Setup()
        {
            log4netContainer = CreateConfiguredContainerForlog4net();
            log4netContainer.AddFacility("logging", new LoggingFacility());
            nlogContainer = CreateConfiguredContainerForNLog();
            nlogContainer.AddFacility("logging", new LoggingFacility());

            outWriter.GetStringBuilder().Length = 0;
            errorWriter.GetStringBuilder().Length = 0;

            Console.SetOut(outWriter);
            Console.SetError(errorWriter);
        }

        [TearDown]
        public void Teardown()
        {
            log4netContainer.Dispose();
            nlogContainer.Dispose();
        }

        [Test]
        public void Simplelog4netTest() {
            String expectedLogOutput = String.Format("INFO  {0} - Hello world\r\n", typeof(Classes.LoggingComponent).ToString());
            String actualLogOutput = "";

            log4netContainer.AddComponent("component", typeof(Classes.LoggingComponent));
            Classes.LoggingComponent test = log4netContainer["component"] as Classes.LoggingComponent;

            test.HelloWorld();

            actualLogOutput = outWriter.GetStringBuilder().ToString();
            Assert.AreEqual(expectedLogOutput, actualLogOutput);
        }

        [Test]
        public void SimpleNLogTest() 
        {
            String expectedLogOutput = String.Format("INFO  {0} - Hello world\r\n", typeof(Classes.LoggingComponent).ToString());
            String actualLogOutput = "";

            nlogContainer.AddComponent("component", typeof(Classes.LoggingComponent));
            Classes.LoggingComponent test = nlogContainer["component"] as Classes.LoggingComponent;

            test.HelloWorld();

            actualLogOutput = outWriter.GetStringBuilder().ToString();
            Assert.AreEqual(expectedLogOutput, actualLogOutput);
        }

        protected virtual IWindsorContainer CreateConfiguredContainerForlog4net()
        {
            IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

            MutableConfiguration confignode = new MutableConfiguration("facility");

            confignode.Children.Add(new MutableConfiguration("framework", "log4net"));
            confignode.Children.Add(new MutableConfiguration("config", "log4net.config"));
            confignode.Children.Add(new MutableConfiguration("interception", "false"));            

            container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", confignode);

            return container;
        }

        protected virtual IWindsorContainer CreateConfiguredContainerForNLog()
        {
            IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

            MutableConfiguration confignode = new MutableConfiguration("facility");

            confignode.Children.Add(new MutableConfiguration("framework", "nlog"));
            confignode.Children.Add(new MutableConfiguration("config", "nlog.config"));
            confignode.Children.Add(new MutableConfiguration("interception", "false"));            

            container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", confignode);

            return container;
        }
    }
}