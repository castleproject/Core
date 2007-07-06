// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;
	using System.Reflection.Emit;
	using System.IO;
	using System.Reflection;

	[TestFixture]
	public class ModuleScopeTestCase
	{
		[Test]
		public void ModuleScopeStoresModuleBuilder ()
		{
			ModuleScope scope = new ModuleScope ();
			ModuleBuilder one = scope.ObtainDynamicModuleWithStrongName ();
			ModuleBuilder two = scope.ObtainDynamicModuleWithStrongName ();

			Assert.AreSame (one, two);
			Assert.AreSame (one.Assembly, two.Assembly);
		}

		[Test]
		public void ModuleScopeCanHandleSignedAndUnsignedInParallel ()
		{
			ModuleScope scope = new ModuleScope ();
			Assert.IsNull (scope.StrongNamedModule);
			Assert.IsNull (scope.WeakNamedModule);

			ModuleBuilder one = scope.ObtainDynamicModuleWithStrongName ();
			Assert.IsNotNull (scope.StrongNamedModule);
			Assert.IsNull (scope.WeakNamedModule);
			Assert.AreSame (one, scope.StrongNamedModule);

			ModuleBuilder two = scope.ObtainDynamicModuleWithWeakName ();
			Assert.IsNotNull (scope.StrongNamedModule);
			Assert.IsNotNull (scope.WeakNamedModule);
			Assert.AreSame (two, scope.WeakNamedModule);

			Assert.AreNotSame (one, two);
			Assert.AreNotSame (one.Assembly, two.Assembly);

			ModuleBuilder three = scope.ObtainDynamicModuleWithStrongName ();
			ModuleBuilder four = scope.ObtainDynamicModuleWithWeakName ();

			Assert.AreSame (one, three);
			Assert.AreSame (two, four);
		}

		[Test]
		public void SaveSigned ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithStrongName ();
			
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists (path))
				File.Delete (path);

			Assert.IsFalse (File.Exists (path));
			scope.SaveAssembly ();

			CheckSignedSavedAssembly(path);
			File.Delete (path);
		}

		private static void CheckSignedSavedAssembly (string path)
		{
			Assert.IsTrue (File.Exists (path));

			AssemblyName assemblyName = AssemblyName.GetAssemblyName (path);
			Assert.AreEqual (ModuleScope.DEFAULT_ASSEMBLY_NAME, assemblyName.Name);

			byte[] keyPairBytes = ModuleScope.GetKeyPair ();
			StrongNameKeyPair keyPair = new StrongNameKeyPair (keyPairBytes);
			byte[] loadedPublicKey = assemblyName.GetPublicKey();

			Assert.AreEqual (keyPair.PublicKey.Length, loadedPublicKey.Length);
			for (int i = 0; i < keyPair.PublicKey.Length; ++i)
				Assert.AreEqual (keyPair.PublicKey[i], loadedPublicKey[i]);
		}

		[Test]
		public void SaveUnsigned ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithWeakName ();

			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists (path))
				File.Delete (path);

			Assert.IsFalse (File.Exists (path));
			scope.SaveAssembly ();

			CheckUnsignedSavedAssembly(path);
			File.Delete (path);
		}

		private static void CheckUnsignedSavedAssembly (string path)
		{
			Assert.IsTrue (File.Exists (path));

			AssemblyName assemblyName = AssemblyName.GetAssemblyName (path);
			Assert.AreEqual (ModuleScope.DEFAULT_ASSEMBLY_NAME, assemblyName.Name);

			byte[] loadedPublicKey = assemblyName.GetPublicKey ();
			Assert.IsNull (loadedPublicKey);
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void SaveThrowsWhenNoModuleObtained ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.SaveAssembly ();
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void SaveThrowsWhenMultipleAssembliesGenerated ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithStrongName ();
			scope.ObtainDynamicModuleWithWeakName ();

			scope.SaveAssembly ();
		}

		[Test]
		public void ExplicitSaveWorksEvenWhenMultipleAssembliesGenerated ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithStrongName ();
			scope.ObtainDynamicModuleWithWeakName ();

			scope.SaveAssembly (true, "StrongNamedAssembly.dll");
			scope.SaveAssembly (false, "WeakNamedAssembly.dll");
			CheckSignedSavedAssembly ("StrongNamedAssembly.dll");
			CheckUnsignedSavedAssembly ("WeakNamedAssembly.dll");

			File.Delete ("StrongNamedAssembly.dll");
			File.Delete ("WeakNamedAssembly.dll");
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedWeakName ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithStrongName ();

			scope.SaveAssembly (false, "StrongNamedAssembly.dll");
		}

		[Test]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedStrongName ()
		{
			ModuleScope scope = new ModuleScope (true);
			scope.ObtainDynamicModuleWithWeakName ();

			scope.SaveAssembly (false, "WeakNamedAssembly.dll");
		}

		[Test]
		public void GeneratedAssembliesDefaultName ()
		{
			ModuleScope scope = new ModuleScope ();
			ModuleBuilder strong = scope.ObtainDynamicModuleWithStrongName ();
			ModuleBuilder weak = scope.ObtainDynamicModuleWithWeakName ();

			Assert.AreEqual (ModuleScope.DEFAULT_ASSEMBLY_NAME, strong.Assembly.GetName ().Name);
			Assert.AreEqual (ModuleScope.DEFAULT_ASSEMBLY_NAME, weak.Assembly.GetName ().Name);
		}

		[Test]
		public void GeneratedAssembliesWithCustomName ()
		{
			ModuleScope scope = new ModuleScope (false, "Strong", "Weak");
			ModuleBuilder strong = scope.ObtainDynamicModuleWithStrongName ();
			ModuleBuilder weak = scope.ObtainDynamicModuleWithWeakName ();

			Assert.AreEqual ("Strong", strong.Assembly.GetName ().Name);
			Assert.AreEqual ("Weak", weak.Assembly.GetName ().Name);
		}
	}
}