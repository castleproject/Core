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

namespace Castle.MicroKernel.Test.Interceptor
{
	using NUnit.Framework;

	using Castle.MicroKernel.Interceptor;
	using Castle.MicroKernel.Interceptor.Default;
	using Castle.MicroKernel.Test.Components;

	/// <summary>
	/// Summary description for DefaultInterceptedComponentBuilderTestCase.
	/// </summary>
	[TestFixture]
	public class DefaultInterceptedComponentBuilderTestCase : Assertion
	{
		[Test]
		public void CreateInterceptedComponent()
		{
			IInterceptedComponentBuilder interceptorManager = new DefaultInterceptedComponentBuilder();

			IMailService mailService = new SimpleMailService();

			IInterceptedComponent component = 
				interceptorManager.CreateInterceptedComponent( 
					mailService, typeof(IMailService) );
			AssertNotNull( "Proxy not created", component.ProxiedInstance );

			IMailService proxiedMailService = component.ProxiedInstance as IMailService;
			AssertNotNull( "Could not cast to IMailService", proxiedMailService );
		}
	}
}
