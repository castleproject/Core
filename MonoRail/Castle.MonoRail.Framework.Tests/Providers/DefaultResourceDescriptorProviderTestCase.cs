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

namespace Castle.MonoRail.Framework.Tests.Providers
{
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultResourceDescriptorProviderTestCase
	{
		private DefaultResourceDescriptorProvider provider = new DefaultResourceDescriptorProvider();

		[Test]
		public void CanCollectResourceFromClass()
		{
			ResourceDescriptor[] descs = provider.CollectResources(typeof(ResourceOnController));

			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual("default", descs[0].Name);
			Assert.AreEqual("resName", descs[0].ResourceName);
		}

		[Test]
		public void CanCollectResourceFromMethod()
		{
			ResourceDescriptor[] descs = provider.CollectResources(typeof(ResourceOnActionController).GetMethod("Action1"));

			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual("action", descs[0].Name);
			Assert.AreEqual("name", descs[0].ResourceName);
		}

		#region Controllers

		[Resource("default", "resName")]
		public class ResourceOnController : Controller
		{
		}

		public class ResourceOnActionController : Controller
		{
			[Resource("action", "name")]
			public void Action1()
			{
			}
		}

		#endregion
	}
}
