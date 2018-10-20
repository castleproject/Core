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
#if FEATURE_CUSTOMMODIFIERS && FEATURE_ASSEMBLYBUILDER_SAVE

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class CustomModifiersTestCase : BasePEVerifyTestCase
	{
		private static Dictionary<string, Type[]> customModifiers;
		private Dictionary<string, Type> generatedTypes;

		static CustomModifiersTestCase()
		{
			// `GenerateTypes` will generate four types per dictionary entry:
			//
			// 1. One type will be called `<key>_AsModoptOnParamType`. This type will have
			//    one method `Foo` with a return type of `void` and a parameter of
			//    type `System.Int32` with the given modopts.
			//
			// 2. One type will be called `<key>_AsModreqOnParamType`. This type will have
			//    one method `Foo` with a return type of `void` and a parameter of
			//    type `System.Int32` with the given modreqs.
			//
			// 3. Another type will be called `<key>_AsModoptOnReturnType`. This type will
			//    also have one method `Foo` with a return type of `System.Int32`
			//    with the given modopts, and no parameters.
			//
			// 4. Another type will be called `<key>_AsModreqOnReturnType`. This type will
			//    also have one method `Foo` with a return type of `System.Int32`
			//    with the given modreqs, and no parameters.
			CustomModifiersTestCase.customModifiers = new Dictionary<string, Type[]>()
			{
				["Bar"] = new[] { typeof(Bar) },
				["Bar_IsConst"] = new[] { typeof(Bar), typeof(IsConst) },
				["Foo_Bar"] = new[] { typeof(Foo), typeof(Bar) },
				["Foo_Bar_IsConst"] = new[] { typeof(Foo), typeof(Bar), typeof(IsConst) },
				["IsByValue"] = new[] { typeof(IsByValue) },
				["IsByValue_IsByValue"] = new[] { typeof(IsByValue), typeof(IsByValue) },
				["IsByValue_IsConst"] = new[] { typeof(IsByValue), typeof(IsConst) },
				["IsByValue_IsConst_IsLong"] = new[] { typeof(IsByValue), typeof(IsConst), typeof(IsLong) },
				["IsByValue_IsLong"] = new[] { typeof(IsByValue), typeof(IsLong) },
				["IsConst"] = new[] { typeof(IsConst) },
				["IsConst_IsConst"] = new[] { typeof(IsConst), typeof(IsConst) },
				["IsConst_IsLong"] = new[] { typeof(IsConst), typeof(IsLong) },
				["IsLong"] = new[] { typeof(IsLong) },
				["IsLong_IsConst"] = new[] { typeof(IsLong), typeof(IsConst) },
				["IsLong_IsLong"] = new[] { typeof(IsLong), typeof(IsLong) },
			};
		}

		public CustomModifiersTestCase()
		{
			// This dictionary will be filled by the `GenerateTypes` one-time setup method:
			this.generatedTypes = new Dictionary<string, Type>();
		}

		public static IEnumerable<string> AsModoptOnParamTypeNames
			=> CustomModifiersTestCase.customModifiers.Keys.Select(typeName => typeName + "_AsModoptOnParamType");

		public static IEnumerable<string> AsModreqOnParamTypeNames
			=> CustomModifiersTestCase.customModifiers.Keys.Select(typeName => typeName + "_AsModreqOnParamType");

		public static IEnumerable<string> AsModoptOnReturnTypeNames
			=> CustomModifiersTestCase.customModifiers.Keys.Select(typeName => typeName + "_AsModoptOnReturnType");

		public static IEnumerable<string> AsModreqOnReturnTypeNames
			=> CustomModifiersTestCase.customModifiers.Keys.Select(typeName => typeName + "_AsModreqOnReturnType");

		public static IEnumerable<string> AllTypeNames
			=> AsModoptOnParamTypeNames
			   .Concat(AsModreqOnParamTypeNames)
			   .Concat(AsModoptOnReturnTypeNames)
			   .Concat(AsModreqOnReturnTypeNames);

		/// <summary>
		/// Emits types as specified by `this.modopts` and stores the dynamic assembly to disk.
		/// The generated types are added to the `this.generatedTypes` dictionary for reference
		/// in unit tests.
		/// </summary>
		[OneTimeSetUp]
		public void GenerateTypes()
		{
			const string assemblyName = "Castle.Core.Tests.CustomModifiersTestCaseDynamicAssembly";
			const string assemblyFileName = "Castle.Core.Tests.CustomModifiersTestCaseDynamicAssembly.dll";

			var moduleScope = new ModuleScope(
				savePhysicalAssembly: true,
				disableSignedModule: false,
				namingScope: new Generators.NamingScope(),
				strongAssemblyName: assemblyName,
				strongModulePath: assemblyFileName,
				weakAssemblyName: assemblyName,
				weakModulePath: assemblyFileName);

			foreach (var partialTypeName in CustomModifiersTestCase.customModifiers.Keys)
			{
				var customModifiers = CustomModifiersTestCase.customModifiers[partialTypeName];
				this.AddTypeWithCustomModifiersAsModoptOnParamType(moduleScope, partialTypeName, customModifiers);
				this.AddTypeWithCustomModifiersAsModreqOnParamType(moduleScope, partialTypeName, customModifiers);
				this.AddTypeWithCustomModifiersAsModoptOnReturnType(moduleScope, partialTypeName, customModifiers);
				this.AddTypeWithCustomModifiersAsModreqOnReturnType(moduleScope, partialTypeName, customModifiers);
			}

#if FEATURE_TEST_PEVERIFY
			// Let's persist and PE-verify the dynamic assembly before it gets used in tests:
			var assemblyPath = moduleScope.SaveAssembly();
			base.RunPEVerifyOnGeneratedAssembly(assemblyPath);
#endif
		}

		/// <summary>
		/// This test checks whether reflection correctly reports back the modopts on
		/// the parameter type in the generated types.
		/// </summary>
		[TestCaseSource(nameof(AsModoptOnParamTypeNames))]
		[ExcludeOnFramework(Framework.Mono, "Mono reports custom modifiers in the opposite order than the CLR does. See https://github.com/castleproject/Core/issues/414 and https://github.com/mono/mono/issues/11302.")]
		public void ReflectionReturnsCorrectModoptOnParamTypeForGeneratedType(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			const string suffix = "_AsModoptOnParamType";
			Assume.That(typeName.EndsWith(suffix));

			var typeNameWithoutSuffix = typeName.Substring(0, typeName.Length - suffix.Length);
			Assume.That(CustomModifiersTestCase.customModifiers.ContainsKey(typeNameWithoutSuffix));

			var modopts = this.generatedTypes[typeName].GetMethod("Foo").GetParameters()[0].GetOptionalCustomModifiers();

			CollectionAssert.AreEqual(expected: CustomModifiersTestCase.customModifiers[typeNameWithoutSuffix].Reverse(), actual: modopts);
			// ^ The emission of custom modifiers performed by DynamicProxy is currently geared towards
			//   the CLR, which reports custom modifiers in reverse order. On Mono, before version 5.16,
			//   Reflection would not report custom modifiers at all; this has now changed. But unlike the
			//   CLR, Reflection on Mono reports custom modifiers in non-reversed order, which makes the
			//   above assertion fail. What probably needs to be done is either of the following:
			//
			//   1. Wait for Mono to become more compatible with CLR and reverse cmod order too;
			//      see https://github.com/mono/mono/issues/11302. Or,
			//
			//   2. Adjust DynamicProxy so it automatically detects whether it needs to reverse cmod order
			//      after reading them, either by checking the current framework or running a pretest.
		}

		/// <summary>
		/// This test checks whether reflection correctly reports back the modreqs on
		/// the parameter type in the generated types.
		/// </summary>
		[TestCaseSource(nameof(AsModreqOnParamTypeNames))]
		[ExcludeOnFramework(Framework.Mono, "Mono reports custom modifiers in the opposite order than the CLR does. See https://github.com/castleproject/Core/issues/414 and https://github.com/mono/mono/issues/11302.")]
		public void ReflectionReturnsCorrectModreqsOnParamTypeForGeneratedType(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			const string suffix = "_AsModreqOnParamType";
			Assume.That(typeName.EndsWith(suffix));

			var typeNameWithoutSuffix = typeName.Substring(0, typeName.Length - suffix.Length);
			Assume.That(CustomModifiersTestCase.customModifiers.ContainsKey(typeNameWithoutSuffix));

			var modreqs = this.generatedTypes[typeName].GetMethod("Foo").GetParameters()[0].GetRequiredCustomModifiers();
			Assume.That(modreqs.Length > 0); // If this fails on mono/linux we have to revisit the commits and issues for IL method custom modifiers. https://github.com/castleproject/Core/issues/277

			CollectionAssert.AreEqual(expected: CustomModifiersTestCase.customModifiers[typeNameWithoutSuffix].Reverse(), actual: modreqs);
			// ^ see comment about `.Reverse()` above.
		}

		/// <summary>
		/// This test checks whether reflection correctly reports back the modopts on
		/// the parameter type in the generated types.
		/// </summary>
		[TestCaseSource(nameof(AsModoptOnReturnTypeNames))]
		public void ReflectionReturnsCorrectModoptOnReturnTypeForGeneratedType(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			const string suffix = "_AsModoptOnReturnType";
			Assume.That(typeName.EndsWith(suffix));

			var typeNameWithoutSuffix = typeName.Substring(0, typeName.Length - suffix.Length);
			Assume.That(CustomModifiersTestCase.customModifiers.ContainsKey(typeNameWithoutSuffix));

			var modopts = this.generatedTypes[typeName].GetMethod("Foo").ReturnParameter.GetOptionalCustomModifiers();
			Assume.That(modopts.Length > 0); // If this fails on mono/linux we have to revisit the commits and issues for IL method custom modifiers. https://github.com/castleproject/Core/issues/277

			CollectionAssert.AreEqual(expected: CustomModifiersTestCase.customModifiers[typeNameWithoutSuffix].Reverse(), actual: modopts);
			// ^ see comment about `.Reverse()` above.
		}

		/// <summary>
		/// This test checks whether reflection correctly reports back the modopts on
		/// the return type in the generated types.
		/// </summary>
		[TestCaseSource(nameof(AsModreqOnReturnTypeNames))]
		public void ReflectionReturnsCorrectModreqOnReturnTypeForGeneratedType(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			const string suffix = "_AsModreqOnReturnType";
			Assume.That(typeName.EndsWith(suffix));

			var typeNameWithoutSuffix = typeName.Substring(0, typeName.Length - suffix.Length);
			Assume.That(CustomModifiersTestCase.customModifiers.ContainsKey(typeNameWithoutSuffix));

			var modreqs = this.generatedTypes[typeName].GetMethod("Foo").ReturnParameter.GetRequiredCustomModifiers();
			Assume.That(modreqs.Length > 0); // If this fails on mono/linux we have to revisit the commits and issues for IL method custom modifiers. https://github.com/castleproject/Core/issues/277

			CollectionAssert.AreEqual(expected: CustomModifiersTestCase.customModifiers[typeNameWithoutSuffix].Reverse(), actual: modreqs);
			// ^ see comment about `.Reverse()` above.
		}

		/// <summary>
		/// This test checks whether Castle can generate proxies for types having a method
		/// whose signature contains certain combinations of modopts or modreqs either on
		/// the parameter type or on the return type.
		/// </summary>
		[TestCaseSource(nameof(AllTypeNames))]
		public void CanGenerateProxyOfTypeWithCustomModifiers(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			var type = this.generatedTypes[typeName];
			var proxyGenerator = new ProxyGenerator();

			var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new DoNothingInterceptor());
			Assert.NotNull(proxy);
		}

		private void AddTypeWithCustomModifiersAsModoptOnParamType(ModuleScope moduleScope, string typeName, params Type[] paramTypeModopts)
		{
			// This method generates a type that would look as follows in IL:
			//
			// .class interface public abstract auto ansi beforefieldinit <typeName>
			// {
			//     .method public hidebysig newslot abstract virtual instance void Foo(int32 modopt(<modopt1>) modopt(<modopt2>) arg) cil managed { }
			// }
			//
			// The C++/CLI equivalent would look like this (but note that only some modopts
			// used in this test fixture have a corresponding C++/CLI modifier):
			//
			// public interface class <typeName> {
			//     public virtual void Foo(<additional_modifiers> signed int arg);
			// };

			typeName += "_AsModoptOnParamType";

			var typeBuilder = moduleScope.DefineType(
				true,
				typeName,
				TypeAttributes.Class | TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var methodBuilder = typeBuilder.DefineMethod(
				"Foo",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual,
				returnType: typeof(void),
				returnTypeRequiredCustomModifiers: null,
				returnTypeOptionalCustomModifiers: null,
				parameterTypes: new[]
				{
					typeof(int)
				},
				parameterTypeRequiredCustomModifiers: null,
				parameterTypeOptionalCustomModifiers: new[]
				{
					paramTypeModopts
				},
				callingConvention: CallingConventions.Standard);

			this.generatedTypes.Add(typeName, typeBuilder.CreateType());
		}

		private void AddTypeWithCustomModifiersAsModreqOnParamType(ModuleScope moduleScope, string typeName, params Type[] paramTypeModreqs)
		{
			// This method generates a type that would look as follows in IL:
			//
			// .class interface public abstract auto ansi beforefieldinit <typeName>
			// {
			//     .method public hidebysig newslot abstract virtual instance void Foo(int32 modreq(<modreq1>) modreq(<modreq2>) arg) cil managed { }
			// }

			typeName += "_AsModreqOnParamType";

			var typeBuilder = moduleScope.DefineType(
				true,
				typeName,
				TypeAttributes.Class | TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var methodBuilder = typeBuilder.DefineMethod(
				"Foo",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual,
				returnType: typeof(void),
				returnTypeRequiredCustomModifiers: null,
				returnTypeOptionalCustomModifiers: null,
				parameterTypes: new[]
				{
					typeof(int)
				},
				parameterTypeRequiredCustomModifiers: new[]
				{
					paramTypeModreqs
				},
				parameterTypeOptionalCustomModifiers: null,
				callingConvention: CallingConventions.Standard);

			this.generatedTypes.Add(typeName, typeBuilder.CreateType());
		}

		private void AddTypeWithCustomModifiersAsModoptOnReturnType(ModuleScope moduleScope, string typeName, params Type[] returnTypeModopts)
		{
			// This method generates a type that would look as follows in IL:
			//
			// .class interface public abstract auto ansi beforefieldinit <typeName>
			// {
			//     .method public hidebysig newslot abstract virtual instance int32 modopt(<modopt1>) modopt(<modopt2>) Foo() cil managed { }
			// }
			//
			// The C++/CLI equivalent would look like this (but note that only some modopts
			// used in this test fixture have a corresponding C++/CLI modifier):
			//
			// public interface class <typeName> {
			//     public virtual <additional_modifiers> signed int Foo();
			// };

			typeName += "_AsModoptOnReturnType";

			var typeBuilder = moduleScope.DefineType(
				true,
				typeName,
				TypeAttributes.Class | TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var methodBuilder = typeBuilder.DefineMethod(
				"Foo",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual,
				returnType: typeof(int),
				returnTypeRequiredCustomModifiers: null,
				returnTypeOptionalCustomModifiers: returnTypeModopts,
				parameterTypes: null,
				parameterTypeRequiredCustomModifiers: null,
				parameterTypeOptionalCustomModifiers: null,
				callingConvention: CallingConventions.Standard);

			this.generatedTypes.Add(typeName, typeBuilder.CreateType());
		}

		private void AddTypeWithCustomModifiersAsModreqOnReturnType(ModuleScope moduleScope, string typeName, params Type[] returnTypeModreqs)
		{
			// This method generates a type that would look as follows in IL:
			//
			// .class interface public abstract auto ansi beforefieldinit <typeName>
			// {
			//     .method public hidebysig newslot abstract virtual instance int32 modreq(<modreq1>) modreq(<modreq2>) Foo() cil managed { }
			// }

			typeName += "_AsModreqOnReturnType";

			var typeBuilder = moduleScope.DefineType(
				true,
				typeName,
				TypeAttributes.Class | TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var methodBuilder = typeBuilder.DefineMethod(
				"Foo",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual,
				returnType: typeof(int),
				returnTypeRequiredCustomModifiers: returnTypeModreqs,
				returnTypeOptionalCustomModifiers: null,
				parameterTypes: null,
				parameterTypeRequiredCustomModifiers: null,
				parameterTypeOptionalCustomModifiers: null,
				callingConvention: CallingConventions.Standard);

			this.generatedTypes.Add(typeName, typeBuilder.CreateType());
		}

		public class Foo { }
		public class Bar { }
	}
}

#endif
