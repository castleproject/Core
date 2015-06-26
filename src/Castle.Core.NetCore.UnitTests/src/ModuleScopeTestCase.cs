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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Serialization;
	using Castle.DynamicProxy.Tests.InterClasses;

	using Xunit;

	public class ModuleScopeTestCase
	{
		[Fact]
		public void ModuleScopeStoresModuleBuilder()
		{
			var scope = new ModuleScope();
			var one = scope.ObtainDynamicModuleWithStrongName();
			var two = scope.ObtainDynamicModuleWithStrongName();

			Assert.Same(one, two);
			Assert.Same(one.Assembly, two.Assembly);
		}

		[Fact]
		public void ModuleScopeCanHandleSignedAndUnsignedInParallel()
		{
			var scope = new ModuleScope();
			Assert.Null(scope.StrongNamedModule);
			Assert.Null(scope.WeakNamedModule);

			var one = scope.ObtainDynamicModuleWithStrongName();
			Assert.NotNull(scope.StrongNamedModule);
			Assert.Null(scope.WeakNamedModule);
			Assert.Same(one, scope.StrongNamedModule);

			var two = scope.ObtainDynamicModuleWithWeakName();
			Assert.NotNull(scope.StrongNamedModule);
			Assert.NotNull(scope.WeakNamedModule);
			Assert.Same(two, scope.WeakNamedModule);

			Assert.NotSame(one, two);
			Assert.NotSame(one.Assembly, two.Assembly);

			var three = scope.ObtainDynamicModuleWithStrongName();
			var four = scope.ObtainDynamicModuleWithWeakName();

			Assert.Same(one, three);
			Assert.Same(two, four);
		}

#if !MONO && !SILVERLIGHT && !NETCORE&& !NETCORE

		[Fact]
		public void ImplicitModulePaths()
		{
			var scope = new ModuleScope(true);
			Assert.Equal(ModuleScope.DEFAULT_FILE_NAME, scope.StrongNamedModuleName);
			Assert.Equal(Path.Combine(Environment.CurrentDirectory, ModuleScope.DEFAULT_FILE_NAME),
				scope.ObtainDynamicModuleWithStrongName().FullyQualifiedName);
			Assert.Null(scope.StrongNamedModuleDirectory);

			Assert.Equal(ModuleScope.DEFAULT_FILE_NAME, scope.WeakNamedModuleName);
			Assert.Equal(Path.Combine(Environment.CurrentDirectory, ModuleScope.DEFAULT_FILE_NAME),
				scope.ObtainDynamicModuleWithWeakName().FullyQualifiedName);
			Assert.Null(scope.WeakNamedModuleDirectory);
		}

		[Fact]
		public void ExplicitModulePaths()
		{
			var scope = new ModuleScope(true, false, "Strong", "StrongModule.dll", "Weak", "WeakModule.dll");
			Assert.Equal("StrongModule.dll", scope.StrongNamedModuleName);
			Assert.Equal(Path.Combine(Environment.CurrentDirectory, "StrongModule.dll"),
				scope.ObtainDynamicModuleWithStrongName().FullyQualifiedName);
			Assert.Null(scope.StrongNamedModuleDirectory);

			Assert.Equal("WeakModule.dll", scope.WeakNamedModuleName);
			Assert.Equal(Path.Combine(Environment.CurrentDirectory, "WeakModule.dll"),
				scope.ObtainDynamicModuleWithWeakName().FullyQualifiedName);
			Assert.Null(scope.WeakNamedModuleDirectory);

			scope = new ModuleScope(true, false, "Strong", @"c:\Foo\StrongModule.dll", "Weak", @"d:\Bar\WeakModule.dll");
			Assert.Equal("StrongModule.dll", scope.StrongNamedModuleName);
			Assert.Equal(@"c:\Foo\StrongModule.dll", scope.ObtainDynamicModuleWithStrongName().FullyQualifiedName);
			Assert.Equal(@"c:\Foo", scope.StrongNamedModuleDirectory);

			Assert.Equal("WeakModule.dll", scope.WeakNamedModuleName);
			Assert.Equal(@"d:\Bar\WeakModule.dll", scope.ObtainDynamicModuleWithWeakName().FullyQualifiedName);
			Assert.Equal(@"d:\Bar", scope.WeakNamedModuleDirectory);
		}

#endif

#if !SILVERLIGHT && !NETCORE
		private static void CheckSignedSavedAssembly(string path)
		{
			Assert.True(File.Exists(path));

			var assemblyName = AssemblyName.GetAssemblyName(path);
			Assert.Equal(ModuleScope.DEFAULT_ASSEMBLY_NAME, assemblyName.Name);

			var keyPairBytes = ModuleScope.GetKeyPair();
			var keyPair = new StrongNameKeyPair(keyPairBytes);
			var loadedPublicKey = assemblyName.GetPublicKey();

			Assert.Equal(keyPair.PublicKey.Length, loadedPublicKey.Length);
			for (var i = 0; i < keyPair.PublicKey.Length; ++i)
			{
				Assert.Equal(keyPair.PublicKey[i], loadedPublicKey[i]);
			}
		}

		[Fact]
		public void SaveSigned()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithStrongName();

			var path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			Assert.False(File.Exists(path));
			var savedPath = scope.SaveAssembly();

			Assert.Equal(savedPath, Path.GetFullPath(path));

			CheckSignedSavedAssembly(path);
			File.Delete(path);
		}

		[Fact]
		public void SaveUnsigned()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithWeakName();

			var path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
			{
				File.Delete(path);
			}

			Assert.False(File.Exists(path));
			var savedPath = scope.SaveAssembly();

			Assert.Equal(savedPath, Path.GetFullPath(path));

			CheckUnsignedSavedAssembly(path);
			File.Delete(path);
		}

		[Fact]
		public void SaveWithPath()
		{
			var strongModulePath = Path.GetTempFileName();
			var weakModulePath = Path.GetTempFileName();

			File.Delete(strongModulePath);
			File.Delete(weakModulePath);

			Assert.False(File.Exists(strongModulePath));
			Assert.False(File.Exists(weakModulePath));

			var scope = new ModuleScope(true, false, "Strong", strongModulePath, "Weak", weakModulePath);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			scope.SaveAssembly(true);
			scope.SaveAssembly(false);

			Assert.True(File.Exists(strongModulePath));
			Assert.True(File.Exists(weakModulePath));

			File.Delete(strongModulePath);
			File.Delete(weakModulePath);
		}

		private static void CheckUnsignedSavedAssembly(string path)
		{
			Assert.True(File.Exists(path));

			var assemblyName = AssemblyName.GetAssemblyName(path);
			Assert.Equal(ModuleScope.DEFAULT_ASSEMBLY_NAME, assemblyName.Name);

			var loadedPublicKey = assemblyName.GetPublicKey();
			Assert.Null(loadedPublicKey);
		}

		[Fact]
		public void SaveReturnsNullWhenNoModuleObtained()
		{
			var scope = new ModuleScope(true);
			Assert.Null(scope.SaveAssembly());
		}

		[Fact]
		public void SaveThrowsWhenMultipleAssembliesGenerated()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var scope = new ModuleScope(true);
				scope.ObtainDynamicModuleWithStrongName();
				scope.ObtainDynamicModuleWithWeakName();

				scope.SaveAssembly();
			});
		}

		[Fact]
		public void SaveWithFlagFalseDoesntThrowsWhenMultipleAssembliesGenerated()
		{
			var scope = new ModuleScope(false);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			scope.SaveAssembly();
		}

		[Fact]
		public void ExplicitSaveWorksEvenWhenMultipleAssembliesGenerated()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithStrongName();
			scope.ObtainDynamicModuleWithWeakName();

			scope.SaveAssembly(true);
			CheckSignedSavedAssembly(ModuleScope.DEFAULT_FILE_NAME);

			scope.SaveAssembly(false);
			CheckUnsignedSavedAssembly(ModuleScope.DEFAULT_FILE_NAME);

			File.Delete(ModuleScope.DEFAULT_FILE_NAME);
		}

		[Fact]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedWeakName()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var scope = new ModuleScope(true);
				scope.ObtainDynamicModuleWithStrongName();

				scope.SaveAssembly(false);
			});
		}

		[Fact]
		public void ExplicitSaveThrowsWhenSpecifiedAssemblyNotGeneratedStrongName()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var scope = new ModuleScope(true);
				scope.ObtainDynamicModuleWithWeakName();

				scope.SaveAssembly(true);
			});
		}

		[Fact]
		public void SavedAssemblyHasCacheMappings()
		{
			var scope = new ModuleScope(true);
			scope.ObtainDynamicModuleWithWeakName();

			var savedPath = scope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var assembly = Assembly.LoadFrom((string)args[0]);
				Assert.True(assembly.IsDefined(typeof(CacheMappingsAttribute), false));
			},
				savedPath);

			File.Delete(savedPath);
		}

		[Fact]
		public void CacheMappingsHoldTypes()
		{
			var scope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(scope);
			var cp = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, ProxyGenerationOptions.Default);

			var savedPath = scope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var assembly = Assembly.LoadFrom((string)args[0]);
				var attribute =
					(CacheMappingsAttribute)
						assembly.GetCustomAttributes(typeof(CacheMappingsAttribute), false)[0];
				var entries = attribute.GetDeserializedMappings();
				Assert.Equal(1, entries.Count);

				var key = new CacheKey(typeof(object), new Type[0],
					ProxyGenerationOptions.Default);
				Assert.True(entries.ContainsKey(key));
				Assert.Equal(args[1], entries[key]);
			},
				savedPath, cp.FullName);

			File.Delete(savedPath);
		}

		[Fact]
		public void GeneratedAssembliesDefaultName()
		{
			var scope = new ModuleScope();
			var strong = scope.ObtainDynamicModuleWithStrongName();
			var weak = scope.ObtainDynamicModuleWithWeakName();

			Assert.Equal(ModuleScope.DEFAULT_ASSEMBLY_NAME, strong.Assembly.GetName().Name);
			Assert.Equal(ModuleScope.DEFAULT_ASSEMBLY_NAME, weak.Assembly.GetName().Name);
		}

		[Fact]
		public void GeneratedAssembliesWithCustomName()
		{
			var scope = new ModuleScope(false, false, "Strong", "Module1.dll", "Weak", "Module2,dll");
			var strong = scope.ObtainDynamicModuleWithStrongName();
			var weak = scope.ObtainDynamicModuleWithWeakName();

			Assert.Equal("Strong", strong.Assembly.GetName().Name);
			Assert.Equal("Weak", weak.Assembly.GetName().Name);
		}

		[Fact]
		public void ModuleScopeDoesntTryToDeleteFromCurrentDirectory()
		{
			var moduleDirectory = Path.Combine(Environment.CurrentDirectory, "GeneratedDlls");
			if (Directory.Exists(moduleDirectory))
			{
				Directory.Delete(moduleDirectory, true);
			}

			var strongModulePath = Path.Combine(moduleDirectory, "Strong.dll");
			var weakModulePath = Path.Combine(moduleDirectory, "Weak.dll");

			Directory.CreateDirectory(moduleDirectory);
			var scope = new ModuleScope(true, false, "Strong", strongModulePath, "Weak", weakModulePath);

			using (File.Create(Path.Combine(Environment.CurrentDirectory, "Strong.dll")))
			{
				scope.ObtainDynamicModuleWithStrongName();
				scope.SaveAssembly(true); // this will throw if SaveAssembly tries to delete from the current directory
			}

			using (File.Create(Path.Combine(Environment.CurrentDirectory, "Weak.dll")))
			{
				scope.ObtainDynamicModuleWithWeakName();
				scope.SaveAssembly(false); // this will throw if SaveAssembly tries to delete from the current directory
			}

			// Clean up the generated DLLs because the FileStreams are now closed
			Directory.Delete(moduleDirectory, true);
			File.Delete(Path.Combine(Environment.CurrentDirectory, "Strong.dll"));
			File.Delete(Path.Combine(Environment.CurrentDirectory, "Weak.dll"));
		}
