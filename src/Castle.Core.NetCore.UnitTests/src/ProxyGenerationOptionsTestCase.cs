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

	// TODO: Does it have to be this description attribute?
#if !NETCORE
	using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
#endif
	using Xunit;

	public class ProxyGenerationOptionsTestCase
	{
		private ProxyGenerationOptions _options1;
		private ProxyGenerationOptions _options2;

		public ProxyGenerationOptionsTestCase()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();
		}

		//TODO
		//[Fact]
		//[ExpectedException(typeof(InvalidOperationException))]
		public void MixinData_NeedsInitialize()
		{
#pragma warning disable 219
			MixinData data = _options1.MixinData;
#pragma warning restore 219
		}

		[Fact]
		public void MixinData()
		{
			_options1.Initialize();
			MixinData data = _options1.MixinData;
			Assert.Equal(0, new List<object>(data.Mixins).Count);
		}

		[Fact]
		public void MixinData_WithMixins()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();
			MixinData data = _options1.MixinData;
			Assert.Equal(1, new List<object>(data.Mixins).Count);
		}

		[Fact]
		public void MixinData_NoReInitializeWhenNothingChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			MixinData data1 = _options1.MixinData;
			_options1.Initialize();
			MixinData data2 = _options1.MixinData;
			Assert.Same(data1, data2);
		}

		[Fact]
		public void MixinData_ReInitializeWhenMixinsChanged()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.Initialize();

			MixinData data1 = _options1.MixinData;

			_options1.AddMixinInstance(new OtherMixin());
			_options1.Initialize();
			MixinData data2 = _options1.MixinData;
			Assert.NotSame(data1, data2);

			Assert.Equal(1, new List<object>(data1.Mixins).Count);
			Assert.Equal(2, new List<object>(data2.Mixins).Count);
		}

		[Fact]
		public void Equals_EmptyOptions()
		{
			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(IConvertible);

			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);

			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(object);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentOptions_AddMixinInstance()
		{
			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.NotEqual(_options1, _options2);
		}

		// TODO
		//[Fact]
		//public void Equals_DifferentOptions_Hook()
		//{
		//	IProxyGenerationHook hook = new LogHook(typeof(object), true);
		//	_options1.Hook = hook;

		//	Assert.NotEqual(_options1, _options2);
		//}

		[Fact]
		public void Equals_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_ComparesMixinTypesNotInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_ComparesSortedMixinTypes()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options1.AddMixinInstance(new ComplexMixin());

			_options2.AddMixinInstance(new ComplexMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_Compares_selectors_existence()
		{
			_options1.Selector = new AllInterceptorSelector();
			_options2.Selector = new TypeInterceptorSelector<StandardInterceptor>();

			Assert.Equal(_options1, _options2);

			_options2.Selector = null;
			Assert.NotEqual(_options1, _options2);

			_options1.Selector = null;
			Assert.Equal(_options1, _options2);
		}

#if !NETCORE
		[Fact]
		public void Equals_DifferentAdditionalAttributes()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.NotEqual(_options1, _options2);
		}

		[Fact]
		public void Equals_SameAdditionalAttributes()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test" });
			_options1.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_SameAdditionalAttributesDifferentOrder()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder2);

			_options2.AdditionalAttributes.Add(builder2);
			_options2.AdditionalAttributes.Add(builder1);

			Assert.Equal(_options1, _options2);
		}

		[Fact]
		public void Equals_DifferentAdditionalAttributesDuplicateEntries()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder1);

			_options2.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.NotEqual(_options1, _options2);
		}
#endif

		[Fact]
		public void GetHashCode_EmptyOptions()
		{
			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualNonEmptyOptions()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();

			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(IConvertible);

			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);
			_options2.AddMixinInstance(mixin);

			IProxyGenerationHook hook = new AllMethodsHook();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new AllInterceptorSelector();
			_options1.Selector = selector;
			_options2.Selector = selector;

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualOptions_DifferentMixinInstances()
		{
			_options1.AddMixinInstance(new SimpleMixin());
			_options2.AddMixinInstance(new SimpleMixin());

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_BaseTypeForInterfaceProxy()
		{
			_options1.BaseTypeForInterfaceProxy = typeof(IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof(object);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_AddMixinInstance()
		{
			SimpleMixin mixin = new SimpleMixin();
			_options1.AddMixinInstance(mixin);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		// TODO
		//[Fact]
		//public void GetHashCode_DifferentOptions_Hook()
		//{
		//	IProxyGenerationHook hook = new LogHook(typeof (object), true);
		//	_options1.Hook = hook;

		//	Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		//}

		[Fact]
		public void GetHashCode_DifferentOptions_Selector()
		{
			_options1.Selector = new AllInterceptorSelector();

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

#if !NETCORE
		[Fact]
		public void GetHashCode_EqualOptions_SameAdditionalAttributes()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test" });
			_options1.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_EqualOptions_SameAdditionalAttributesDifferentOrder()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options1.AdditionalAttributes.Add(builder2);

			_options2.AdditionalAttributes.Add(builder2);
			_options2.AdditionalAttributes.Add(builder1);

			Assert.Equal(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_DifferentAdditionalAttributes()
		{
			var builder1 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });
			var builder2 = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test2" });

			_options1.AdditionalAttributes.Add(builder1);
			_options2.AdditionalAttributes.Add(builder2);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Fact]
		public void GetHashCode_DifferentOptions_SameAdditionalAttributesButDuplicateEntries()
		{
			var builder = new CustomAttributeBuilder(typeof(DescriptionAttribute).GetConstructor(new[] { typeof(string) }),
				new object[] { "Test1" });

			_options1.AdditionalAttributes.Add(builder);

			_options2.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);
			_options2.AdditionalAttributes.Add(builder);

			Assert.NotEqual(_options1.GetHashCode(), _options2.GetHashCode());
		}
#endif
	}
}