// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

using Castle.DynamicProxy.Tests.BugsReported;
using Castle.DynamicProxy.Tests.Interceptors;

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using Castle.DynamicProxy.Serialization;
	using Castle.DynamicProxy.Generators;
	using NUnit.Framework;
	using Castle.Core.Interceptor;
	using System.Runtime.Serialization;
	using Castle.DynamicProxy.Test.Classes;
	using Castle.DynamicProxy.Tests;
	using Castle.DynamicProxy.Tests.InterClasses;

	/// <summary>
	/// Summary description for SerializableClassTestCase.
	/// </summary>
	[TestFixture]
	public class SerializableClassTestCase : BasePEVerifyTestCase
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
			ProxyObjectReference.ResetScope();

			MySerializableClass proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy.GetType().IsSerializable);
		}

		[Test]
		public void ImplementsISerializable()
		{
			ProxyObjectReference.ResetScope();

			MySerializableClass proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy is ISerializable);

		}

		[Test]
		public void SimpleProxySerialization()
		{
			ProxyObjectReference.ResetScope();

			MySerializableClass proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			DateTime current = proxy.Current;

			MySerializableClass otherProxy = (MySerializableClass)SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void SerializationDelegate()
		{
			ProxyObjectReference.ResetScope();

			MySerializableClass2 proxy = (MySerializableClass2)
				generator.CreateClassProxy(typeof(MySerializableClass2), new StandardInterceptor());

			DateTime current = proxy.Current;

			MySerializableClass2 otherProxy = (MySerializableClass2)SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void SimpleInterfaceProxy()
		{
			ProxyObjectReference.ResetScope();

			object proxy = generator.CreateInterfaceProxyWithTarget(typeof(IMyInterface2), new MyInterfaceImpl(), new StandardInterceptor());

			Assert.IsTrue(proxy.GetType().IsSerializable);

			IMyInterface2 inter = (IMyInterface2)proxy;

			inter.Name = "opa";
			Assert.AreEqual("opa", inter.Name);
			inter.Started = true;
			Assert.AreEqual(true, inter.Started);

			IMyInterface2 otherProxy = (IMyInterface2)SerializeAndDeserialize(proxy);

			Assert.AreEqual(inter.Name, otherProxy.Name);
			Assert.AreEqual(inter.Started, otherProxy.Started);
		}


		[Test]
		public void SimpleInterfaceProxy_WithoutTarget()
		{
			ProxyObjectReference.ResetScope();

			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IMyInterface2), new Type[]{typeof(IMyInterface)}, new StandardInterceptor());

			Assert.IsTrue(proxy is IMyInterface2);
			Assert.IsTrue(proxy is IMyInterface);


			object otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMyInterface2);
			Assert.IsTrue(otherProxy is IMyInterface);

		}

		[Test]
		public void CustomMarkerInterface()
		{
			ProxyObjectReference.ResetScope();

			object proxy = generator.CreateClassProxy(typeof(ClassWithMarkerInterface),
				new Type[] { typeof(IMarkerInterface) },
				new StandardInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(proxy is IMarkerInterface);

			object otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMarkerInterface);
		}


		[Test]
		public void HashtableSerialization()
		{
			ProxyObjectReference.ResetScope();

			object proxy = generator.CreateClassProxy(
				typeof(Hashtable), new StandardInterceptor());

			Assert.IsTrue(typeof(Hashtable).IsAssignableFrom(proxy.GetType()));

			(proxy as Hashtable).Add("key", "helloooo!");

			Hashtable otherProxy = (Hashtable)SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy.ContainsKey("key"));
			Assert.AreEqual("helloooo!", otherProxy["key"]);
		}

		public static object SerializeAndDeserialize(object proxy)
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, proxy);
			stream.Position = 0;
			return formatter.Deserialize(stream);
		}
	}
}
