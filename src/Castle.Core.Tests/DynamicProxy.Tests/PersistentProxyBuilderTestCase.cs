// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

#if NET462_OR_GREATER

	[TestFixture]
	public class PersistentProxyBuilderTestCase
	{
		[Test]
		public void PersistentProxyBuilder_NullIfNothingSaved()
		{
			PersistentProxyBuilder builder = new PersistentProxyBuilder();
			string path = builder.SaveAssembly();
			Assert.IsNull(path);
		}

		[Test]
		[Platform(Exclude = "Mono", Reason = "On Mono, `ModuleBuilder.FullyQualifiedName` does not return a fully qualified name including a path. See https://github.com/mono/mono/issues/8503.")]
		public void PersistentProxyBuilder_SavesSignedFile()
		{
			PersistentProxyBuilder builder = new PersistentProxyBuilder();
			builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, ProxyGenerationOptions.Default);
			string path = builder.SaveAssembly();
			Assert.IsNotNull(path);
			Assert.IsNotEmpty(path);
			Assert.IsTrue(Path.IsPathRooted(path));
			Assert.IsTrue(path.EndsWith(ModuleScope.DEFAULT_FILE_NAME));
		}
	}

#elif NET9_0_OR_GREATER

	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	public class PersistentProxyBuilderTestCase
	{
		private List<Assembly> assemblies;
		private PersistentProxyBuilder builder;

		[SetUp]
		public void SetupProxyBuilder()
		{
			assemblies = new List<Assembly>();
			builder = new PersistentProxyBuilder();
			builder.AssemblyCreated += (object _, PersistentProxyBuilderAssemblyEventArgs e) =>
			{
				assemblies.Add(e.Assembly);
			};
		}

		[Test]
		public void SavesOneAssemblyPerProxiedType()
		{
			var oneProxyType = builder.CreateInterfaceProxyTypeWithoutTarget(typeof(IOne), Type.EmptyTypes, ProxyGenerationOptions.Default);
			Assert.AreEqual(1, assemblies.Count);

			var twoProxyType = builder.CreateInterfaceProxyTypeWithoutTarget(typeof(ITwo), Type.EmptyTypes, ProxyGenerationOptions.Default);
			Assert.AreEqual(2, assemblies.Count);

			var oneAssembly = assemblies[0];
			var twoAssembly = assemblies[1];
			Assert.AreSame(oneAssembly, oneProxyType.Assembly);
			Assert.AreSame(twoAssembly, twoProxyType.Assembly);
			Assert.AreNotSame(oneAssembly, twoAssembly);
		}

		[Test]
		public void TypeCacheWorks()
		{
			var proxyType1 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, ProxyGenerationOptions.Default);
			var proxyType2 = builder.CreateClassProxyType(typeof(object), Type.EmptyTypes, ProxyGenerationOptions.Default);

			Assert.AreEqual(1, assemblies.Count);
			Assert.AreSame(proxyType1, proxyType2);
			Assert.AreSame(proxyType1.Assembly, proxyType2.Assembly);
		}
	}

#endif

}
