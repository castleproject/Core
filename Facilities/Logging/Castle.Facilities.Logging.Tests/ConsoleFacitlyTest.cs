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
    using Castle.Windsor;
    using NUnit.Framework;

    /// <summary>
	/// Summary description for ConsoleFacitlyTest.
	/// </summary>
	[TestFixture]
	public class ConsoleFacitlyTest : BaseTest
	{
        private IWindsorContainer container;
        private StringWriter outWriter = new StringWriter();
        private StringWriter errorWriter = new StringWriter();

        [SetUp]
        public void Setup()
        {
            container = base.CreateConfiguredContainer("console", "", "true");
            

            outWriter.GetStringBuilder().Length = 0;
            errorWriter.GetStringBuilder().Length = 0;

            Console.SetOut(outWriter);
            Console.SetError(errorWriter);
        }

        [TearDown]
        public void Teardown()
        {
            container.Dispose();
        }

        [Test]
        public void SimpleTest() 
        {
            String expectedLogOutput = String.Format("[Debug] '{0}' Method Name: HelloWorld\r\n", typeof(LoggingInterceptor).ToString());
            String actualLogOutput = "";

            container.AddComponent("component", typeof(Classes.LoggingComponent));
            Classes.LoggingComponent test = container["component"] as Classes.LoggingComponent;

            test.HelloWorld();

            actualLogOutput = outWriter.GetStringBuilder().ToString();
            Assert.AreEqual(expectedLogOutput, actualLogOutput);
        }
	}
}
