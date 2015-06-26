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
#if !SILVERLIGHT && !NETCORE
	using System.Collections.Specialized;

	using Xunit;

	public class AdaptingNameValueCollectionsTestCase
	{
		private NameValueCollection nameValueCollection;
		private DictionaryAdapterFactory factory;

		public AdaptingNameValueCollectionsTestCase()
		{
			nameValueCollection = new NameValueCollection();
			factory = new DictionaryAdapterFactory();
		}

		[Fact]
		public void Factory_ForNameValueCollection_CreatesTheAdapter()
		{
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.NotNull(furniture);
		}

		[Fact]
		public void Adapter_OnNameValueCollection_CanGetProperties()
		{
			var typeName = "Chair";
			nameValueCollection["TypeName"] = typeName;
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.Equal(typeName, furniture.TypeName);
		}

		[Fact]
		public void Adapter_OnNameValueCollection_CanSetProperties()
		{
			var typeName = "Chair";
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			furniture.TypeName = typeName;
			Assert.Equal(typeName, nameValueCollection["TypeName"]);
		}

		[Fact]
		public void Adapter_OnNameValueCollectionWithPropertyBinder_CanGetProperties()
		{
			int legs = 2;
			nameValueCollection["Legs"] = legs.ToString();
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			Assert.Equal(legs, furniture.Legs);
		}

		[Fact]
		public void Adapter_OnNameValueCollectionWithPropertyBinder_CanSetProperties()
		{
			int legs = 2;
			var furniture = factory.GetAdapter<IFurniture>(nameValueCollection);
			furniture.Legs = legs;
			Assert.Equal(legs.ToString(), nameValueCollection["Legs"]);
		}

		public interface IFurniture
		{
			string TypeName { get; set; }

			int? Legs { get; set; }
		}
	}
#endif
}