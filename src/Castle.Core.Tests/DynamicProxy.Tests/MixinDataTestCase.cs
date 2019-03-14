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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using Castle.DynamicProxy.Tests.Mixins;
	using NUnit.Framework;

	[TestFixture]
	public class MixinDataTestCase
	{
		private SimpleMixin simpleMixin;
		private OtherMixin otherMixin;
		private ComplexMixin complexMixin;

		[SetUp]
		public void SetUp()
		{
			simpleMixin = new SimpleMixin();
			otherMixin = new OtherMixin();
			complexMixin = new ComplexMixin();
		}

		[Test]
		public void Mixins()
		{
			MixinData mixinData = new MixinData(new object[] {simpleMixin});
			List<object> mixins = new List<object>(mixinData.Mixins);
			Assert.AreEqual(1, mixins.Count);
			Assert.AreSame(simpleMixin, mixins[0]);
		}

		[Test]
		public void ContainsMixinWithInterface()
		{
			MixinData mixinData = new MixinData(new object[] { simpleMixin });
			Assert.IsTrue(mixinData.ContainsMixin(typeof(ISimpleMixin)));
			Assert.IsFalse(mixinData.ContainsMixin(typeof(IOtherMixin)));
		}

		[Test]
		public void MixinsNotImplementingInterfacesAreIgnored()
		{
			MixinData mixinData = new MixinData(new object[] {new object()});
			List<object> mixins = new List<object>(mixinData.Mixins);
			Assert.AreEqual(0, mixins.Count);
		}

		[Test]
		public void MixinsAreSortedByInterface()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			List<object> mixins1 = new List<object>(mixinData1.Mixins);
			Assert.AreEqual(2, mixins1.Count);
			Assert.AreSame(otherMixin, mixins1[0]);
			Assert.AreSame(simpleMixin, mixins1[1]);

			MixinData mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			List<object> mixins2 = new List<object>(mixinData2.Mixins);
			Assert.AreEqual(2, mixins2.Count);
			Assert.AreSame(otherMixin, mixins2[0]);
			Assert.AreSame(simpleMixin, mixins2[1]);
		}

		[Test]
		public void MixinInterfaces()
		{
			MixinData mixinData = new MixinData(new object[] { simpleMixin });
			List<Type> mixinInterfaces = new List<Type>(mixinData.MixinInterfaces);
			Assert.AreEqual(1, mixinInterfaces.Count);
			Assert.AreSame(mixinInterfaces[0], typeof (ISimpleMixin));
		}

		[Test]
		public void MixinInterfaces_SortedLikeMixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { simpleMixin, otherMixin });
			List<Type> mixinInterfaces1 = new List<Type>(mixinData1.MixinInterfaces);
			Assert.AreEqual(2, mixinInterfaces1.Count);
			Assert.AreSame(typeof(IOtherMixin), mixinInterfaces1[0]);
			Assert.AreSame(typeof(ISimpleMixin), mixinInterfaces1[1]);

			MixinData mixinData2 = new MixinData(new object[] { otherMixin, simpleMixin });
			List<Type> mixinInterfaces2 = new List<Type>(mixinData2.MixinInterfaces);
			Assert.AreEqual(2, mixinInterfaces2.Count);
			Assert.AreSame(typeof (IOtherMixin), mixinInterfaces2[0]);
			Assert.AreSame(typeof (ISimpleMixin), mixinInterfaces2[1]);
		}

		[Test]
		public void GetMixinPosition()
		{
			MixinData mixinData = new MixinData(new object[] { simpleMixin });
			Assert.AreEqual(0, mixinData.GetMixinPosition(typeof(ISimpleMixin)));
		}

		[Test]
		public void GetMixinPosition_MatchesMixinInstances()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			Assert.AreEqual(0, mixinData1.GetMixinPosition(typeof(IOtherMixin)));
			Assert.AreEqual(1, mixinData1.GetMixinPosition(typeof(ISimpleMixin)));

			MixinData mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.AreEqual(0, mixinData2.GetMixinPosition(typeof(IOtherMixin)));
			Assert.AreEqual(1, mixinData2.GetMixinPosition(typeof(ISimpleMixin)));
		}

		[Test]
		public void GetMixinPosition_MatchesMixinInstances_WithMultipleInterfacesPerMixin()
		{
			MixinData mixinData = new MixinData(new object[] { complexMixin, simpleMixin });
			Assert.AreEqual(0, mixinData.GetMixinPosition(typeof(IFirst)));
			Assert.AreEqual(1, mixinData.GetMixinPosition(typeof(ISecond)));
			Assert.AreEqual(2, mixinData.GetMixinPosition(typeof(ISimpleMixin)));
			Assert.AreEqual(3, mixinData.GetMixinPosition(typeof(IThird)));

			List<object> mixins = new List<object>(mixinData.Mixins);
			Assert.AreSame(complexMixin, mixins[0]);
			Assert.AreSame(complexMixin, mixins[1]);
			Assert.AreSame(simpleMixin, mixins[2]);
			Assert.AreSame(complexMixin, mixins[3]);
		}

		[Test]
		public void Equals_True_WithDifferentOrder()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.AreEqual(mixinData1, mixinData2);
		}

		[Test]
		public void Equals_True_WithDifferentInstances()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {new SimpleMixin(), new OtherMixin()});
			Assert.AreEqual(mixinData1, mixinData2);
		}

		[Test]
		public void Equals_False_WithDifferentInstances()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {simpleMixin, complexMixin});
			Assert.AreNotEqual(mixinData1, mixinData2);
		}

		[Test]
		public void Equals_False_WithInstanceCount()
		{
			MixinData mixinData1 = new MixinData(new object[] {otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.AreNotEqual(mixinData1, mixinData2);
		}

		[Test]
		public void GetHashCode_Equal_WithDifferentOrder()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {otherMixin, simpleMixin});
			Assert.AreEqual(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		[Test]
		public void GetHashCode_Equal_WithDifferentInstances()
		{
			MixinData mixinData1 = new MixinData(new object[] {simpleMixin, otherMixin});
			MixinData mixinData2 = new MixinData(new object[] {new SimpleMixin(), new OtherMixin()});
			Assert.AreEqual(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		[Test]
		public void TwoMixinsWithSameInterfaces()
		{
			SimpleMixin mixin1 = new SimpleMixin();
			OtherMixinImplementingISimpleMixin mixin2 = new OtherMixinImplementingISimpleMixin();

			Assert.Throws<ArgumentException>(() =>
				new MixinData(new object[] { mixin1, mixin2 })
			);
		}

		[Test]
		public void Equals_false_for_two_instances_having_different_delegate_type_mixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { typeof(Action) });
			MixinData mixinData2 = new MixinData(new object[] { typeof(Action<int>) });
			Assert.AreNotEqual(mixinData1, mixinData2);
		}

		[Test]
		public void Equals_true_for_two_instances_having_same_delegate_type_mixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { typeof(Action) });
			MixinData mixinData2 = new MixinData(new object[] { typeof(Action) });
			Assert.AreEqual(mixinData1, mixinData2);
		}

		[Test]
		public void GetHashCode_equal_for_two_instances_having_same_delegate_type_mixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { typeof(Action) });
			MixinData mixinData2 = new MixinData(new object[] { typeof(Action) });
			Assert.AreEqual(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		// This test appears to be incorrect. However, remember that GetHashCode is allowed to
		// sacrifice accuracy for speed. It's not a full replacement for the more expensive
		// Equals check; the only requirement is that equal object have equal hashcodes.
		[Test]
		public void GetHashCode_equal_for_two_instances_having_different_delegate_type_mixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { typeof(Action) });
			MixinData mixinData2 = new MixinData(new object[] { typeof(Action<int>) });
			Assert.AreEqual(mixinData1.GetHashCode(), mixinData2.GetHashCode());
		}

		// This is a current limitation that means DynamicProxy will might not recognize
		// suitable cached proxy types for such compatible delegate scenarios, and regenerate
		// an identical type. This might not even matter that much in practice.
		[Test]
		public void Equals_false_for_two_instances_having_compatible_delegate_type_mixins()
		{
			MixinData mixinData1 = new MixinData(new object[] { typeof(Func<object, bool>) });
			MixinData mixinData2 = new MixinData(new object[] { typeof(Predicate<object>) });
			Assert.AreNotEqual(mixinData1, mixinData2);
		}

		[Test]
		public void Ctor_succeeds_when_mixing_regular_mixin_instances_with_delegate_mixins()
		{
			var mixinData = new MixinData(new object[]
			{
				new NotADelegate(),
				new Action(() => { }),
			});
		}

		[Test]
		public void Ctor_succeeds_when_mixing_regular_mixin_instances_with_delegate_type_mixins()
		{
			var mixinData = new MixinData(new object[]
			{
				new NotADelegate(),
				typeof(Action),
			});
		}

		[Test]
		public void Ctor_throws_when_multiple_delegate_mixins_for_same_Invoke_signature()
		{
			Assert.Throws<ArgumentException>(() => new MixinData(new object[]
			{
				new NotADelegate(),
				new Func<object, bool>(_ => true),
				new Predicate<object>(_ => false),
			}));
		}

		[Test]
		public void Ctor_throws_when_multiple_delegate_type_mixins_for_same_Invoke_signature()
		{
			Assert.Throws<ArgumentException>(() => new MixinData(new object[]
			{
				typeof(Func<object, bool>),
				new NotADelegate(),
				typeof(Predicate<object>)
			}));
		}

		public class NotADelegate : INotADelegate { }

		public interface INotADelegate { }
	}
}