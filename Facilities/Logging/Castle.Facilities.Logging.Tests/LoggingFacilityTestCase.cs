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
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Model.Configuration;
    using Castle.Windsor;
    using NUnit.Framework;

    using Castle.MicroKernel;


	/// <summary>
	/// Summary description for LoggingFacilityTestCase.
	/// </summary>
    [TestFixture]
    public class LoggingFacilityTestCase {
        private IWindsorContainer kernel;

        [SetUp]
        public void Setup()
        {
            kernel = CreateConfiguredContainer();
            kernel.AddFacility("logging", new LoggingFacility());
        }

        [TearDown]
        public void Teardown()
        {
            kernel.Dispose();
            kernel = null;
        }

        [Test]
        public void SimpleTest() {
            String expectedLogOutput = "HellowWorld";
            String actualLogOutput = "";

            kernel.AddComponent("component", typeof(Classes.LoggingComponent));

            Classes.LoggingComponent test = kernel["component"] as Classes.LoggingComponent;

            test.HelloWorld(); //should cause some kind of HelloWorld to be logged.

            
            //dump log output to the actualLogOutput variable

            Assert.IsTrue(expectedLogOutput.Equals(actualLogOutput));
        }

        protected virtual IWindsorContainer CreateConfiguredContainer()
        {
            IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

            MutableConfiguration confignode = new MutableConfiguration("facility");

            confignode.Children.Add(new MutableConfiguration("framework", "log4net"));
            confignode.Children.Add(new MutableConfiguration("config", "logging.config"));
            confignode.Children.Add(new MutableConfiguration("intercept", "false"));
            

            container.Kernel.ConfigurationStore.AddFacilityConfiguration("logging", confignode);

            return container;
        }
    }
}