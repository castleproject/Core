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

namespace Castle.Windsor.Tests.Installer
{
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class InstallerTestCase
	{
		[Test]
		public void InstallCalcService()
		{
			WindsorContainer container = new WindsorContainer(ConfigHelper.ResolveConfigPath("installerconfig.xml"));

			Assert.IsTrue(container.Kernel.HasComponent(typeof(ICalcService)));
			Assert.IsTrue(container.Kernel.HasComponent("calcservice"));
		}

		[Test]
		public void InstallChildContainer()
		{
			IWindsorContainer container = new WindsorContainer(ConfigHelper.ResolveConfigPath("installerconfig.xml"));
			IWindsorContainer child1 = container.GetChildContainer("child1");

			Assert.IsNotNull(child1);
			Assert.AreEqual(child1.Parent, container);
			Assert.IsTrue(child1.Kernel.HasComponent(typeof(ICalcService)));
			Assert.IsTrue(child1.Kernel.HasComponent("child_calcservice"));

			ICalcService calcservice = container.Resolve("calcservice") as ICalcService;
			ICalcService child_calcservice = child1.Resolve(typeof(ICalcService)) as ICalcService;
			Assert.AreNotEqual(calcservice, child_calcservice);
		}
	}
}

#endif