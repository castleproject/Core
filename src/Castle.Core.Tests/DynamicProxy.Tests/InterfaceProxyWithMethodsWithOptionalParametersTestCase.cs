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

#if !DOTNET35 && !SILVERLIGHT
namespace Castle.DynamicProxy.Tests
{
	using CastleTests.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InterfaceProxyWithMethodsWithOptionalParametersTestCase
	{
		[Test]
#if __MonoCS__
		// Seems like mono is too strict, and doesn't handle a nullable default parameter in ParameterBuilder
		// https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Reflection.Emit/ParameterBuilder.cs#L101
		[Ignore("System.ArgumentException : Constant does not match the defined type.")]
#endif
		public void CanCreateInterfaceProxy()
		{
			var proxyGenerator = new ProxyGenerator();
			proxyGenerator.CreateInterfaceProxyWithoutTarget<InterfaceWithMethodsWithAllKindsOfOptionalParameters>();
		}
	}
}
#endif