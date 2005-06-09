// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;

	using NUnit.Framework;

	using Castle.DynamicProxy.Test.Classes;
	using Castle.DynamicProxy.Test.Mixins;
	using Castle.DynamicProxy.Test.ClassInterfaces;

	/// <summary>
	/// Summary description for SerializableClassTestCase.
	/// </summary>
	[TestFixture]
	public class SerializableClassTestCase
	{
		ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void CreateSerializable()
		{
			MySerializableClass myClass = new MySerializableClass();

			MySerializableClass proxy = (MySerializableClass) 
				generator.CreateClassProxy( typeof(MySerializableClass), new StandardInterceptor() );

			Assert.IsTrue( proxy.GetType().IsSerializable );
		}

		[Test]
		public void SimpleProxySerialization()
		{
			MySerializableClass proxy = (MySerializableClass) 
				generator.CreateClassProxy( typeof(MySerializableClass), new StandardInterceptor() );

			DateTime current = proxy.Current;

			MySerializableClass otherProxy = (MySerializableClass) SerializeAndDeserialize(proxy);

			Assert.AreEqual( current, otherProxy.Current );
		}

		[Test]
		public void SerializationDelegate()
		{
			MySerializableClass2 proxy = (MySerializableClass2) 
				generator.CreateClassProxy( typeof(MySerializableClass2), new StandardInterceptor() );

			DateTime current = proxy.Current;

			MySerializableClass2 otherProxy = (MySerializableClass2) SerializeAndDeserialize(proxy);

			Assert.AreEqual( current, otherProxy.Current );
		}

		[Test]
		public void SimpleInterfaceProxy()
		{
			object proxy = generator.CreateProxy( 
				typeof(IMyInterface), new StandardInterceptor( ), new MyInterfaceImpl() );

			Assert.IsTrue( proxy.GetType().IsSerializable );
			
			IMyInterface inter = (IMyInterface) proxy;

			inter.Name = "opa";
			Assert.AreEqual( "opa", inter.Name );
			inter.Started = true;
			Assert.AreEqual( true, inter.Started );

			IMyInterface otherProxy = (IMyInterface) SerializeAndDeserialize(proxy);

			Assert.AreEqual( inter.Name, otherProxy.Name );
			Assert.AreEqual( inter.Started, otherProxy.Started );
		}

		[Test]
		[Category("DotNetOnly")]
		public void MixinSerialization()
		{
			GeneratorContext context = new GeneratorContext();
			SimpleMixin mixin1 = new SimpleMixin();
			OtherMixin mixin2 = new OtherMixin();

			context.AddMixinInstance( mixin1 );
			context.AddMixinInstance( mixin2 );

			object proxy = generator.CreateCustomClassProxy( 
				typeof(SimpleClass), new StandardInterceptor(), context );

			Assert.IsTrue( typeof(SimpleClass).IsAssignableFrom( proxy.GetType() ) );

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.AreEqual(1, mixin.DoSomething());

			IOtherMixin other = proxy as IOtherMixin;
			Assert.AreEqual(3, other.Sum(1,2));

			SimpleClass otherProxy = (SimpleClass) SerializeAndDeserialize(proxy);

			mixin = otherProxy as ISimpleMixin;
			Assert.AreEqual(1, mixin.DoSomething());

			other = otherProxy as IOtherMixin;
			Assert.AreEqual(3, other.Sum(1,2));
		}

		[Test]
		[Ignore("XmlSerializer does not respect the ObjectReference protocol so it wont work")]
		public void XmlSerialization()
		{
			GeneratorContext context = new GeneratorContext();
			SimpleMixin mixin1 = new SimpleMixin();
			OtherMixin mixin2 = new OtherMixin();

			context.AddMixinInstance( mixin1 );
			context.AddMixinInstance( mixin2 );

			object proxy = generator.CreateCustomClassProxy( 
				typeof(SimpleClass), new StandardInterceptor(), context );

			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SimpleClass));
			MemoryStream stream = new MemoryStream();
			serializer.Serialize(stream, proxy);
			stream.Position = 0;
			SimpleClass otherProxy = (SimpleClass) serializer.Deserialize( stream );

			ISimpleMixin mixin = otherProxy as ISimpleMixin;
			Assert.AreEqual(1, mixin.DoSomething());

			IOtherMixin other = otherProxy as IOtherMixin;
			Assert.AreEqual(3, other.Sum(1,2));
		}

		[Test]
		public void HashtableSerialization()
		{
			object proxy = generator.CreateClassProxy( 
				typeof(Hashtable), new StandardInterceptor() );

			Assert.IsTrue( typeof(Hashtable).IsAssignableFrom( proxy.GetType() ) );

			(proxy as Hashtable).Add("key", "helloooo!");

			Hashtable otherProxy = (Hashtable) SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy.ContainsKey("key"));
			Assert.AreEqual("helloooo!", otherProxy["key"]);
		}

		public object SerializeAndDeserialize( object proxy )
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize( stream, proxy );
			stream.Position = 0;
			return formatter.Deserialize( stream );
		}
	}
}
