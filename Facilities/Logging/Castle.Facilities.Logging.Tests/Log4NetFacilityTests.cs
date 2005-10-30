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
	/// Summary description for Log4NetFacilityTests.
	/// </summary>
	[TestFixture, Ignore("Dont think we are able to hook Console Output here")]
	public class Log4NetFacilityTests : BaseTest
	{
        private IWindsorContainer container;
        private StringWriter outWriter = new StringWriter();
        private StringWriter errorWriter = new StringWriter();

        [SetUp]
        public void Setup()
        {
            container = base.CreateConfiguredContainer(LoggerImplementation.Log4net);            

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
            container.AddComponent("component", typeof(Classes.LoggingComponent));
            Classes.LoggingComponent test = container["component"] as Classes.LoggingComponent;

            test.DoSomething();

			String expectedLogOutput = String.Format("[Info] '{0}' Hello world\r\n", typeof(Classes.LoggingComponent).FullName);
			String actualLogOutput = outWriter.GetStringBuilder().ToString();
            Assert.AreEqual(expectedLogOutput, actualLogOutput);
        }
	}
}
