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

namespace Castle.Components.Binder.Tests
{
	using System;
	using System.Collections.Specialized;
	
	using NUnit.Framework;

	[TestFixture]
	public class TreeBuilderTestCase
	{
		[Test]
		public void SimpleEntries()
		{
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add("name", "hammett");
			nameValueColl.Add("age", "27");
			nameValueColl.Add("age", "28");
			
			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
			
			Assert.IsNotNull(root);
			Assert.AreEqual(2, root.ChildrenCount);
			
			LeafNode entry = (LeafNode) root.GetChildNode("name");
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
			Assert.AreEqual(new String[] { "27", "28" }, entry.Value);
		}

		[Test]
		public void EntriesStartingWithDotShouldBeConsideredSimple()
		{
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add(".name", "hammett");
			nameValueColl.Add(".age", "27");
			
			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
			
			Assert.IsNotNull(root);
			Assert.AreEqual(2, root.ChildrenCount);
			
			LeafNode entry = (LeafNode) root.GetChildNode(".name");
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
		public void CompositeEntries()
		{
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add("customer.name", "hammett");
			nameValueColl.Add("customer.age", "27");
			nameValueColl.Add("customer.age", "28");
			
			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
			
			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);
			
			CompositeNode node = (CompositeNode) root.GetChildNode("customer");
			Assert.IsNotNull(node);
			Assert.AreEqual("customer", node.Name);
			Assert.AreEqual(NodeType.Composite, node.NodeType);
			
			LeafNode entry = (LeafNode) node.GetChildNode("name");
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
			Assert.AreEqual(new String[] { "27", "28" }, entry.Value);
		}
		
		[Test]
		public void IndexedEntries()
		{
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add("customer[0].name", "hammett");
			nameValueColl.Add("customer[0].age", "27");
			nameValueColl.Add("customer[0].age", "28");
			nameValueColl.Add("customer[1].name", "hamilton");
			nameValueColl.Add("customer[1].age", "28");
			nameValueColl.Add("customer[1].age", "29");
			
			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
			
			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);
			
			IndexedNode indexNode = (IndexedNode) root.GetChildNode("customer");
			Assert.IsNotNull(indexNode);
			Assert.AreEqual(2, indexNode.ChildrenCount);
			
			CompositeNode node = (CompositeNode) indexNode.GetChildNode("0");
			Assert.IsNotNull(node);
			Assert.AreEqual(2, node.ChildrenCount);
			
			LeafNode entry = (LeafNode) node.GetChildNode("name");
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
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add("emails[0]", "hammett@gmail.com");
			nameValueColl.Add("emails[1]", "hammett@apache.org");
			nameValueColl.Add("emails[2]", "hammett@uol.com.br");
			
			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
			
			Assert.IsNotNull(root);
			Assert.AreEqual(1, root.ChildrenCount);
			
			IndexedNode indexNode = (IndexedNode) root.GetChildNode("emails");
			Assert.IsNotNull(indexNode);
			Assert.AreEqual(3, indexNode.ChildrenCount);
			
			Node[] entries = indexNode.ChildNodes;
			Assert.IsNotNull(entries);
			Assert.AreEqual(3, entries.Length);
			
			Assert.AreEqual("hammett@gmail.com", ((LeafNode)entries[0]).Value);
			Assert.AreEqual("hammett@apache.org", ((LeafNode)entries[1]).Value);
			Assert.AreEqual("hammett@uol.com.br", ((LeafNode)entries[2]).Value);
		}

		[Test, ExpectedException(typeof(BindingException), "Attempt to create or obtain a composite node named Process, but a node with the same exists with the type Leaf")]
		public void RepeatedNamesForNodes()
		{
			NameValueCollection nameValueColl = new NameValueCollection();
			
			nameValueColl.Add("profile.Process", "test");
			nameValueColl.Add("profile.Process.Id", "1");

			TreeBuilder builder = new TreeBuilder();
			CompositeNode root = builder.BuildSourceNode(nameValueColl);
		}
	}
}
