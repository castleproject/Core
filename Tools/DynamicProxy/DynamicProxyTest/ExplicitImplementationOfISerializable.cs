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
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Serialization;
	
	using NUnit.Framework;

	[TestFixture]
	public class ExplicitImplementationOfISerializable
	{
		private GeneratorContext context;
		private ProxyGenerator generator;

		[SetUp]
		public void Setup()
		{
			context = new GeneratorContext();
			generator = new ProxyGenerator();
			ProxyObjectReference.ResetScope();
		}

		[Test]
		public void CreateClassThatImplementISerializable()
		{
			MyExplicitISerializable proxy = (MyExplicitISerializable)this.generator.CreateCustomClassProxy(typeof(MyExplicitISerializable),new StandardInterceptor(), this.context);
			Assert.IsTrue(proxy.GetType().IsSerializable);
		}

		[Test]
		public void SimpleProxySerialization()
		{
			MyExplicitISerializable proxy = (MyExplicitISerializable)
				this.generator.CreateCustomClassProxy(
					typeof(MyExplicitISerializable),new StandardInterceptor(), this.context);
			
			DateTime original = proxy.Current;
			MyExplicitISerializable serializedProxy = (MyExplicitISerializable) SerializableClassTestCase.SerializeAndDeserialize(proxy);
			Assert.AreEqual(original, serializedProxy.Current);
		}

		[Test]
		public void ProxySerializationWhileOverridingExplicitInterface()
		{
			MyExplicitISerializable2 proxy = (MyExplicitISerializable2)
				this.generator.CreateCustomClassProxy(
				typeof(MyExplicitISerializable2),new StandardInterceptor(), this.context);
			
			MyExplicitISerializable2 serializedProxy = (MyExplicitISerializable2) SerializableClassTestCase.SerializeAndDeserialize(proxy);
			Assert.AreEqual(proxy.Info, serializedProxy.Info);
		}

		[Test]
		public void ExplicitISerializeDoes_NOT_CallToBaseImplementation()
		{
			MyExplicitISerializable3 proxy = (MyExplicitISerializable3)
				this.generator.CreateCustomClassProxy(
				typeof(MyExplicitISerializable3),new StandardInterceptor(), this.context);
			
			MyExplicitISerializable3 serializedProxy = (MyExplicitISerializable3) SerializableClassTestCase.SerializeAndDeserialize(proxy);
			Assert.IsFalse( (proxy.Info+"_Serialized").Equals( serializedProxy.Info ) );
		}
	}

	[Serializable]
	public class MyExplicitISerializable
	{
		public DateTime Current;

		public MyExplicitISerializable()
		{
			Current = DateTime.Now;
		}
	}
	
	[Serializable]
	public class MyExplicitISerializable2 : MyExplicitISerializable, ISerializable
	{
		public string Info;

		public MyExplicitISerializable2()
		{
			Info = Current.ToString();
		}

		protected MyExplicitISerializable2(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			Current = serializationInfo.GetDateTime("dt");
			Info = serializationInfo.GetString("info");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dt", Current, typeof(DateTime));
			info.AddValue("info", Info, typeof(string));
		}
	}


	[Serializable]
	public class MyExplicitISerializable3 : MyExplicitISerializable, ISerializable
	{
		public string Info;

		public MyExplicitISerializable3()
		{
			Info = Current.ToString();
		}

		protected MyExplicitISerializable3(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			Current = serializationInfo.GetDateTime("dt");
			Info = serializationInfo.GetString("info");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("dt", Current, typeof(DateTime));
			info.AddValue("info", Info+"_Serialized", typeof(string));
		}
	}

}
