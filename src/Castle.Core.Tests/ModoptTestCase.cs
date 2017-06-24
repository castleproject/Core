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
#if FEATURE_EMIT_CUSTOMMODIFIERS

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
	public class ModoptTestCase : BasePEVerifyTestCase
	{
		private static Dictionary<string, Type[]> modopts;
		private Dictionary<string, Type> generatedTypes;

		static ModoptTestCase()
		{
			// `GenerateTypes` will generate one type per dictionary entry. The key specifies
			// the type's name, and the type will have one method `Foo` with a return type of void
			// and taking a `System.Int32` parameter with the given modopts:
			ModoptTestCase.modopts = new Dictionary<string, Type[]>()
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

		public ModoptTestCase()
		{
			// This dictionary will be filled by the `GenerateTypes` one-time setup method:
			this.generatedTypes = new Dictionary<string, Type>();
		}

		public static IEnumerable<string> TypeNames => ModoptTestCase.modopts.Keys;

		/// <summary>
		/// Emits types as specified by `this.modopts` and stores the dynamic assembly to disk.
		/// The generated types are added to the `this.generatedTypes` dictionary for reference
		/// in unit tests.
		/// </summary>
		[OneTimeSetUp]
		public void GenerateTypes()
		{
			const string assemblyName = "Castle.Core.Tests.ModoptTestCaseDynamicAssembly";
			const string assemblyFileName = "Castle.Core.Tests.ModoptTestCaseDynamicAssembly.dll";

			var moduleScope = new ModuleScope(
				savePhysicalAssembly: true,
				disableSignedModule: false,
				namingScope: new Generators.NamingScope(),
				strongAssemblyName: assemblyName,
				strongModulePath: assemblyFileName,
				weakAssemblyName: assemblyName,
				weakModulePath: assemblyFileName);

			foreach (var typeName in ModoptTestCase.modopts.Keys)
			{
				var modopts = ModoptTestCase.modopts[typeName];
				this.AddType(moduleScope, typeName, typeof(int), modopts);
			}

			// Let's persist and PE-verify the dynamic assembly before it gets used in tests:
			var assemblyPath = moduleScope.SaveAssembly();
			base.RunPEVerifyOnGeneratedAssembly(assemblyPath);
		}

		/// <summary>
		/// This test checks whether reflection correctly reports back the modopts in
		/// the generated types.
		/// </summary>
		[TestCaseSource(nameof(TypeNames))]
		public void ReflectionReturnsCorrectModoptsForGeneratedType(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			var modopts = this.generatedTypes[typeName].GetMethod("Foo").GetParameters()[0].GetOptionalCustomModifiers();
			CollectionAssert.AreEqual(expected: ModoptTestCase.modopts[typeName].Reverse(), actual: modopts);
			// ^ Interesting detail: .NET appears to report modopts in reverse order.
			//   That is why we need to `.Reverse()` the original sequence.
		}

		/// <summary>
		/// This test checks whether Castle can generate proxies for types having a method
		/// whose signature contains certain combinations of modopts.
		/// </summary>
		[TestCaseSource(nameof(TypeNames))]
		public void CanGenerateProxyOfTypeWithModopts(string typeName)
		{
			Assume.That(this.generatedTypes.ContainsKey(typeName));

			var type = this.generatedTypes[typeName];
			var proxyGenerator = new ProxyGenerator();

			var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new DoNothingInterceptor());
			Assert.NotNull(proxy);
		}

		private void AddType(ModuleScope moduleScope, string typeName, Type paramType, params Type[] paramTypeModOpts)
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
					paramType
				},
				parameterTypeRequiredCustomModifiers: null,
				parameterTypeOptionalCustomModifiers: new[]
				{
					paramTypeModOpts
				},
				callingConvention: CallingConventions.Standard);

			this.generatedTypes.Add(typeName, typeBuilder.CreateType());
		}

		public class Foo { }
		public class Bar { }
	}
}

#endif
