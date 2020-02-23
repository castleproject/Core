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

	using Castle.Core.Tests;

	using NUnit.Framework;

	[TestFixture]
	public class BaseTestCaseTestCase : BasePEVerifyTestCase
	{
		public override void TearDown()
		{
			ResetGeneratorAndBuilder(); // we call TearDown ourselves in these test cases
			base.TearDown();
		}

		[Test]
		public void TearDown_DoesNotSaveAnything_IfNoProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			base.TearDown();

			Assert.IsFalse(File.Exists(path));
		}

#if FEATURE_ASSEMBLYBUILDER_SAVE
		[Test]
		public void TearDown_SavesAssembly_IfProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			generator.CreateClassProxy(typeof(object), new StandardInterceptor());

			base.TearDown();
			Assert.IsTrue(File.Exists(path));
		}
#endif

		private void FindVerificationErrors()
		{
			ModuleBuilder moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
			TypeBuilder invalidType = moduleBuilder.DefineType("InvalidType");
			MethodBuilder invalidMethod = invalidType.DefineMethod("InvalidMethod", MethodAttributes.Public);
			invalidMethod.GetILGenerator().Emit(OpCodes.Ldnull); // missing RET statement

#if FEATURE_LEGACY_REFLECTION_API
			invalidType.CreateType();
#else
			invalidType.CreateTypeInfo().AsType();
#endif

			if (!IsVerificationDisabled)
			{
				Console.WriteLine("This next test case is expected to yield a verification error.");
			}

			base.TearDown();
		}

#if FEATURE_ASSEMBLYBUILDER_SAVE
		[Test]
		[ExcludeOnFramework(Framework.Mono, "Mono doesn't have peverify, so we can't perform verification.")]
		public void TearDown_FindsVerificationErrors()
		{
			var ex = Assert.Throws<AssertionException>(() => FindVerificationErrors());
			StringAssert.Contains("PeVerify reported error(s)", ex.Message);
			StringAssert.Contains("fall through end of the method without returning", ex.Message);
		}
#endif

		[Test]
		public void DisableVerification_DisablesVerificationForTestCase()
		{
			DisableVerification();

			FindVerificationErrors();
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase1()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase2()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}
	}
}
