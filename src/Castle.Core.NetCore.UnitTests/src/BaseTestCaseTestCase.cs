// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;

	using Xunit;

	public class BaseTestCaseTestCase : BasePEVerifyTestCase
	{
		public override void TearDown()
		{
			ResetGeneratorAndBuilder(); // we call TearDown ourselves in these test cases
			base.TearDown();
		}

		[Fact]
#if SILVERLIGHT
		[Ignore("This passes in NUnit, but when run in a Browser test harness like UnitDriven this failed because of access to the disk???")]
#endif
		public void TearDown_DoesNotSaveAnything_IfNoProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			base.TearDown();

			Assert.False(File.Exists(path));
		}

#if !NETCORE
		[Fact]
#endif
#if SILVERLIGHT || MONO
		[Ignore("Cannot do in Silverlight or Mono.")]
#endif
		public void TearDown_SavesAssembly_IfProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			generator.CreateClassProxy(typeof(object), new StandardInterceptor());

			base.TearDown();
			Assert.True(File.Exists(path));
		}

		//[Fact]
#if SILVERLIGHT || MONO || NETCORE
	//[Fact(Skip = "Cannot do in Silverlight or Mono.")]
#endif
		// TODO: What to do about expected exceptions
		//[ExpectedException(typeof (AssertionException))]
		public void TearDown_FindsVerificationErrors()
		{
			ModuleBuilder moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
			TypeBuilder invalidType = moduleBuilder.DefineType("InvalidType");
			MethodBuilder invalidMethod = invalidType.DefineMethod("InvalidMethod", MethodAttributes.Public);
			invalidMethod.GetILGenerator().Emit(OpCodes.Ldnull); // missing RET statement

#if !NETCORE
			//TODO: There is a CreateTypeInfo - will that work?
			invalidType.CreateType();
#endif

			if (!IsVerificationDisabled)
			{
				Console.WriteLine("This next test case is expected to yield a verification error.");
			}

			base.TearDown();
		}

		[Fact]
		public void DisableVerification_DisablesVerificationForTestCase()
		{
			DisableVerification();
			TearDown_FindsVerificationErrors();
		}

		[Fact]
		public void DisableVerification_ResetInNextTestCase1()
		{
			Assert.False(IsVerificationDisabled);
			DisableVerification();
			Assert.True(IsVerificationDisabled);
		}

		[Fact]
		public void DisableVerification_ResetInNextTestCase2()
		{
			Assert.False(IsVerificationDisabled);
			DisableVerification();
			Assert.True(IsVerificationDisabled);
		}
	}
}