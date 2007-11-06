namespace Castle.DynamicProxy.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class ProxyGenerationOptionsTestCase
	{
		ProxyGenerationOptions _options1;
		ProxyGenerationOptions _options2;

		[SetUp]
		public void Init()
		{
			_options1 = new ProxyGenerationOptions();
			_options2 = new ProxyGenerationOptions();
		}

		[Test]
		public void Equals_EmptyOptions ()
		{
			Assert.AreEqual (_options1, _options2);
		}

		[Test]
		public void Equals_EqualNonEmptyOptions ()
		{
			_options1 = new ProxyGenerationOptions ();
			_options2 = new ProxyGenerationOptions ();

			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (IConvertible);

			object mixin = new object ();
			_options1.AddMixinInstance (mixin);
			_options2.AddMixinInstance (mixin);

			IProxyGenerationHook hook = new AllMethodsHook ();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new Castle.DynamicProxy.Tests.SerializableClassTestCase.SerializableInterceptorSelector ();
			_options1.Selector = selector;
			_options2.Selector = selector;

			_options1.UseSelector = true;
			_options2.UseSelector = true;

			_options1.UseSingleInterfaceProxy = true;
			_options2.UseSingleInterfaceProxy = true;

			Assert.AreEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_BaseTypeForInterfaceProxy ()
		{
			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (object);

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_AddMixinInstance ()
		{
			object mixin = new object ();
			_options1.AddMixinInstance (mixin);

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_Hook ()
		{
			IProxyGenerationHook hook = new GenerationHookTestCase.LogHook (typeof (object), true);
			_options1.Hook = hook;

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_Selector ()
		{
			IInterceptorSelector selector = new Castle.DynamicProxy.Tests.SerializableClassTestCase.SerializableInterceptorSelector ();
			_options1.Selector = selector;

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_UseSelector ()
		{
			_options1.UseSelector = true;
			_options2.UseSelector = false;

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void Equals_DifferentOptions_UseSingleInterfaceProxy ()
		{
			_options1.UseSingleInterfaceProxy = true;
			_options2.UseSingleInterfaceProxy = false;

			Assert.AreNotEqual (_options1, _options2);
		}

		[Test]
		public void GetHashCode_EmptyOptions ()
		{
			Assert.AreEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_EqualNonEmptyOptions ()
		{
			_options1 = new ProxyGenerationOptions ();
			_options2 = new ProxyGenerationOptions ();

			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (IConvertible);

			object mixin = new object ();
			_options1.AddMixinInstance (mixin);
			_options2.AddMixinInstance (mixin);

			IProxyGenerationHook hook = new AllMethodsHook ();
			_options1.Hook = hook;
			_options2.Hook = hook;

			IInterceptorSelector selector = new Castle.DynamicProxy.Tests.SerializableClassTestCase.SerializableInterceptorSelector ();
			_options1.Selector = selector;
			_options2.Selector = selector;

			_options1.UseSelector = true;
			_options2.UseSelector = true;

			_options1.UseSingleInterfaceProxy = true;
			_options2.UseSingleInterfaceProxy = true;

			Assert.AreEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_BaseTypeForInterfaceProxy ()
		{
			_options1.BaseTypeForInterfaceProxy = typeof (IConvertible);
			_options2.BaseTypeForInterfaceProxy = typeof (object);

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_AddMixinInstance ()
		{
			object mixin = new object ();
			_options1.AddMixinInstance (mixin);

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_Hook ()
		{
			IProxyGenerationHook hook = new GenerationHookTestCase.LogHook (typeof (object), true);
			_options1.Hook = hook;

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_Selector ()
		{
			IInterceptorSelector selector = new Castle.DynamicProxy.Tests.SerializableClassTestCase.SerializableInterceptorSelector ();
			_options1.Selector = selector;

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_UseSelector ()
		{
			_options1.UseSelector = true;
			_options2.UseSelector = false;

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}

		[Test]
		public void GetHashCode_DifferentOptions_UseSingleInterfaceProxy ()
		{
			_options1.UseSingleInterfaceProxy = true;
			_options2.UseSingleInterfaceProxy = false;

			Assert.AreNotEqual (_options1.GetHashCode(), _options2.GetHashCode());
		}
	}
}