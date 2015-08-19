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

namespace Castle.Components.DictionaryAdapter.Tests
{
#if !SILVERLIGHT
	using System.Collections.Specialized;

	using NUnit.Framework;

	[TestFixture]
	public class AdaptingNameValueCollectionsTestCase
	{
		private NameValueCollection nameValueCollection;
		private DictionaryAdapterFactory factory;

#if FEATURE_XUNITNET
		public AdaptingNameValueCollectionsTestCase()
#else
		[SetUp]
		public void SetUp()
#endif
		{
			nameValueCollection = new NameValueCollection();
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void Factory_ForNameValueCollection_CreatesTheAdapter()
		{
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.IsNotNull(furniture);
		}

		[Test]
		public void Adapter_OnNameValueCollection_CanGetProperties()
		{
			var typeName = "Chair";
			nameValueCollection["TypeName"] = typeName;
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.AreEqual(typeName, furniture.TypeName);
		}

		[Test]
		public void Adapter_OnNameValueCollection_CanSetProperties()
		{
			var typeName = "Chair";
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			furniture.TypeName = typeName;
			Assert.AreEqual(typeName, nameValueCollection["TypeName"]);
		}

		[Test]
		public void Adapter_OnNameValueCollectionWithPropertyBinder_CanGetProperties()
		{
			int legs = 2;
			nameValueCollection["Legs"] = legs.ToString();
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.AreEqual(legs, furniture.Legs);
		}

		[Test]
		public void Adapter_OnNameValueCollectionWithPropertyBinder_CanSetProperties()
		{
			int legs = 2;
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			furniture.Legs = legs;
			Assert.AreEqual(legs.ToString(), nameValueCollection["Legs"]);
		}

		public interface IFurniture
		{
			string TypeName { get; set; }

			int? Legs { get; set; }
		}
	}
#endif
}
