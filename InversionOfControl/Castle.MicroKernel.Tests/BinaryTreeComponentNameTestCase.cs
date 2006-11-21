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

namespace Castle.MicroKernel.Tests
{
	using System;
	using NUnit.Framework;
	using Castle.Core;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.SubSystems.Naming;

	[TestFixture]
	public class BinaryTreeComponentNameTestCase
	{
		[Test]
		public void Usage()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();

			DefaultHandler handler1 = new DefaultHandler(
				new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler2 = new DefaultHandler(
				new ComponentModel("B", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler3 = new DefaultHandler(
				new ComponentModel("C", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler4 = new DefaultHandler(
				new ComponentModel("D", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler5 = new DefaultHandler(
				new ComponentModel("E", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler6 = new DefaultHandler(
				new ComponentModel("F", typeof(DefaultHandler), typeof(DefaultHandler)));

			tree.Add(new ComponentName("protocolhandler"), handler1);
			tree.Add(new ComponentName("protocolhandler:key=1"), handler2);
			tree.Add(new ComponentName("protocolhandler:key=2"), handler3);
			tree.Add(new ComponentName("protocolhandler:key=2,secure=true"), handler4);
			tree.Add(new ComponentName("modelmanager"), handler5);
			tree.Add(new ComponentName("viewmanager"), handler6);

			Assert.AreSame(handler1, tree.GetHandler(new ComponentName("protocolhandler")));
			Assert.AreSame(handler2, tree.GetHandler(new ComponentName("protocolhandler:key=1")));
			Assert.AreSame(handler3, tree.GetHandler(new ComponentName("protocolhandler:key=2")));
			Assert.AreSame(handler4, tree.GetHandler(new ComponentName("protocolhandler:key=2,secure=true")));
			Assert.AreSame(handler5, tree.GetHandler(new ComponentName("modelmanager")));
			Assert.AreSame(handler6, tree.GetHandler(new ComponentName("viewmanager")));

			IHandler[] handlers = tree.GetHandlers(new ComponentName("protocolhandler"));

			Assert.AreEqual(4, handlers.Length);
			Assert.AreSame(handler1, handlers[0]);
			Assert.AreSame(handler2, handlers[1]);
			Assert.AreSame(handler3, handlers[2]);
			Assert.AreSame(handler4, handlers[3]);

			handlers = tree.GetHandlers(new ComponentName("protocolhandler:*"));

			Assert.AreEqual(4, handlers.Length);
			Assert.AreSame(handler1, handlers[0]);
			Assert.AreSame(handler2, handlers[1]);
			Assert.AreSame(handler3, handlers[2]);
			Assert.AreSame(handler4, handlers[3]);

			handlers = tree.GetHandlers(new ComponentName("protocolhandler:secure=true"));

			Assert.AreEqual(1, handlers.Length);
			Assert.AreSame(handler4, handlers[0]);

			handlers = tree.GetHandlers(new ComponentName("protocolhandler:key=2"));

			Assert.AreEqual(2, handlers.Length);
			Assert.AreSame(handler3, handlers[0]);
			Assert.AreSame(handler4, handlers[1]);
		}


		[Test]
		public void ComponentNameEmptyProperties()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));
			DefaultHandler handler2 = new DefaultHandler(new ComponentModel("B", typeof(DefaultHandler), typeof(DefaultHandler)));
			tree.Add(new ComponentName("protocolhandler:key=1"), handler2);
			tree.Add(new ComponentName("protocolhandler"), handler1);

			Assert.AreEqual(handler1, tree.GetHandler(new ComponentName("protocolhandler")));
		}

		[Test]
		public void RemoveUnbalancedRoot()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));
			tree.Add(new ComponentName("1000"), handler1);
			tree.Add(new ComponentName("7500"), handler1);
			tree.Add(new ComponentName("6000"), handler1);
			tree.Add(new ComponentName("2000"), handler1);

			tree.Remove(new ComponentName("1000"));
			assertRemoved(tree, 3, new ComponentName("1000"), new ComponentName("6000"));
		}


		[Test]
		public void RemoveBalancedRoot()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));
			tree.Add(new ComponentName("1000"), handler1);
			tree.Add(new ComponentName("0500"), handler1);
			tree.Add(new ComponentName("6000"), handler1);
			tree.Add(new ComponentName("2000"), handler1);

			tree.Remove(new ComponentName("1000"));
			assertRemoved(tree, 3, new ComponentName("1000"), new ComponentName("6000"));
		}

		[Test]
		public void RemoveSibling()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));

			tree.Add(new ComponentName("1000"), handler1);
			tree.Add(new ComponentName("0500"), handler1);
			tree.Add(new ComponentName("0500:P=1"), handler1);
			tree.Add(new ComponentName("0500:p=2"), handler1);

			tree.Remove(new ComponentName("0500:p=2"));
			assertRemoved(tree, 3, new ComponentName("0500:P=2"), new ComponentName("0500:P=1"));
		}


		[Test]
		public void RemoveLeaf()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));

			tree.Add(new ComponentName("1000"), handler1);
			tree.Add(new ComponentName("0500"), handler1);

			tree.Remove(new ComponentName("0500"));
			assertRemoved(tree, 1, new ComponentName("0500"), new ComponentName("1000"));
		}

		[Test]
		public void RemoveNodeWithSiblings()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			DefaultHandler handler1 = new DefaultHandler(new ComponentModel("A", typeof(DefaultHandler), typeof(DefaultHandler)));

			tree.Add(new ComponentName("1000"), handler1);
			tree.Add(new ComponentName("0500"), handler1);
			tree.Add(new ComponentName("0500:P=1"), handler1);
			tree.Add(new ComponentName("0500:p=2"), handler1);
			tree.Add(new ComponentName("0400"), handler1);

			tree.Remove(new ComponentName("0500"));
			Assert.AreEqual(4, tree.Count);
			Assert.AreEqual(4, tree.Handlers.Length);
			Assert.AreEqual(2, tree.GetHandlers(new ComponentName("0500")).Length);
			Assert.IsNotNull(tree.GetHandler(new ComponentName("0500:p=2")));
		}


		private void assertRemoved(BinaryTreeComponentName tree, int expectedCount, ComponentName removed,
		                           ComponentName exists)
		{
			Assert.AreEqual(expectedCount, tree.Count);
			Assert.AreEqual(expectedCount, tree.Handlers.Length);
			Assert.IsNull(tree.GetHandler(removed));
			Assert.IsNotNull(tree.GetHandler(exists));
		}


		[Test]
		public void AccessEmptyTree()
		{
			BinaryTreeComponentName tree = new BinaryTreeComponentName();
			Assert.IsFalse(tree.Contains(new ComponentName("Test")));
		}
	}
}