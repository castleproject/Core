// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
// 
namespace Castle.Components.Binder.Tests
{
	using System.Collections.Specialized;
	using NUnit.Framework;

	[TestFixture]
	public class TreeBuilderTestCase
	{
		[Test]
		public void CompositeEntries()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("customer.name", "hammett");
			nameValueColl.Add("customer.age", "27");
			nameValueColl.Add("customer.age", "28");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);

			var node = (CompositeNode) root.GetChildNode("customer");
			Assert.IsNotNull(node);
			Assert.AreEqual("customer", node.Name);
			Assert.AreEqual(NodeType.Composite, node.NodeType);

			var entry = (LeafNode) node.GetChildNode("name");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual("name", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("hammett", entry.Value);

			entry = (LeafNode) node.GetChildNode("age");
			Assert.IsNotNull(entry);
			Assert.IsTrue(entry.IsArray);
			Assert.AreEqual("age", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual(new[] {"27", "28"}, entry.Value);
		}

		[Test]
		public void EntriesStartingWithDotShouldBeConsideredSimple()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add(".name", "hammett");
			nameValueColl.Add(".age", "27");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.IsNotNull(root);
			Assert.AreEqual(2, root.ChildrenCount);

			var entry = (LeafNode) root.GetChildNode(".name");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual(".name", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("hammett", entry.Value);

			entry = (LeafNode) root.GetChildNode(".age");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual(".age", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("27", entry.Value);
		}

		[Test]
		public void IndexedEntries()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("customer[0].name", "hammett");
			nameValueColl.Add("customer[0].age", "27");
			nameValueColl.Add("customer[0].age", "28");
			nameValueColl.Add("customer[1].name", "hamilton");
			nameValueColl.Add("customer[1].age", "28");
			nameValueColl.Add("customer[1].age", "29");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);

			var indexNode = (IndexedNode) root.GetChildNode("customer");
			Assert.IsNotNull(indexNode);
			Assert.AreEqual(2, indexNode.ChildrenCount);

			var node = (CompositeNode) indexNode.GetChildNode("0");
			Assert.IsNotNull(node);
			Assert.AreEqual(2, node.ChildrenCount);

			var entry = (LeafNode) node.GetChildNode("name");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual("name", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("hammett", entry.Value);

			node = (CompositeNode) indexNode.GetChildNode("1");
			Assert.IsNotNull(node);
			Assert.AreEqual(2, node.ChildrenCount);

			entry = (LeafNode) node.GetChildNode("name");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual("name", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("hamilton", entry.Value);
		}

		[Test]
		public void IndexedFlatEntries()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("emails[0]", "hammett@gmail.com");
			nameValueColl.Add("emails[1]", "hammett@apache.org");
			nameValueColl.Add("emails[2]", "hammett@uol.com.br");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);

			var indexNode = (IndexedNode) root.GetChildNode("emails");
			Assert.IsNotNull(indexNode);
			Assert.AreEqual(3, indexNode.ChildrenCount);

			Node[] entries = indexNode.ChildNodes;
			Assert.IsNotNull(entries);
			Assert.AreEqual(3, entries.Length);

			Assert.AreEqual("hammett@gmail.com", ((LeafNode) entries[0]).Value);
			Assert.AreEqual("hammett@apache.org", ((LeafNode) entries[1]).Value);
			Assert.AreEqual("hammett@uol.com.br", ((LeafNode) entries[2]).Value);
		}

		[Test,
		 ExpectedException(typeof (BindingException),
		 	ExpectedMessage =
		 		"Attempt to create or obtain a composite node named Process, but a node with the same exists with the type Leaf")]
		public void RepeatedNamesForNodes()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("profile.Process", "test");
			nameValueColl.Add("profile.Process.Id", "1");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
		}

		[Test]
		public void SimpleEntries()
		{
			var nameValueColl = new NameValueCollection();

			nameValueColl.Add("name", "hammett");
			nameValueColl.Add("age", "27");
			nameValueColl.Add("age", "28");

			var builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);

			Assert.IsNotNull(root);
			Assert.AreEqual(2, root.ChildrenCount);

			var entry = (LeafNode) root.GetChildNode("name");
			Assert.IsNotNull(entry);
			Assert.IsFalse(entry.IsArray);
			Assert.AreEqual("name", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual("hammett", entry.Value);

			entry = (LeafNode) root.GetChildNode("age");
			Assert.IsNotNull(entry);
			Assert.IsTrue(entry.IsArray);
			Assert.AreEqual("age", entry.Name);
			Assert.AreEqual(NodeType.Leaf, entry.NodeType);
			Assert.AreEqual(new[] {"27", "28"}, entry.Value);
		}
	}
}