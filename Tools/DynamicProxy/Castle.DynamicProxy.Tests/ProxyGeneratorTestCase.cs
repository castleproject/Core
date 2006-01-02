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

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Reflection;

	using NUnit.Framework;

	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Test.Classes;
	using Castle.DynamicProxy.Test.Interceptors;
	using Castle.DynamicProxy.Test.ClassInterfaces;
	

	[TestFixture]
	public class ProxyGeneratorTestCase
	{
		private ProxyGenerator _generator;

		[SetUp]
		public void Init()
		{
			_generator = new ProxyGenerator();
		}

		[Test]
		public void ProxyForClass()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ServiceClass), new ResultModifiedInvocationHandler( ) );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(ServiceClass).IsAssignableFrom( proxy.GetType() ) );

			ServiceClass inter = (ServiceClass) proxy;

			Assert.AreEqual( 44, inter.Sum( 20, 25 ) );
			Assert.AreEqual( true, inter.Valid );
		}

		[Test]
		public void ProxyForClassWithInterfaces()
		{
			object proxy = _generator.CreateClassProxy( typeof(ServiceClass), new Type[] { typeof(IDisposable) }, 
				new ResultModifiedInvocationHandler( ) );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(ServiceClass).IsAssignableFrom( proxy.GetType() ) );
			Assert.IsTrue( typeof(IDisposable).IsAssignableFrom( proxy.GetType() ) );

			ServiceClass inter = (ServiceClass) proxy;

			Assert.AreEqual( 44, inter.Sum( 20, 25 ) );
			Assert.AreEqual( true, inter.Valid );
		}

		[Test]
		public void ProxyForClassWithSuperClass()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(SpecializedServiceClass), new ResultModifiedInvocationHandler( ) );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(ServiceClass).IsAssignableFrom( proxy.GetType() ) );
			Assert.IsTrue( typeof(SpecializedServiceClass).IsAssignableFrom( proxy.GetType() ) );

			SpecializedServiceClass inter = (SpecializedServiceClass) proxy;

			Assert.AreEqual( 44, inter.Sum( 20, 25 ) );
			Assert.AreEqual( -6, inter.Subtract( 20, 25 ) );
			Assert.AreEqual( true, inter.Valid );
		}

		[Test]
		public void ProxyForClassWhichImplementsInterfaces()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(MyInterfaceImpl), new ResultModifiedInvocationHandler( ) );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(MyInterfaceImpl).IsAssignableFrom( proxy.GetType() ) );
			Assert.IsTrue( typeof(IMyInterface).IsAssignableFrom( proxy.GetType() ) );

			IMyInterface inter = (IMyInterface) proxy;

			Assert.AreEqual( 44, inter.Calc( 20, 25 ) );
		}

		[Test]
		public void ProxyingClassWithoutVirtualMethods()
		{
			NoVirtualMethodClass proxy = (NoVirtualMethodClass) _generator.CreateClassProxy( 
				typeof(NoVirtualMethodClass), new StandardInterceptor( ) );
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void ProxyingClassWithSealedMethods()
		{
			SealedMethodsClass proxy = (SealedMethodsClass) _generator.CreateClassProxy( 
				typeof(SealedMethodsClass), new StandardInterceptor() );
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void HashtableProxy()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(Hashtable), new HashtableInterceptor() );

			Assert.IsTrue( typeof(Hashtable).IsAssignableFrom( proxy.GetType() ) );

			object value = (proxy as Hashtable)["key"];

			Assert.IsTrue(value is String);
			Assert.AreEqual("default", value.ToString());
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void CreateClassProxyInvalidBaseClass()
		{
			_generator.CreateClassProxy( 
					typeof(ICloneable), new StandardInterceptor( ) );
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CreateClassProxyNullBaseClass()
		{
			_generator.CreateClassProxy( 
				null, new StandardInterceptor( ) );
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void CreateClassProxyNullInterceptor()
		{
			_generator.CreateClassProxy( 
				typeof(SpecializedServiceClass), null );
		}

		[Test]
		public void TestGenerationSimpleInterface()
		{
			object proxy = _generator.CreateProxy( 
				typeof(IMyInterface), new ResultModifiedInvocationHandler(), new MyInterfaceImpl() );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(IMyInterface).IsAssignableFrom( proxy.GetType() ) );

			IMyInterface inter = (IMyInterface) proxy;

			Assert.AreEqual( 44, inter.Calc( 20, 25 ) );

			inter.Name = "opa";
			Assert.AreEqual( "opa", inter.Name );

			inter.Started = true;
			Assert.AreEqual( true, inter.Started );
		}

		[Test]
		public void UsingCache()
		{
			object proxy = _generator.CreateProxy( 
				typeof(IMyInterface), new StandardInterceptor(), new MyInterfaceImpl() );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(IMyInterface).IsAssignableFrom( proxy.GetType() ) );

			proxy = _generator.CreateProxy( 
				typeof(IMyInterface), new StandardInterceptor(), new MyInterfaceImpl() );
		}

		[Test]
		public void TestGenerationWithInterfaceHeritage()
		{
			object proxy = _generator.CreateProxy( 
				typeof(IMySecondInterface), new StandardInterceptor( ), new MySecondInterfaceImpl() );

			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(IMyInterface).IsAssignableFrom( proxy.GetType() ) );
			Assert.IsTrue( typeof(IMySecondInterface).IsAssignableFrom( proxy.GetType() ) );

			IMySecondInterface inter = (IMySecondInterface) proxy;
			inter.Calc(1, 1);

			inter.Name = "hammett";
			Assert.AreEqual( "hammett", inter.Name );

			inter.Address = "pereira leite, 44";
			Assert.AreEqual( "pereira leite, 44", inter.Address );
			
			Assert.AreEqual( 45, inter.Calc( 20, 25 ) );
		}

		[Test]
		public void ClassWithConstructors()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ClassWithConstructors), 
				new StandardInterceptor(), 
				new ArrayList() );

			Assert.IsNotNull( proxy );

			ClassWithConstructors objProxy = (ClassWithConstructors) proxy;
			
			Assert.IsNotNull( objProxy.List );
			Assert.IsNull( objProxy.Dictionary );

			proxy = _generator.CreateClassProxy( 
				typeof(ClassWithConstructors), 
				new StandardInterceptor(), 
				new ArrayList(), new Hashtable() );

			Assert.IsNotNull( proxy );
			objProxy = (ClassWithConstructors) proxy;
			
			Assert.IsNotNull( objProxy.List );
			Assert.IsNotNull( objProxy.Dictionary );
		}

		[Test]
		public void TestEnumProperties()
		{
			ServiceStatusImpl service = new ServiceStatusImpl();

			object proxy = _generator.CreateProxy( 
				typeof(IServiceStatus), new StandardInterceptor( ), service );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(IServiceStatus).IsAssignableFrom( proxy.GetType() ) );

			IServiceStatus inter = (IServiceStatus) proxy;
			Assert.AreEqual( State.Invalid, inter.ActualState );
			
			inter.ChangeState( State.Valid );
			Assert.AreEqual( State.Valid, inter.ActualState );
		}

		[Test]
		public void TestAttributesForInterfaceProxies()
		{
			ServiceStatusImpl service = new ServiceStatusImpl();

			object proxy = _generator.CreateProxy( 
				typeof(IServiceStatus), new MyInterfaceProxy( ), service );
			
			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(IServiceStatus).IsAssignableFrom( proxy.GetType() ) );

			IServiceStatus inter = (IServiceStatus) proxy;

			Assert.AreEqual( State.Invalid, inter.ActualState );
			
			inter.ChangeState( State.Valid );
			Assert.AreEqual( State.Valid, inter.ActualState );
		}

		[Test]
		public void ProxyForClassWithGuidProperty() 
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ClassWithGuid), new StandardInterceptor() );

			Assert.IsNotNull( proxy );
			Assert.IsTrue( typeof(ClassWithGuid).IsAssignableFrom( proxy.GetType() ) );

			ClassWithGuid inter = (ClassWithGuid) proxy;

			Assert.IsNotNull( inter.GooId );
		}

		[Test]
		public void ProxyingClassWithSByteEnum()
		{
			ClassWithSByteEnum proxy = (ClassWithSByteEnum)
				_generator.CreateClassProxy(
					typeof(ClassWithSByteEnum), new StandardInterceptor() );
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void ProxyForMarshalByRefClass()
		{
			ClassMarshalByRef proxy = (ClassMarshalByRef) 
				_generator.CreateClassProxy(
				typeof(ClassMarshalByRef), new StandardInterceptor());

			Assert.IsNotNull(proxy);

			object o = new object();
			Assert.AreEqual(o, proxy.Ping(o));

			int i = 10;
			Assert.AreEqual(i, proxy.Pong(i));
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithPrimitiveTypeParams()
		{
			LogInvokeInterceptor interceptor = new LogInvokeInterceptor();

			RefAndOutClass proxy = (RefAndOutClass) 
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			int int1 = -3;
			proxy.RefInt(ref int1);
			Assert.AreEqual(-2, int1);

			int int2;
			proxy.OutInt(out int2);
			Assert.AreEqual(2, int2);

			Assert.AreEqual("RefInt OutInt ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithReferenceTypeParams()
		{
			LogInvokeInterceptor interceptor = new LogInvokeInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			string string1 = "foobar";
			proxy.RefString(ref string1);
			Assert.AreEqual("foobar_string", string1);

			string string2;
			proxy.OutString(out string2);
			Assert.AreEqual("string", string2);

			Assert.AreEqual("RefString OutString ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithStructTypeParams()
		{
			LogInvokeInterceptor interceptor = new LogInvokeInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			DateTime dt1 = new DateTime(1999, 1, 1);
			proxy.RefDateTime(ref dt1);
			Assert.AreEqual(new DateTime(2000, 1, 1), dt1);

			DateTime dt2;
			proxy.OutDateTime(out dt2);
			Assert.AreEqual(new DateTime(2005, 1, 1), dt2);

			Assert.AreEqual("RefDateTime OutDateTime ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithEnumTypeParams()
		{
			LogInvokeInterceptor interceptor = new LogInvokeInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			SByteEnum value1 = SByteEnum.One;
			proxy.RefSByteEnum(ref value1);
			Assert.AreEqual(SByteEnum.Two, value1);

			SByteEnum value2;
			proxy.OutSByteEnum(out value2);
			Assert.AreEqual(SByteEnum.Two, value2);

			Assert.AreEqual("RefSByteEnum OutSByteEnum ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithPrimitiveTypeParamsWhereInterceptorModifiesTheValues()
		{
			RefAndOutInterceptor interceptor = new RefAndOutInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			int arg1 = -3;
			proxy.RefInt(ref arg1);
			Assert.AreEqual(98, arg1);

			int arg2;
			proxy.OutInt(out arg2);
			Assert.AreEqual(102, arg2);

			Assert.AreEqual("RefInt OutInt ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithReferenceTypeParamsWhereInterceptorModifiesTheValues()
		{
			RefAndOutInterceptor interceptor = new RefAndOutInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			string string1 = "foobar";
			proxy.RefString(ref string1);
			Assert.AreEqual("foobar_string_xxx", string1);

			string string2;
			proxy.OutString(out string2);
			Assert.AreEqual("string_xxx", string2);

			Assert.AreEqual("RefString OutString ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithStructTypeParamsWhereInterceptorModifiesTheValues()
		{
			RefAndOutInterceptor interceptor = new RefAndOutInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			DateTime dt1 = new DateTime(1999, 1, 1);
			proxy.RefDateTime(ref dt1);
			Assert.AreEqual(new DateTime(2000, 2, 1), dt1);

			DateTime dt2;
			proxy.OutDateTime(out dt2);
			Assert.AreEqual(new DateTime(2005, 2, 1), dt2);

			Assert.AreEqual("RefDateTime OutDateTime ", interceptor.LogContents);
		}

		[Test]
		[Category("DotNetOnly")]
		public void ProxyForRefAndOutClassWithEnumTypeParamsWhereInterceptorModifiesTheValues()
		{
			RefAndOutInterceptor interceptor = new RefAndOutInterceptor();

			RefAndOutClass proxy = (RefAndOutClass)
				_generator.CreateClassProxy(
				typeof(RefAndOutClass), interceptor);

			Assert.IsNotNull(proxy);

			SByteEnum value1 = SByteEnum.One;
			proxy.RefSByteEnum(ref value1);
			Assert.AreEqual(SByteEnum.One, value1);

			SByteEnum value2;
			proxy.OutSByteEnum(out value2);
			Assert.AreEqual(SByteEnum.One, value2);

			Assert.AreEqual("RefSByteEnum OutSByteEnum ", interceptor.LogContents);
		}

		[Test]
		public void ProtectedProperties()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ClassWithProtectedMethods), new StandardInterceptor() );

			Assert.IsTrue( proxy is ClassWithProtectedMethods );
		}

		[Test]
		public void Indexer()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ClassWithIndexer), new StandardInterceptor() );

			Assert.IsTrue( proxy is ClassWithIndexer );
		}

		[Test]
		public void Indexer2()
		{
			IndexerInterface proxy = (IndexerInterface) 
				_generator.CreateProxy( typeof(IndexerInterface), 
					new StandardInterceptor(), new IndexerClass() );

			Assert.IsNotNull( proxy );

			string dummy = proxy["1"];
			dummy = proxy[1];
		}

		[Test]
		public void ReflectionTest()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(MySerializableClass), new StandardInterceptor() );

			Type type = proxy.GetType();
			
			Assert.IsNotNull(type);

			PropertyInfo info = type.GetProperty("Current", BindingFlags.DeclaredOnly|BindingFlags.Public|BindingFlags.Instance);

			Assert.IsNotNull(info);
		}

		[Test]
		public void IDataReaderProxyGeneration()
		{
			IDataReader reader = IDataReaderProxy.NewInstance( new DummyReader() );
		}
	}
}
