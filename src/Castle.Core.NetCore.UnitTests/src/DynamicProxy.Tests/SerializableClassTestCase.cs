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
#if !MONO && !SILVERLIGHT && !NETCORE&& !NETCORE
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

	using Xunit;

	public class SerializableClassTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void BaseTypeForInterfaceProxy_is_honored_after_deserialization()
		{
			var options = new ProxyGenerationOptions
			{
				BaseTypeForInterfaceProxy = typeof(SimpleClass)
			};
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), Type.EmptyTypes, options);
			var newProxy = SerializeAndDeserialize(proxy);
			Assert.Equal(typeof(SimpleClass), newProxy.GetType().BaseType);
		}

		[Fact]
		//(Description = "DYNPROXY-133")]
		public void Can_proxy_class_with_explicit_GetObjectData()
		{
			generator.CreateClassProxy<SerializableExplicitImpl>();
		}

		[Fact]
		public void ClassProxyWithTargetSerialization()
		{
			var proxy = generator.CreateClassProxyWithTarget(new MySerializableClass(), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.Equal(current, otherProxy.Current);
		}

		[Fact]
		public void CreateSerializable()
		{
			var proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.True(proxy.GetType().IsSerializable);
		}

		[Fact]
		public void CustomMarkerInterface()
		{
			var proxy = generator.CreateClassProxy(typeof(ClassWithMarkerInterface),
				new[] { typeof(IMarkerInterface) },
				new StandardInterceptor());

			Assert.NotNull(proxy);
			Assert.True(proxy is IMarkerInterface);

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.True(otherProxy is IMarkerInterface);
		}

		[Fact]
		public void DeserializationWithSpecificModuleScope()
		{
			ProxyObjectReference.SetScope(generator.ProxyBuilder.ModuleScope);
			var first = generator.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			var second = SerializeAndDeserialize(first);
			Assert.Same(first.GetType(), second.GetType());
		}

		[Fact]
		public void HashtableSerialization()
		{
			var proxy = generator.CreateClassProxy(
				typeof(Hashtable), new StandardInterceptor());

			Assert.True(typeof(Hashtable).IsAssignableFrom(proxy.GetType()));

			(proxy as Hashtable).Add("key", "helloooo!");

			var otherProxy = (Hashtable)SerializeAndDeserialize(proxy);

			Assert.True(otherProxy.ContainsKey("key"));
			Assert.Equal("helloooo!", otherProxy["key"]);
		}

		[Fact]
		public void ImplementsISerializable()
		{
			var proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			Assert.True(proxy is ISerializable);
		}

		public SerializableClassTestCase()
		{
			ProxyObjectReference.ResetScope();
		}

		[Fact]
		public void MixinFieldsSetOnDeserialization_ClassProxy()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (MySerializableClass)generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.True(proxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.True(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.True(otherProxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.True(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Fact]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTarget()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithTarget(
				typeof(IService),
				new ServiceImpl(),
				options,
				new StandardInterceptor());

			Assert.True(proxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.True(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.True(otherProxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.True(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Fact]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTargetInterface()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithTargetInterface(
				typeof(IService),
				new ServiceImpl(),
				options,
				new StandardInterceptor());

			Assert.True(proxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.True(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.True(otherProxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.True(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Fact]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithoutTarget()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (IService)generator.CreateInterfaceProxyWithoutTarget(
				typeof(IService),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.True(proxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.True(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.True(otherProxy is IMixedInterface);
			Assert.NotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.True(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Fact]
		public void MixinsAppliedOnDeserialization()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			var proxy = (MySerializableClass)generator.CreateClassProxy(
				typeof(MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			Assert.True(proxy is IMixedInterface);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.True(otherProxy is IMixedInterface);
		}

		[Fact]
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

			Assert.Equal(proxy.GetType(), proxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.NotEqual(proxy.GetType(), proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.Equal(proxy.GetType().BaseType, proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			var options2 =
				(ProxyGenerationOptions)proxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.NotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.NotNull(options2.Selector);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.Equal(otherProxy.GetType(), otherProxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.NotEqual(otherProxy.GetType(), otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.Equal(otherProxy.GetType().BaseType,
				otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions)otherProxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.NotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.NotNull(options2.Selector);
		}

		[Fact]
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
			Assert.Equal(typeof(MySerializableClass), holder.Type);
			Assert.NotNull(holder.Element);
			Assert.True(holder.Element is MySerializableClass);
			Assert.NotEqual(typeof(MySerializableClass), holder.Element.GetType());

			// check whether options were applied correctly
			Assert.Equal(holder.Element.GetType(), holder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.NotEqual(holder.Element.GetType(),
				holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.Equal(holder.Element.GetType().BaseType,
				holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			var options2 =
				(ProxyGenerationOptions)holder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.NotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.NotNull(options2.Selector);

			var otherHolder = SerializeAndDeserialize(holder);

			// check holder elements
			Assert.Equal(typeof(MySerializableClass), otherHolder.Type);
			Assert.NotNull(otherHolder.Element);
			Assert.True(otherHolder.Element is MySerializableClass);
			Assert.NotEqual(typeof(MySerializableClass), otherHolder.Element.GetType());

			// check whether options were applied correctly
			Assert.Equal(otherHolder.Element.GetType(), otherHolder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.NotEqual(otherHolder.Element.GetType(),
				otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.Equal(otherHolder.Element.GetType().BaseType,
				otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions)otherHolder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.NotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.NotNull(options2.Selector);
		}

		//[Fact]
		// TODO: Calls base.Init which is now the constructor in xUnit
		//public void ProxyKnowsItsGenerationOptions()
		//{
		//	var hook = new MethodFilterHook(".*");
		//	var options = new ProxyGenerationOptions(hook);
		//	options.AddMixinInstance(new SerializableMixin());

		//	var proxy = generator.CreateClassProxy(
		//		typeof(MySerializableClass),
		//		new Type[0],
		//		options,
		//		new StandardInterceptor());

		//	var field = proxy.GetType().GetField("proxyGenerationOptions");
		//	Assert.NotNull(field);
		//	Assert.Same(options, field.GetValue(proxy));

		//	base.Init();

		//	proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), new StandardInterceptor());
		//	field = proxy.GetType().GetField("proxyGenerationOptions");
		//	Assert.Same(ProxyGenerationOptions.Default, field.GetValue(proxy));

		//	base.Init();

		//	proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl(), options,
		//	                                                 new StandardInterceptor());
		//	field = proxy.GetType().GetField("proxyGenerationOptions");
		//	Assert.Same(options, field.GetValue(proxy));

		//	base.Init();

		//	proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl(),
		//	                                                          new StandardInterceptor());
		//	field = proxy.GetType().GetField("proxyGenerationOptions");
		//	Assert.Same(ProxyGenerationOptions.Default, field.GetValue(proxy));
		//}

		[Fact]
		public void ReusingModuleScopeFromProxyObjectReference()
		{
			var generatorWithSpecificModuleScope =
				new ProxyGenerator(new DefaultProxyBuilder(ProxyObjectReference.ModuleScope));
			Assert.Same(generatorWithSpecificModuleScope.ProxyBuilder.ModuleScope, ProxyObjectReference.ModuleScope);
			var first =
				generatorWithSpecificModuleScope.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			var second = SerializeAndDeserialize(first);
			Assert.Same(first.GetType(), second.GetType());
		}

		[Fact]
		public void SerializatingObjectsWithoutDefaultConstructor()
		{
			var proxy = (C)generator.CreateClassProxy(typeof(C), new object[] { 1 }, new StandardInterceptor());
			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.Equal(proxy.I, otherProxy.I);
			Assert.Same(otherProxy, otherProxy.This);
		}

		[Fact]
		public void SerializationDelegate()
		{
			var proxy = (MySerializableClass2)
				generator.CreateClassProxy(typeof(MySerializableClass2), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.Equal(current, otherProxy.Current);
		}

		[Fact]
		public void SerializeClassWithDirectAndIndirectSelfReference()
		{
			var proxy =
				(ClassWithDirectAndIndirectSelfReference)
					generator.CreateClassProxy(typeof(ClassWithDirectAndIndirectSelfReference),
						new Type[0], new StandardInterceptor());
			Assert.Same(proxy, proxy.This);

			var otherProxy =
				SerializeAndDeserialize(proxy);
			Assert.Same(otherProxy, otherProxy.List[0]);
			Assert.Same(otherProxy, otherProxy.This);
		}

		[Fact]
		public void SerializeClassWithIndirectSelfReference()
		{
			var proxy =
				(ClassWithIndirectSelfReference)generator.CreateClassProxy(typeof(ClassWithIndirectSelfReference),
					new Type[0], new StandardInterceptor());
			Assert.Same(proxy, proxy.List[0]);

			var otherProxy = SerializeAndDeserialize(proxy);
			Assert.Same(otherProxy, otherProxy.List[0]);
		}

		[Fact]
		public void SerializeObjectsWithDelegateToOtherObject()
		{
			var eventHandlerInstance = new EventHandlerClass();
			var proxy =
				(DelegateHolder)generator.CreateClassProxy(typeof(DelegateHolder), new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateMember = new EventHandler(eventHandlerInstance.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });
			proxy.ComplexTypeMember.Add(eventHandlerInstance);

			Assert.NotNull(proxy.DelegateMember);
			Assert.NotNull(proxy.DelegateMember.Target);

			Assert.NotNull(proxy.ComplexTypeMember);
			Assert.Equal(4, proxy.ComplexTypeMember.Count);
			Assert.Equal(1, proxy.ComplexTypeMember[0]);
			Assert.Equal(2, proxy.ComplexTypeMember[1]);
			Assert.Equal(3, proxy.ComplexTypeMember[2]);
			Assert.Same(proxy.ComplexTypeMember[3], proxy.DelegateMember.Target);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.NotNull(otherProxy.DelegateMember);
			Assert.NotNull(otherProxy.DelegateMember.Target);

			Assert.NotNull(otherProxy.ComplexTypeMember);
			Assert.Equal(4, otherProxy.ComplexTypeMember.Count);
			Assert.Equal(1, otherProxy.ComplexTypeMember[0]);
			Assert.Equal(2, otherProxy.ComplexTypeMember[1]);
			Assert.Equal(3, otherProxy.ComplexTypeMember[2]);
			Assert.Same(otherProxy.ComplexTypeMember[3], otherProxy.DelegateMember.Target);
		}

		[Fact]
		public void SerializeObjectsWithDelegateToThisObject()
		{
			var proxy =
				(DelegateHolder)generator.CreateClassProxy(typeof(DelegateHolder), new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.NotNull(proxy.DelegateMember);
			Assert.Same(proxy, proxy.DelegateMember.Target);

			Assert.NotNull(proxy.ComplexTypeMember);
			Assert.Equal(3, proxy.ComplexTypeMember.Count);
			Assert.Equal(1, proxy.ComplexTypeMember[0]);
			Assert.Equal(2, proxy.ComplexTypeMember[1]);
			Assert.Equal(3, proxy.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.NotNull(otherProxy.DelegateMember);
			Assert.Same(otherProxy, otherProxy.DelegateMember.Target);

			Assert.NotNull(otherProxy.ComplexTypeMember);
			Assert.Equal(3, otherProxy.ComplexTypeMember.Count);
			Assert.Equal(1, otherProxy.ComplexTypeMember[0]);
			Assert.Equal(2, otherProxy.ComplexTypeMember[1]);
			Assert.Equal(3, otherProxy.ComplexTypeMember[2]);
		}

		[Fact]
		public void SerializeObjectsWithIndirectDelegateToMember()
		{
			var proxy = (IndirectDelegateHolder)generator.CreateClassProxy(typeof(IndirectDelegateHolder),
				new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.DelegateHolder.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.NotNull(proxy.DelegateHolder.DelegateMember);
			Assert.Same(proxy.DelegateHolder, proxy.DelegateHolder.DelegateMember.Target);

			Assert.NotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.Equal(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.Equal(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.Equal(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.Equal(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.NotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.Same(otherProxy.DelegateHolder, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.NotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.Equal(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.Equal(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.Equal(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.Equal(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Fact]
		public void SerializeObjectsWithIndirectDelegateToThisObject()
		{
			var proxy = (IndirectDelegateHolder)generator.CreateClassProxy(typeof(IndirectDelegateHolder),
				new IInterceptor[] { new StandardInterceptor() });

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new[] { 1, 2, 3 });

			Assert.NotNull(proxy.DelegateHolder.DelegateMember);
			Assert.Same(proxy, proxy.DelegateHolder.DelegateMember.Target);

			Assert.NotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.Equal(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.Equal(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.Equal(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.Equal(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			var otherProxy = (SerializeAndDeserialize(proxy));

			Assert.NotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.Same(otherProxy, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.NotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.Equal(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.Equal(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.Equal(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.Equal(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Fact]
		public void SimpleInterfaceProxy()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget(typeof(IMyInterface2), new MyInterfaceImpl(), new StandardInterceptor());

			Assert.True(proxy.GetType().IsSerializable);

			var inter = (IMyInterface2)proxy;

			inter.Name = "opa";
			Assert.Equal("opa", inter.Name);
			inter.Started = true;
			Assert.Equal(true, inter.Started);

			var otherProxy = (IMyInterface2)SerializeAndDeserialize(proxy);

			Assert.Equal(inter.Name, otherProxy.Name);
			Assert.Equal(inter.Started, otherProxy.Started);
		}

		[Fact]
		public void SimpleInterfaceProxy_WithoutTarget()
		{
			var proxy =
				generator.CreateInterfaceProxyWithoutTarget(typeof(IMyInterface2), new[] { typeof(IMyInterface) },
					new StandardInterceptor());

			Assert.True(proxy is IMyInterface2);
			Assert.True(proxy is IMyInterface);

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.True(otherProxy is IMyInterface2);
			Assert.True(otherProxy is IMyInterface);
		}

		[Fact]
		public void SimpleProxySerialization()
		{
			var proxy = (MySerializableClass)
				generator.CreateClassProxy(typeof(MySerializableClass), new StandardInterceptor());

			var current = proxy.Current;

			var otherProxy = SerializeAndDeserialize(proxy);

			Assert.Equal(current, otherProxy.Current);
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

#endif
}