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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Tests.Mixins;

	using CastleTests;

	using NUnit.Framework;

	using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

	[TestFixture]
	public class ProxyGenerationOptionsTestCase
	{
		private ProxyGenerationOptions _options1;
		private ProxyGenerationOptions _options2;

#if FEATURE_XUNITNET
		public ProxyGenerationOptionsTestCase()
#else
		[SetUp]
		public void Init()
#endif
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();
		}

		[Test]
		public void MixinData_NeedsInitialize()
		{
			Assert.Throws<InvalidOperationException>(delegate {
#pragma warning disable 219
				MixinData data = _options1.MixinData;
#pragma warning restore 219
			});
		}

		[Test]
		public void MixinData()
		{
			_options1.Initialize();
			MixinData data = _options1.MixinData;
			Assert.AreEqual(0, new List<object>(data.Mixins).Count);
		}

		[Test]
		public void MixinData_WithMixins()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();
			MixinData data = _options1.MixinData;
			Assert.AreEqual(1, new List<object>(data.Mixins).Count);
		}

		[Test]
		public void MixinData_NoReInitializeWhenNothingChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			MixinData data1 = _options1.MixinData;
			_options1.Initialize();
			MixinData data2 = _options1.MixinData;
			Assert.AreSame(data1, data2);
		}

		[Test]
		public void MixinData_ReInitializeWhenMixinsChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			MixinData data1 = _options1.MixinData;

			_options1.AddMixinInstance(new OtherMixin());
			_options1.Initialize();
			MixinData data2 = _options1.MixinData;
			Assert.AreNotSame(data1, data2);

			Assert.AreEqual (1, new List<object>(data1.Mixins).Count);
			Assert.AreEqual (2, new List<object>(data2.Mixins).Count);
		}

		[Test]
		public void Equals_EmptyOptions()
		{
			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (IConvertible);

			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);

			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (object);

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_AddMixinInstance()
		{
			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_Hook()
		{
			IProxyGenerationHook hook = new LogHook(typeof(object), true);
			_options1.Hook = hook;

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void Equals_ComparesMixinTypesNotInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_ComparesSortedMixinTypes()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.AddMixinInstance(new ComplexMixin());

			_options2.AddMixinInstance(new ComplexMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_Compares_selectors_existence()
		{
			_options1.Selector = new AllInterceptorSelector();
			_options2.Selector = new TypeInterceptorSelector<StandardInterceptor>();

			Assert.AreEqual(_options1, _options2);

			_options2.Selector = null;
			Assert.AreNotEqual(_options1, _options2);

			_options1.Selector = null;
			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentAdditionalAttributes()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void Equals_SameAdditionalAttributes()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test" });
			_options1.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_SameAdditionalAttributesDifferentOrder()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder2);

			_options2.AdditionalAttributes.Add(builder2);
			_options2.AdditionalAttributes.Add(builder1);

			Assert.AreEqual(_options1, _options2);
		}

		[Test]
		public void Equals_DifferentAdditionalAttributesDuplicateEntries()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder1);

			_options2.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.AreNotEqual(_options1, _options2);
		}

		[Test]
		public void GetHashCode_EmptyOptions()
		{
			Assert.AreEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (IConvertible);

			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);

			
			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.AreEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_EqualOptions_DifferentMixinInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.AreEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (object);

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_AddMixinInstance()
		{
			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_Hook()
		{
			IProxyGenerationHook hook = new LogHook(typeof (object), true);
			_options1.Hook = hook;

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_EqualOptions_SameAdditionalAttributes()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new [] { typeof(string) }), new object[] { "Test" });
			_options1.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.AreEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_EqualOptions_SameAdditionalAttributesDifferentOrder()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder2);

			_options2.AdditionalAttributes.Add(builder2);
			_options2.AdditionalAttributes.Add(builder1);

			Assert.AreEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_DifferentAdditionalAttributes()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_SameAdditionalAttributesButDuplicateEntries()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }), new object[] { "Test1" });

			_options1.AdditionalAttributes.Add(builder);

			_options2.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.AreNotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}
	}
}