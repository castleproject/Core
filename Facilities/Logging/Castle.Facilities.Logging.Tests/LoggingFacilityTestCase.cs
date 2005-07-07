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

    using NUnit.Framework;

    using Castle.MicroKernel;


	/// <summary>
	/// Summary description for LoggingFacilityTestCase.
	/// </summary>
    [TestFixture]
    public class LoggingFacilityTestCase {
        private IKernel kernel;

        [SetUp]
        public void Setup()
        {
            kernel = new DefaultKernel();
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
            String expectedLogOutput = "hello world";
            String actualLogOutput = "";

            //do something to cause a log message "hello world"
            //dump log output to the actualLogOutput variable

            Assert.IsTrue(expectedLogOutput.Equals(actualLogOutput));
        }
    }
}