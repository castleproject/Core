// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections.Specialized;
	
	using NUnit.Framework;

	[TestFixture]
	public class NameValueCollectionAdapterTestCase
	{
		[Test]
		public void NoValidEntries()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customername", "x");
			args.Add("customerage", "x");
			args.Add("customerall", "x");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			Assert.IsNull(node);
		}

		[Test]
		public void OneLevelNode()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer.name", "hammett");
			args.Add("customer.age", "26");
			args.Add("customer.all", "yada yada yada");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsFalse(node.IsIndexed);

			Assert.AreEqual("hammett", node.GetEntryValue("name"));
			Assert.AreEqual("26", node.GetEntryValue("age"));
			Assert.AreEqual("yada yada yada", node.GetEntryValue("all"));
		}

		[Test]
		public void OneLeveWithMetaTags()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer@ignore", "true");
			args.Add("customer@usetype", "mytype");
			args.Add("customer.name", "hammett");
			args.Add("customer.age", "26");
			args.Add("customer.all", "yada yada yada");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsFalse(node.IsIndexed);

			Assert.IsTrue(node.ShouldIgnore);
			Assert.AreEqual("mytype", node.GetMetaEntryValue("usetype"));

			Assert.AreEqual("hammett", node.GetEntryValue("name"));
			Assert.AreEqual("26", node.GetEntryValue("age"));
			Assert.AreEqual("yada yada yada", node.GetEntryValue("all"));
		}

		[Test]
		public void IgnoredNode()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer.name", "hammett");
			args.Add("customer.age", "26");
			args.Add("customer.all", "yada yada yada");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsFalse(node.IsIndexed);

			Assert.AreEqual("hammett", node.GetEntryValue("name"));
			Assert.AreEqual("26", node.GetEntryValue("age"));
			Assert.AreEqual("yada yada yada", node.GetEntryValue("all"));
		}

		[Test]
		public void TwoLevels()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer.name", "hammett");
			args.Add("customer.age", "26");
			args.Add("customer.location.code", "pt-br");
			args.Add("customer.location.country", "55");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsFalse(node.IsIndexed);

			IBindingDataSourceNode locationNode = node.ObtainNode("location");
			
			Assert.IsNotNull(locationNode);
			Assert.IsFalse(locationNode.IsIndexed);
			Assert.AreEqual("pt-br", locationNode.GetEntryValue("code"));
			Assert.AreEqual("55", locationNode.GetEntryValue("country"));
		}

		[Test]
		public void IndexedContent()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer[0].name", "hammett");
			args.Add("customer[0].age", "26");
			args.Add("customer[0].all", "yada yada yada");
			args.Add("customer[10].name", "frasier");
			args.Add("customer[10].age", "50");
			args.Add("customer[10].all", "yada");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsTrue(node.IsIndexed);
			Assert.AreEqual(2, node.IndexedNodes.Length);

			bool isFirst = true;
			foreach(IBindingDataSourceNode indexNode in node.IndexedNodes)
			{
				if (isFirst)
				{
					Assert.AreEqual("hammett", indexNode.GetEntryValue("name"));
					Assert.AreEqual("26", indexNode.GetEntryValue("age"));
					Assert.AreEqual("yada yada yada", indexNode.GetEntryValue("all"));
				}
				else
				{
					Assert.AreEqual("frasier", indexNode.GetEntryValue("name"));
					Assert.AreEqual("50", indexNode.GetEntryValue("age"));
					Assert.AreEqual("yada", indexNode.GetEntryValue("all"));
				}

				if (isFirst) isFirst = false;
			}
		}

		[Test]
		public void IndexedContentWithMeta()
		{
			NameValueCollection args = new NameValueCollection();

			args.Add("customer[0]@ignore", "yes");
			args.Add("customer[0].name", "hammett");
			args.Add("customer[0].age", "26");
			args.Add("customer[0].all", "yada yada yada");

			NameValueCollectionAdapter dataSource = new NameValueCollectionAdapter(args);

			IBindingDataSourceNode node = dataSource.ObtainNode("customer");
			
			Assert.IsNotNull(node);
			Assert.IsTrue(node.IsIndexed);
			Assert.AreEqual(1, node.IndexedNodes.Length);

			IBindingDataSourceNode indexNode = node.IndexedNodes[0];

			Assert.IsTrue(indexNode.ShouldIgnore);
			Assert.AreEqual("hammett", indexNode.GetEntryValue("name"));
			Assert.AreEqual("26", indexNode.GetEntryValue("age"));
			Assert.AreEqual("yada yada yada", indexNode.GetEntryValue("all"));
		}
	}
}
