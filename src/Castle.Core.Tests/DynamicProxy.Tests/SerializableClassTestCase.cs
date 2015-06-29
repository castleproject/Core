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

#if FEATURE_SERIALIZATION

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using Castle.DynamicProxy.Serialization;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.BugsReported;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Serialization;

	using NUnit.Framework;

	[TestFixture]
	public class SerializableClassTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void BaseTypeForInterfaceProxy_is_honored_after_deserialization()
		{
			var options = new ProxyGenerationOptions
			{
				BaseTypeForInterfaceProxy = typeof(SimpleClass)
			};
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), Type.EmptyTypes, options);
			var newProxy = SerializeAndDeserialize(proxy);
			Assert.AreEqual(typeof(SimpleClass), newProxy.GetType().BaseType);
		}

		[Test(Description = "DYNPROXY-133")]
		public void Can_proxy_class_with_explicit_GetObjectData()
		{
			generator.CreateClassProxy<SerializableExplicitImpl>();
		}

		[Test]
		public void ClassProxyWithTargetSerialization()
		{
			var proxy = generator.CreateClassProxyWithTarget(new MySerializableClass(), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void CreateSerializable()
		{
			var proxy = (MySerializableClass)
			            generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy.GetType().IsSerializable);
		}

		[Test]
		public void CustomMarkerInterface()
		{
			var proxy = generator.CreateClassProxy(typeof(ClassWithMarkerInterface),
			                                       new[] { typeof(IMarkerInterface) },
			                                       new StandardInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(proxy is IMarkerInterface);

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMarkerInterface);
		}

		[Test]
		public void DeserializationWithSpecificModuleScope()
		{
			ProxyObjectReference.SetScope(generator.ProxyBuilder.ModuleScope);
			var first = generator.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			var second = SerializeAndDeserialize(first);
			Assert.AreSame(first.GetType(), second.GetType());
		}

		[Test]
		public void HashtableSerialization()
		{
			var proxy = generator.CreateClassProxy(
				typeof(Hashtable), new StandardInterceptor());

			Assert.IsTrue(typeof(Hashtable).IsAssignableFrom(proxy.GetType()));

			(proxy as Hashtable).Add("key", "helloooo!");

			var otherProxy = (Hashtable)SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy.ContainsKey("key"));
			Assert.AreEqual("helloooo!", otherProxy["key"]);
		}

		[Test]
		public void ImplementsISerializable()
		{
			var proxy = (MySerializableClass)
			            generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy is ISerializable);
		}

		public override void Init()
		{
			base.Init();
			ProxyObjectReference.ResetScope();
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_ClassProxy()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (MySerializableClass)generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTarget()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithTarget(
				typeof(IService),
				new ServiceImpl(),
				options,
				new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTargetInterface()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithTargetInterface(
				typeof(IService),
				new ServiceImpl(),
				options,
				new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithoutTarget()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithoutTarget(
				typeof(IService),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinsAppliedOnDeserialization()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (MySerializableClass)generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
		}

		[Test]
		public void ProxyGenerationOptionsRespectedOnDeserialization()
		{
			var hook = new MethodFilterHook("(get_Current)|(GetExecutingObject)");
			var options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());
			options.Selector = new SerializableInterceptorSelector();

			var proxy = (MySerializableClass)generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.AreEqual(proxy.GetType(), proxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(proxy.GetType(), proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(proxy.GetType().BaseType, proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			var options2 =
				(ProxyGenerationOptions)proxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.AreEqual(otherProxy.GetType(), otherProxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(otherProxy.GetType(), otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(otherProxy.GetType().BaseType,
			                otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions)otherProxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);
		}

		[Test]
		public void ProxyGenerationOptionsRespectedOnDeserializationComplex()
		{
			var hook = new MethodFilterHook("(get_Current)|(GetExecutingObject)");
			var options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());
			options.Selector = new SerializableInterceptorSelector();

			var holder = new ComplexHolder();
			holder.Type = typeof(MySerializableClass);
			holder.Element = generator.CreateClassProxy(typeof(MySerializableClass), new Type[0], options,
			                                            new StandardInterceptor());

			// check holder elements
			Assert.AreEqual(typeof(MySerializableClass), holder.Type);
			Assert.IsNotNull(holder.Element);
			Assert.IsTrue(holder.Element is MySerializableClass);
			Assert.AreNotEqual(typeof(MySerializableClass), holder.Element.GetType());

			// check whether options were applied correctly
			Assert.AreEqual(holder.Element.GetType(), holder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(holder.Element.GetType(),
			                   holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(holder.Element.GetType().BaseType,
			                holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			var options2 =
				(ProxyGenerationOptions)holder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);

			var otherHolder = SerializeAndDeserialize(holder);

			// check holder elements
			Assert.AreEqual(typeof(MySerializableClass), otherHolder.Type);
			Assert.IsNotNull(otherHolder.Element);
			Assert.IsTrue(otherHolder.Element is MySerializableClass);
			Assert.AreNotEqual(typeof(MySerializableClass), otherHolder.Element.GetType());

			// check whether options were applied correctly
			Assert.AreEqual(otherHolder.Element.GetType(), otherHolder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(otherHolder.Element.GetType(),
			                   otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(otherHolder.Element.GetType().BaseType,
			                otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions)otherHolder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);
		}

		[Test]
		public void ProxyKnowsItsGenerationOptions()
		{
			var hook = new MethodFilterHook(".*");
			var options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());

			var proxy = generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			var field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.IsNotNull(field);
			Assert.AreSame(options, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(ProxyGenerationOptions.Default, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl(), options,
			                                                 new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(options, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl(),
			                                                          new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(ProxyGenerationOptions.Default, field.GetValue(proxy));
		}

		[Test]
		public void ReusingModuleScopeFromProxyObjectReference()
		{
			var generatorWithSpecificModuleScope =
				new ProxyGenerator(new DefaultProxyBuilder(ProxyObjectReference.ModuleScope));
			Assert.AreSame(generatorWithSpecificModuleScope.ProxyBuilder.ModuleScope, ProxyObjectReference.ModuleScope);
			var first =
				generatorWithSpecificModuleScope.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			var second = SerializeAndDeserialize(first);
			Assert.AreSame(first.GetType(), second.GetType());
		}

		[Test]
		public void SerializatingObjectsWithoutDefaultConstructor()
		{
			var proxy = (C)generator.CreateClassProxy(typeof(C), new object[] { 1 }, new StandardInterceptor());
			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.AreEqual(proxy.I, otherProxy.I);
			Assert.AreSame(otherProxy, otherProxy.This);
		}

		[Test]
		public void SerializationDelegate()
		{
			var proxy = (MySerializableClass2)
			            generator.CreateClassProxy(typeof(MySerializableClass2), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void SerializeClassWithDirectAndIndirectSelfReference()
		{
			var proxy =
				(ClassWithDirectAndIndirectSelfReference)
				generator.CreateClassProxy(typeof(ClassWithDirectAndIndirectSelfReference),
				                           new Type[0], new StandardInterceptor());
			Assert.AreSame(proxy, proxy.This);

			var otherProxy =
				SerializeAndDeserialize(proxy);
			Assert.AreSame(otherProxy, otherProxy.List[0]);
			Assert.AreSame(otherProxy, otherProxy.This);
		}

		[Test]
		public void SerializeClassWithIndirectSelfReference()
		{
			var proxy =
				(ClassWithIndirectSelfReference)generator.CreateClassProxy(typeof(ClassWithIndirectSelfReference),
				                                                           new Type[0], new StandardInterceptor());
			Assert.AreSame(proxy, proxy.List[0]);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.AreSame(otherProxy, otherProxy.List[0]);
		}

		[Test]
		public void SerializeObjectsWithDelegateToOtherObject()
		{
			var eventHandlerInstance = new EventHandlerClass();
			var proxy =
				(DelegateHolder)generator.CreateClassProxy(typeof(DelegateHolder), new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateMember = new EventHandler(eventHandlerInstance.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });
			proxy.ComplexTypeMember.Add(eventHandlerInstance);

			Assert.IsNotNull(proxy.DelegateMember);
			Assert.IsNotNull(proxy.DelegateMember.Target);

			Assert.IsNotNull(proxy.ComplexTypeMember);
			Assert.AreEqual(4, proxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.ComplexTypeMember[2]);
			Assert.AreSame(proxy.ComplexTypeMember[3], proxy.DelegateMember.Target);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateMember);
			Assert.IsNotNull(otherProxy.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.ComplexTypeMember);
			Assert.AreEqual(4, otherProxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.ComplexTypeMember[2]);
			Assert.AreSame(otherProxy.ComplexTypeMember[3], otherProxy.DelegateMember.Target);
		}

		[Test]
		public void SerializeObjectsWithDelegateToThisObject()
		{
			var proxy =
				(DelegateHolder)generator.CreateClassProxy(typeof(DelegateHolder), new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.IsNotNull(proxy.DelegateMember);
			Assert.AreSame(proxy, proxy.DelegateMember.Target);

			Assert.IsNotNull(proxy.ComplexTypeMember);
			Assert.AreEqual(3, proxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateMember);
			Assert.AreSame(otherProxy, otherProxy.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.ComplexTypeMember[2]);
		}

		[Test]
		public void SerializeObjectsWithIndirectDelegateToMember()
		{
			var proxy = (IndirectDelegateHolder)generator.CreateClassProxy(typeof(IndirectDelegateHolder),
			                                                               new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.DelegateHolder.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.IsNotNull(proxy.DelegateHolder.DelegateMember);
			Assert.AreSame(proxy.DelegateHolder, proxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.AreSame(otherProxy.DelegateHolder, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Test]
		public void SerializeObjectsWithIndirectDelegateToThisObject()
		{
			var proxy = (IndirectDelegateHolder)generator.CreateClassProxy(typeof(IndirectDelegateHolder),
			                                                               new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.IsNotNull(proxy.DelegateHolder.DelegateMember);
			Assert.AreSame(proxy, proxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.AreSame(otherProxy, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Test]
		public void SimpleInterfaceProxy()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget(typeof(IMyInterface2), new MyInterfaceImpl(), new StandardInterceptor());

			Assert.IsTrue(proxy.GetType().IsSerializable);

			var inter = (IMyInterface2)proxy;

			inter.Name = "opa";
			Assert.AreEqual("opa", inter.Name);
			inter.Started = true;
			Assert.AreEqual(true, inter.Started);

			var otherProxy = (IMyInterface2)SerializeAndDeserialize(proxy);

			Assert.AreEqual(inter.Name, otherProxy.Name);
			Assert.AreEqual(inter.Started, otherProxy.Started);
		}

		[Test]
		public void SimpleInterfaceProxy_WithoutTarget()
		{
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget(typeof(IMyInterface2), new[] { typeof(IMyInterface) },
				                                            new StandardInterceptor());

			Assert.IsTrue(proxy is IMyInterface2);
			Assert.IsTrue(proxy is IMyInterface);

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMyInterface2);
			Assert.IsTrue(otherProxy is IMyInterface);
		}

		[Test]
		public void SimpleProxySerialization()
		{
			var proxy = (MySerializableClass)
			            generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		public override void TearDown()
		{
			base.TearDown();
			ProxyObjectReference.ResetScope();
		}

		public static T SerializeAndDeserialize<T>(T proxy)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, proxy);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}
	}
}

#endif