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

namespace Castle.MicroKernel.Test
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Model.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for DependencyDisposeTestCase.
	/// </summary>
	[TestFixture]
	public class DependencyDisposeTestCase : Assertion
	{
		[Test]
		public void SingletonDependencyDisposal()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService) );
			container.AddComponent( "b", typeof(ISpamService2), typeof(AvalonSpamService3) );

			IHandler handler = container[ "b" ];
			ISpamService2 spamService = handler.Resolve() as ISpamService2;
			AssertNotNull( spamService );

			AssertNotNull( spamService.MailService );
			AvalonMailService mailService = (AvalonMailService) spamService.MailService;

			Assert( !mailService.disposed );

			handler.Release( spamService );

			Assert( "A singleton component should have not been disposed", !mailService.disposed );
		}

		[Test]
		public void TransientDependencyDisposal()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService2) );
			container.AddComponent( "b", typeof(ISpamService2), typeof(AvalonSpamService3) );

			IHandler handler = container[ "b" ];
			ISpamService2 spamService = handler.Resolve() as ISpamService2;
			AssertNotNull( spamService );

			AssertNotNull( spamService.MailService );
			AvalonMailService mailService = (AvalonMailService) spamService.MailService;

			Assert( !mailService.disposed );

			handler.Release( spamService );

			Assert( "A transient component should have been disposed", mailService.disposed );
		}
	}
}
