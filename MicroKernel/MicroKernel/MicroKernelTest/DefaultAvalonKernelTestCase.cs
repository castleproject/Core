using Castle.MicroKernel.Handler;
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

	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for DefaultAvalonKernelTestCase.
	/// </summary>
	[TestFixture]
	public class DefaultAvalonKernelTestCase : Assertion
	{
		[Test]
		public void Creation()
		{
			IAvalonKernel kernel = new DefaultAvalonKernel();
			AssertNotNull(kernel);
		}

		/// <summary>
		/// Just a simple Service resolution.
		/// No concerns or aspects involved.
		/// </summary>
		[Test]
		public void SimpleUsage()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(SimpleMailService) );

			IHandler handler = container[ "a" ];

			IMailService service = handler.Resolve() as IMailService;

			AssertNotNull( service );

			service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");

			handler.Release( service );
		}

		[Test]
		public void SimpleAvalonComponent()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService2) );

			IHandler handler = container[ "a" ];

			IMailService service = handler.Resolve() as IMailService;

			AssertNotNull( service );

			AvalonMailService realInstance = (AvalonMailService) service;

			Assert( realInstance.initialized );
			Assert( realInstance.configured );
			Assert( !realInstance.disposed );

			service.Send("hammett at apache dot org", "johndoe at yahoo dot org", "Aloha!", "What's up?");

			handler.Release( service );

			Assert( realInstance.disposed );
		}

		[Test]
		public void AvalonComponentWithUnsatisfiedDependencies()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "b", typeof(ISpamService), typeof(AvalonSpamService) );

			IHandler handler = container[ "b" ];

			try
			{
				ISpamService service = handler.Resolve() as ISpamService;
				Fail("Dependencies unsatisfied for this component. Resolve should fail.");
			}
			catch(HandlerException)
			{
				// Expected.
			}
		}

		[Test]
		public void AvalonComponentWithDependencies()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService) );
			container.AddComponent( "b", typeof(ISpamService), typeof(AvalonSpamService) );

			IHandler handler = container[ "b" ];

			ISpamService service = handler.Resolve() as ISpamService;
			AssertNotNull( service );

			service.AnnoyPeople( "Work at home and earn a thousand dollars per second!" );

			handler.Release( service );
		}

		[Test]
		public void HybridAvalonComponent()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService) );
			container.AddComponent( "b", typeof(ISpamService), typeof(AvalonSpamService2) );

			IHandler handler = container[ "b" ];

			ISpamService service = handler.Resolve() as ISpamService;
			AssertNotNull( service );

			service.AnnoyPeople( "Work at home and earn a thousand dollars per second!" );

			handler.Release( service );
		}

		[Test]
		public void HybridAvalonComponentUsingSetters()
		{
			IAvalonKernel container = new DefaultAvalonKernel();
			container.AddComponent( "a", typeof(IMailService), typeof(AvalonMailService) );
			container.AddComponent( "b", typeof(ISpamService2), typeof(AvalonSpamService3) );

			IHandler handler = container[ "b" ];

			ISpamService service = handler.Resolve() as ISpamService;
			AssertNotNull( service );

			service.AnnoyPeople( "Work at home and earn a thousand dollars per second!" );

			handler.Release( service );
		}
	}
}
