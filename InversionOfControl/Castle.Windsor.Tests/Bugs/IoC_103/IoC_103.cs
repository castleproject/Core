// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT // we do not support xml config on SL

using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using NUnit.Framework;
using Castle.Windsor.Configuration.Interpreters;

namespace Castle.Windsor.Tests.Bugs.IoC_103 {
	[TestFixture]
	public class IoC103 {

		const string SAMPLE1XMLFILENAME =
 @"Bugs/IoC_103/sample1.xml"
;

		[Test]
		public void TestInvalidNodeNameInConfigurationExceptionRaised() {
			bool hasfailed = false;
			try {
				IWindsorContainer container = new WindsorContainer(new XmlInterpreter(ConfigHelper.ResolveConfigPath(SAMPLE1XMLFILENAME)));
			}
			catch (Exception e) {
				hasfailed = true;
				// expected exception
				// but verify message contains valid verbose enough cause
				string message = e.Message;

				string explaintestfail = "the exception message is expected to contains 'conteainers',' facilities',' components', 'bootstrap' and such explaination, but was '"+message+"'";
				Assert.IsTrue(message.Contains("<containers>"), explaintestfail);
                // At that point of parsing, properties and includes are not resolved, so they will not be mentioned.
                // Assert.IsTrue(message.Contains("<include/>"), explaintestfail);
                // Assert.IsTrue(message.Contains("<properties>"), explaintestfail);
				Assert.IsTrue(message.Contains("<facilities>"), explaintestfail);
				Assert.IsTrue(message.Contains("<components>"), explaintestfail);
				Assert.IsTrue(message.Contains("<bootstrap>"), explaintestfail);

			}
			finally {
				Assert.IsTrue(hasfailed, "it should have failed");
			}


		}
	}
}

#endif