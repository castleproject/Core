// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

using Castle.DynamicProxy.Tests.Mixins;

namespace Castle.DynamicProxy.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class MixinDataTestCase
	{
		private SimpleMixin simpleMixin;
		private OtherMixin otherMixin;
		private ComplexMixin complexMixin;

		[SetUp]
		public void SetUp ()
		{
			simpleMixin = new SimpleMixin ();
			otherMixin = new OtherMixin ();
			complexMixin = new ComplexMixin ();
		}

		[Test]
		public void GetMixinInterfaceImplementationsAsArray ()
		{
			MixinData mixinData = new MixinData (new object[] { simpleMixin });
			object[] mixinsAsArray = mixinData.GetMixinInterfaceImplementationsAsArray ();
			Assert.AreEqual (1, mixinsAsArray.Length);
			Assert.AreSame (simpleMixin, mixinsAsArray[0]);
		}

		[Test]
		public void MixinsNotImplementingInterfacesAreIgnored ()
		{
			MixinData mixinData = new MixinData (new object[] { new object() });
			object[] mixinsAsArray = mixinData.GetMixinInterfaceImplementationsAsArray ();
			Assert.AreEqual (0, mixinsAsArray.Length);
		}


		[Test]
		public void MixinsAreSortedByInterface ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			object[] mixinsAsArray1 = mixinData1.GetMixinInterfaceImplementationsAsArray ();
			Assert.AreEqual (2, mixinsAsArray1.Length);
			Assert.AreSame (otherMixin, mixinsAsArray1[0]);
			Assert.AreSame (simpleMixin, mixinsAsArray1[1]);

			MixinData mixinData2 = new MixinData (new object[] { otherMixin, simpleMixin });
			object[] mixinsAsArray2 = mixinData2.GetMixinInterfaceImplementationsAsArray ();
			Assert.AreEqual (2, mixinsAsArray2.Length);
			Assert.AreSame (otherMixin, mixinsAsArray2[0]);
			Assert.AreSame (simpleMixin, mixinsAsArray2[1]);
		}

		[Test]
		public void MixinInterfacesAndPositions ()
		{
			MixinData mixinData = new MixinData (new object[] { simpleMixin });
			Assert.AreEqual (1, mixinData.MixinInterfacesAndPositions.Count);
			Assert.AreEqual (0, mixinData.MixinInterfacesAndPositions[typeof (ISimpleMixin)]);
		}

		[Test]
		public void MixinInterfacesAndPositions_SortedLikeMixins ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			Assert.AreEqual (2, mixinData1.MixinInterfacesAndPositions.Count);
			Assert.AreEqual (0, mixinData1.MixinInterfacesAndPositions[typeof (IOtherMixin)]);
			Assert.AreEqual (1, mixinData1.MixinInterfacesAndPositions[typeof (ISimpleMixin)]);

			MixinData mixinData2 = new MixinData (new object[] { otherMixin, simpleMixin });
			Assert.AreEqual (2, mixinData2.MixinInterfacesAndPositions.Count);
			Assert.AreEqual (0, mixinData2.MixinInterfacesAndPositions[typeof (IOtherMixin)]);
			Assert.AreEqual (1, mixinData2.MixinInterfacesAndPositions[typeof (ISimpleMixin)]);
		}

		[Test]
		public void Equals_True_WithDifferentOrder ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { otherMixin, simpleMixin });
			Assert.AreEqual (mixinData1, mixinData2);
		}

		[Test]
		public void Equals_True_WithDifferentInstances ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { new SimpleMixin(), new OtherMixin() });
			Assert.AreEqual (mixinData1, mixinData2);
		}

		[Test]
		public void Equals_False_WithDifferentInstances ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { simpleMixin, complexMixin });
			Assert.AreNotEqual (mixinData1, mixinData2);
		}

		[Test]
		public void Equals_False_WithInstanceCount ()
		{
			MixinData mixinData1 = new MixinData (new object[] { otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { otherMixin, simpleMixin});
			Assert.AreNotEqual (mixinData1, mixinData2);
		}

		[Test]
		public void GetHashCode_Equal_WithDifferentOrder ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { otherMixin, simpleMixin });
			Assert.AreEqual (mixinData1.GetHashCode (), mixinData2.GetHashCode ());
		}

		[Test]
		public void GetHashCode_Equal_WithDifferentInstances ()
		{
			MixinData mixinData1 = new MixinData (new object[] { simpleMixin, otherMixin });
			MixinData mixinData2 = new MixinData (new object[] { new SimpleMixin (), new OtherMixin () });
			Assert.AreEqual (mixinData1.GetHashCode (), mixinData2.GetHashCode ());
		}
	}
}