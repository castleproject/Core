// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

#if !DOTNET35
namespace Castle.DynamicProxy.Tests
{
	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyWithMethodsWithOptionalParametersTestCase
	{
		[Test]
		// This previously failed on Mono because Mono doesn't handle nullable default parameters in ParameterBuilder.
		// It appears that in the meantime, DynamicProxy's handling of default values was made more error-tolerant,
		// so this test now passes even on Mono.
		public void CanCreateClassProxy()
		{
			var proxyGenerator = new ProxyGenerator();
			proxyGenerator.CreateClassProxy<ClassWithMethodsWithAllKindsOfOptionalParameters>();
		}
	}
}
#endif