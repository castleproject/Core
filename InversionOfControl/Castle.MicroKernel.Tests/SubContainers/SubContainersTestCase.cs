// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Tests.SubContainers
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.Tests.ClassComponents;

	/// <summary>
	/// Summary description for SubContainersTestCase.
	/// </summary>
	[TestFixture]
	public class SubContainersTestCase
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void DependenciesSatisfiedAmongContainers()
		{
			IKernel subkernel = new DefaultKernel();

			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);

			subkernel.AddComponent( "spamservice", typeof(DefaultSpamService) );

			DefaultSpamService spamservice = (DefaultSpamService) subkernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}

		[Test]
		public void DependenciesSatisfiedAmongContainersUsingEvents()
		{
			IKernel subkernel = new DefaultKernel();

			subkernel.AddComponent( "spamservice", typeof(DefaultSpamServiceWithConstructor) );

			kernel.AddComponent( "mailsender", typeof(DefaultMailSenderService) );
			kernel.AddComponent( "templateengine", typeof(DefaultTemplateEngine) );

			kernel.AddChildKernel(subkernel);

			DefaultSpamServiceWithConstructor spamservice = 
				(DefaultSpamServiceWithConstructor) subkernel["spamservice"];

			Assert.IsNotNull(spamservice);
			Assert.IsNotNull(spamservice.MailSender);
			Assert.IsNotNull(spamservice.TemplateEngine);
		}
	}
}
