// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Tests
{
	using Castle.MicroKernel.Handlers;
	using NUnit.Framework;

	using System.Collections;

	using Castle.MicroKernel.Tests.RuntimeParameters;


	[TestFixture]
	public class RuntimeParametersTestCase
	{
		private IKernel kernel;
		private Hashtable deps;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
			kernel.AddComponent("compa", typeof(CompA));
			kernel.AddComponent("compb", typeof(CompB));

			deps = new Hashtable();
			deps.Add("cc", new CompC(12));
			deps.Add("myArgument", "ernst");
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		[ExpectedException(typeof(HandlerException), "Can't create component 'compb' as it has " +
			"dependencies to be satisfied. \r\ncompb is waiting for the following dependencies: \r\n\r\n" +
			"Services: \r\n- Castle.MicroKernel.Tests.RuntimeParameters.CompC which was not registered. \r\n")]
		public void WithoutParameters()
		{
			CompB compb = kernel[typeof(CompB)] as CompB;
		}

		[Test]
		public void ResolveUsingParameters()
		{
			CompB compb = kernel.Resolve(typeof(CompB), deps) as CompB;

			AssertDependencies(compb);
		}

		[Test]
		public void ResolveUsingParametersWithinTheHandler()
		{
			kernel.RegisterCustomDependencies("compb", deps);
			CompB compb = kernel[typeof(CompB)] as CompB;

			AssertDependencies(compb);
		}

		[Test]
		public void ParametersPrecedence()
		{
			kernel.RegisterCustomDependencies("compb", deps);

			CompB instance_with_model = (CompB)kernel[typeof(CompB)];
			Assert.AreSame(deps["cc"], instance_with_model.Compc, "Model dependency should override kernel dependency");

			Hashtable deps2 = new Hashtable();
			deps2.Add("cc", new CompC(12));
			deps2.Add("myArgument", "ayende");
	
			CompB instance_with_args = (CompB) kernel.Resolve(typeof(CompB), deps2);
			
			Assert.AreSame(deps2["cc"],instance_with_args.Compc, "Should get it from resolve params");
			Assert.AreEqual("ayende", instance_with_args.MyArgument);
		}
		
		private void AssertDependencies(CompB compb)
		{
			Assert.IsNotNull(compb, "Component B should have been resolved");

			Assert.IsNotNull(compb.Compc, "CompC property should not be null");
			Assert.IsTrue(compb.MyArgument != string.Empty, "MyArgument property should not be empty");

			Assert.AreSame(deps["cc"], compb.Compc, "CompC property should be the same instnace as in the hashtable argument");
			Assert.IsTrue("ernst".Equals(compb.MyArgument),string.Format( "The MyArgument property of compb should be equal to ernst, found {0}", compb.MyArgument));
		}
	}
}
