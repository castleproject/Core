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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;

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
		[ExcludeOnFramework(Framework.Mono, "On Mono, `ModuleBuilder.FullyQualifiedName` does not return a fully qualified name including a path. See https://github.com/mono/mono/issues/8503.")]
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
}

#endif