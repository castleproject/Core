// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Tests.Lifecycle
{
	using System;

	using NUnit.Framework;

	using Castle.MicroKernel.Tests.Lifecycle.Components;

	/// <summary>
	/// Summary description for LifecycleTestCase.
	/// </summary>
	[TestFixture]
	public class LifecycleTestCase 
	{
		private IKernel kernel;

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();
		}

		[TearDown]
		public void Dispose()
		{
			kernel.Dispose();
		}

		[Test]
		public void InitializeLifecycle()
		{
			kernel.AddComponent( "a", typeof(HttpFakeServer) );
			HttpFakeServer server = (HttpFakeServer) kernel["a"];

			Assert.IsTrue( server.IsInitialized );
		}

		[Test]
		public void DisposableLifecycle()
		{
			kernel.AddComponent( "a", typeof(HttpFakeServer) );
			IHandler handler = kernel.GetHandler("a");
			HttpFakeServer server = (HttpFakeServer) handler.Resolve(CreationContext.Empty);

			handler.Release( server );

			Assert.IsTrue( server.IsDisposed );
		}
	}
}
