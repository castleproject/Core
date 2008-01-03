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
	using System.IO;
	using Castle.MonoRail.Framework.Providers;
	using Descriptors;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultTransformFilterDescriptorProviderTestCase
	{
		private DefaultTransformFilterDescriptorProvider provider = new DefaultTransformFilterDescriptorProvider();

		[Test]
		public void CanCollectTransformationFiltersFromMethod()
		{
			TransformFilterDescriptor[] descs = provider.CollectFilters(typeof(TransformOnActionController).GetMethod("Action1"));

			Assert.IsNotNull(descs);
			Assert.AreEqual(1, descs.Length);
			Assert.AreEqual(typeof(DummyTransFilter), descs[0].TransformFilterType);
		}

		#region Controllers

		public class TransformOnActionController : Controller
		{
			[TransformFilter(typeof(DummyTransFilter))]
			public void Action1()
			{
			}
		}

		public class DummyTransFilter : TransformFilter
		{
			public DummyTransFilter(Stream baseStream) : base(baseStream)
			{
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new System.NotImplementedException();
			}
		}

		#endregion
	}
}
