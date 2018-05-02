// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class VirtualObjectTestCase
	{
		[Test]
		public void Virtual_Realize()
		{
			var target = new MockVirtualTarget();
			var obj    = MockVirtual.CreateVirtual(NodeA, target, MemberA);

			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
			Assert.AreSame(NodeA, target.RealizedNode);
			Assert.AreSame(MemberA, target.RealizedMember);
		}

		[Test]
		public void Virtual_Realize_Event()
		{
			var target = new MockVirtualTarget();
			var obj    = MockVirtual.CreateVirtual(NodeA, target, MemberA);
			var fired  = false;

			obj.Realized += (sender, args) =>
			{
				Assert.AreSame(obj, sender);
				Assert.NotNull(args);
				Assert.IsInstanceOf<EventArgs>(args);
				fired = true;
			};

			obj.Realize();

			Assert.True(fired, "Event fired");
		}

		[Test]
		public void Virtual_AddSite()
		{
			var target1 = new MockVirtualTarget();
			var target2 = new MockVirtualTarget();
			var obj     = MockVirtual.CreateVirtual(NodeA, target1, MemberA);

			obj.AddSite(new VirtualSite<Node, Member>(target2, MemberB));
			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
			Assert.AreSame(NodeA, target1.RealizedNode);
			Assert.AreSame(MemberA, target1.RealizedMember);
			Assert.AreSame(NodeA, target2.RealizedNode);
			Assert.AreSame(MemberB, target2.RealizedMember);
		}

		[Test]
		public void Virtual_RemoveSite()
		{
			var target1 = new MockVirtualTarget();
			var target2 = new MockVirtualTarget();
			var obj   = MockVirtual.CreateVirtual(NodeA, target1, MemberA);

			obj.AddSite   (new VirtualSite<Node, Member>(target2, MemberB));
			obj.RemoveSite(new VirtualSite<Node, Member>(target1, MemberA));
			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
			Assert.IsNull(target1.RealizedNode);
			Assert.IsNull(target1.RealizedMember);
			Assert.AreSame(NodeA, target2.RealizedNode);
			Assert.AreSame(MemberB, target2.RealizedMember);
		}

		[Test]
		public void Real_Realize()
		{
			var obj = MockVirtual.CreateReal(NodeA);

			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
		}

		[Test]
		public void Real_Realize_Event()
		{
			var obj   = MockVirtual.CreateReal(NodeA);
			var fired = false;

			obj.Realized += (sender, args) =>
			{
				fired = true;
			};

			obj.Realize();

			Assert.False(fired, "Event fired");
		}

		[Test]
		public void Real_AddSite()
		{
			var target = new MockVirtualTarget();
			var obj    = MockVirtual.CreateReal(NodeA);

			obj.AddSite(new VirtualSite<Node, Member>(target, MemberA));
			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
			Assert.IsNull(target.RealizedNode);
			Assert.IsNull(target.RealizedMember);
		}

		[Test]
		public void Real_RemoveSite()
		{
			var target = new MockVirtualTarget();
			var obj    = MockVirtual.CreateReal(NodeA);

			obj.RemoveSite(new VirtualSite<Node, Member>(target, MemberA));
			var realizedNode = obj.Realize();

			Assert.AreSame(NodeA, realizedNode);
			Assert.IsNull(target.RealizedNode);
			Assert.IsNull(target.RealizedMember);
		}

		private static readonly Node
			NodeA = new Node();

		private static readonly Member
			MemberA = new Member(),
			MemberB = new Member();

		private sealed class Node   { }
		private sealed class Member { }

		private sealed class MockVirtual : VirtualObject<Node>
		{
			public static IVirtual<Node> CreateReal(Node node)
			{
				return new MockVirtual() { RealNode = node };
			}

			public static IVirtual<Node> CreateVirtual(Node node, IVirtualTarget<Node, Member> target, Member member)
			{
				var site = new VirtualSite<Node, Member>(target, member);

				return new MockVirtual(site) { PendingNode = node };
			}

			private MockVirtual()
				: base() { }

			private MockVirtual(IVirtualSite<Node> site)
				: base(site) { }

			private Node PendingNode;
			private Node RealNode;

			public override bool IsReal
			{
				get { return RealNode != null; }
			}

			protected override bool TryRealize(out Node node)
			{
				if (IsReal)
				{
					node = RealNode;
					return false;
				}
				else
				{
					node = RealNode = PendingNode;
					return true;
				}
			}
		}

		private sealed class MockVirtualTarget : IVirtualTarget<Node, Member>
		{
			public void OnRealizing(Node node, Member member)
			{
				RealizedNode   = node;
				RealizedMember = member;
			}

			public Node   RealizedNode   { get; private set; }
			public Member RealizedMember { get; private set; }
		}
	}
}