#endif

		[Fact]
		public void DefaultProxyBuilderWithSpecificScope()
		{
			var scope = new ModuleScope(false);
			var builder = new DefaultProxyBuilder(scope);
			Assert.Same(scope, builder.ModuleScope);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void LoadAssemblyIntoCache_InvalidAssembly()
		{
			var newScope = new ModuleScope(false);
			Assert.Throws<ArgumentException>(() => newScope.LoadAssemblyIntoCache(Assembly.GetExecutingAssembly()));
		}

		[Fact]
		public void LoadAssemblyIntoCache_CreateClassProxy()
		{
			CheckLoadAssemblyIntoCache(
				builder => builder.CreateClassProxyType(typeof(object), null, ProxyGenerationOptions.Default));
		}

		[Fact]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithoutTarget()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithoutTarget(typeof(IServiceProvider), new Type[0],
						ProxyGenerationOptions.Default);
				});
		}

		[Fact]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithTarget()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithTarget(typeof(IMyInterface2), new Type[0], typeof(MyInterfaceImpl),
						ProxyGenerationOptions.Default);
				});
		}

		[Fact]
		public void LoadAssemblyIntoCache_CreateInterfaceProxyTypeWithTargetInterface()
		{
			CheckLoadAssemblyIntoCache(
				delegate(IProxyBuilder builder)
				{
					return builder.CreateInterfaceProxyTypeWithTargetInterface(typeof(IMyInterface2), null,
						ProxyGenerationOptions.Default);
				});
		}

		[Fact]
		public void LoadAssemblyIntoCache_DifferentGenerationOptions()
		{
			var savedScope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(savedScope);

			var options1 = new ProxyGenerationOptions();
			options1.AddMixinInstance(new DateTime());
			var options2 = ProxyGenerationOptions.Default;

			var cp1 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options1);
			var cp2 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options2);
			Assert.NotSame(cp1, cp2);
			Assert.Same(cp1, builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options1));
			Assert.Same(cp2, builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, options2));

			var path = savedScope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var newScope = new ModuleScope(false);
				var newBuilder = new DefaultProxyBuilder(newScope);

				var assembly = Assembly.LoadFrom((string)args[0]);
				newScope.LoadAssemblyIntoCache(assembly);

				var newOptions1 = new ProxyGenerationOptions();
				newOptions1.AddMixinInstance(new DateTime());
				var newOptions2 = ProxyGenerationOptions.Default;

				var loadedCP1 = newBuilder.CreateClassProxyType(typeof(object),
					Type.EmptyTypes,
					newOptions1);
				var loadedCP2 = newBuilder.CreateClassProxyType(typeof(object),
					Type.EmptyTypes,
					newOptions2);
				Assert.NotSame(loadedCP1, loadedCP2);
				Assert.Equal(assembly, loadedCP1.Assembly);
				Assert.Equal(assembly, loadedCP2.Assembly);
			}, path);

			File.Delete(path);
		}

		private delegate Type ProxyCreator(IProxyBuilder proxyBuilder);

		private void CheckLoadAssemblyIntoCache(ProxyCreator creator)
		{
			var savedScope = new ModuleScope(true);
			var builder = new DefaultProxyBuilder(savedScope);

			var cp = creator(builder);
			Assert.Same(cp, creator(builder));

			var path = savedScope.SaveAssembly();

			CrossAppDomainCaller.RunInOtherAppDomain(delegate(object[] args)
			{
				var newScope = new ModuleScope(false);
				var newBuilder = new DefaultProxyBuilder(newScope);

				var assembly = Assembly.LoadFrom((string)args[0]);
				newScope.LoadAssemblyIntoCache(assembly);

				var loadedCP = assembly.GetType((string)args[1]);
				Assert.Same(loadedCP, ((ProxyCreator)args[2])(newBuilder));
				Assert.Equal(assembly, ((ProxyCreator)args[2])(newBuilder).Assembly);
			}, path, cp.FullName, creator);

			File.Delete(path);
		}

#endif
	}
}