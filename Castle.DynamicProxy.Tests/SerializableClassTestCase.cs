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

#if !MONO && !SILVERLIGHT
namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text.RegularExpressions;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Serialization;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.BugsReported;
	using Castle.DynamicProxy.Tests.InterClasses;
	using NUnit.Framework;

	[TestFixture]
	public class SerializableClassTestCase : BasePEVerifyTestCase
	{
		public override void Init()
		{
			base.Init();
			ProxyObjectReference.ResetScope();
		}

		public override void TearDown()
		{
			base.TearDown();
			ProxyObjectReference.ResetScope();
		}

		[Test]
		public void CreateSerializable()
		{
			MySerializableClass proxy = (MySerializableClass)
			                            generator.CreateClassProxy(typeof (MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy.GetType().IsSerializable);
		}

		[Test]
		public void ImplementsISerializable()
		{
			MySerializableClass proxy = (MySerializableClass)
			                            generator.CreateClassProxy(typeof (MySerializableClass), new StandardInterceptor());

			Assert.IsTrue(proxy is ISerializable);
		}

		[Test]
		public void SimpleProxySerialization()
		{
			MySerializableClass proxy = (MySerializableClass)
			                            generator.CreateClassProxy(typeof (MySerializableClass), new StandardInterceptor());

			DateTime current = proxy.Current;

			MySerializableClass otherProxy = (MySerializableClass) SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void SerializationDelegate()
		{
			MySerializableClass2 proxy = (MySerializableClass2)
			                             generator.CreateClassProxy(typeof (MySerializableClass2), new StandardInterceptor());

			DateTime current = proxy.Current;

			MySerializableClass2 otherProxy = SerializeAndDeserialize(proxy);

			Assert.AreEqual(current, otherProxy.Current);
		}

		[Test]
		public void SimpleInterfaceProxy()
		{
			object proxy =
				generator.CreateInterfaceProxyWithTarget(typeof (IMyInterface2), new MyInterfaceImpl(), new StandardInterceptor());

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
			object proxy =
				generator.CreateInterfaceProxyWithoutTarget(typeof (IMyInterface2), new Type[] {typeof (IMyInterface)},
				                                            new StandardInterceptor());

			Assert.IsTrue(proxy is IMyInterface2);
			Assert.IsTrue(proxy is IMyInterface);


			object otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMyInterface2);
			Assert.IsTrue(otherProxy is IMyInterface);
		}

		[Test]
		public void CustomMarkerInterface()
		{
			object proxy = generator.CreateClassProxy(typeof (ClassWithMarkerInterface),
			                                          new Type[] {typeof (IMarkerInterface)},
			                                          new StandardInterceptor());

			Assert.IsNotNull(proxy);
			Assert.IsTrue(proxy is IMarkerInterface);

			object otherProxy = SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy is IMarkerInterface);
		}

		[Test]
		public void HashtableSerialization()
		{
			object proxy = generator.CreateClassProxy(
				typeof (Hashtable), new StandardInterceptor());

			Assert.IsTrue(typeof (Hashtable).IsAssignableFrom(proxy.GetType()));

			(proxy as Hashtable).Add("key", "helloooo!");

			Hashtable otherProxy = (Hashtable) SerializeAndDeserialize(proxy);

			Assert.IsTrue(otherProxy.ContainsKey("key"));
			Assert.AreEqual("helloooo!", otherProxy["key"]);
		}

		public static T SerializeAndDeserialize<T>(T proxy)
		{
			using(var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, proxy);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

		[Serializable]
		public class C
		{
			public int I;
			public C This;

			public C(int i)
			{
				I = i;
				This = this;
			}
		}

		[Test]
		public void SerializatingObjectsWithoutDefaultConstructor()
		{
			C proxy = (C) generator.CreateClassProxy(typeof(C), new object[] {1}, new StandardInterceptor());
			C otherProxy = (C) SerializeAndDeserialize(proxy);

			Assert.AreEqual(proxy.I, otherProxy.I);
			Assert.AreSame(otherProxy, otherProxy.This);
		}

		[Serializable]
		public class EventHandlerClass
		{
			public void TestHandler(object sender, EventArgs e)
			{
			}
		}

		[Serializable]
		public class DelegateHolder
		{
			public EventHandler DelegateMember;
			public ArrayList ComplexTypeMember;

			public DelegateHolder()
			{
			}

			public void TestHandler(object sender, EventArgs e)
			{
			}
		}

		[Serializable]
		public class IndirectDelegateHolder
		{
			public DelegateHolder DelegateHolder = new DelegateHolder();

			public void TestHandler(object sender, EventArgs e)
			{
			}
		}

		[Test]
		public void SerializeObjectsWithDelegateToOtherObject()
		{
			EventHandlerClass eventHandlerInstance = new EventHandlerClass();
			DelegateHolder proxy =
				(DelegateHolder) generator.CreateClassProxy(typeof (DelegateHolder), new IInterceptor[] {new StandardInterceptor()});

			proxy.DelegateMember = new EventHandler(eventHandlerInstance.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new int[] {1, 2, 3});
			proxy.ComplexTypeMember.Add(eventHandlerInstance);

			Assert.IsNotNull(proxy.DelegateMember);
			Assert.IsNotNull(proxy.DelegateMember.Target);

			Assert.IsNotNull(proxy.ComplexTypeMember);
			Assert.AreEqual(4, proxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.ComplexTypeMember[2]);
			Assert.AreSame(proxy.ComplexTypeMember[3], proxy.DelegateMember.Target);

			DelegateHolder otherProxy = (DelegateHolder) (SerializeAndDeserialize(proxy));

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
			DelegateHolder proxy =
				(DelegateHolder) generator.CreateClassProxy(typeof (DelegateHolder), new IInterceptor[] {new StandardInterceptor()});

			proxy.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.ComplexTypeMember = new ArrayList(new int[] {1, 2, 3});

			Assert.IsNotNull(proxy.DelegateMember);
			Assert.AreSame(proxy, proxy.DelegateMember.Target);

			Assert.IsNotNull(proxy.ComplexTypeMember);
			Assert.AreEqual(3, proxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.ComplexTypeMember[2]);

			DelegateHolder otherProxy = (DelegateHolder) (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateMember);
			Assert.AreSame(otherProxy, otherProxy.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.ComplexTypeMember[2]);
		}

		[Test]
		public void SerializeObjectsWithIndirectDelegateToThisObject()
		{
			IndirectDelegateHolder proxy = (IndirectDelegateHolder) generator.CreateClassProxy(typeof (IndirectDelegateHolder),
			                                                                                   new IInterceptor[]
			                                                                                   	{new StandardInterceptor()});

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new int[] {1, 2, 3});

			Assert.IsNotNull(proxy.DelegateHolder.DelegateMember);
			Assert.AreSame(proxy, proxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			IndirectDelegateHolder otherProxy = (IndirectDelegateHolder) (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.AreSame(otherProxy, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Test]
		public void SerializeObjectsWithIndirectDelegateToMember()
		{
			IndirectDelegateHolder proxy = (IndirectDelegateHolder) generator.CreateClassProxy(typeof (IndirectDelegateHolder),
			                                                                                   new IInterceptor[]
			                                                                                   	{new StandardInterceptor()});

			proxy.DelegateHolder.DelegateMember = new EventHandler(proxy.DelegateHolder.TestHandler);
			proxy.DelegateHolder.ComplexTypeMember = new ArrayList(new int[] {1, 2, 3});

			Assert.IsNotNull(proxy.DelegateHolder.DelegateMember);
			Assert.AreSame(proxy.DelegateHolder, proxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(proxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, proxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, proxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, proxy.DelegateHolder.ComplexTypeMember[2]);

			IndirectDelegateHolder otherProxy = (IndirectDelegateHolder) (SerializeAndDeserialize(proxy));

			Assert.IsNotNull(otherProxy.DelegateHolder.DelegateMember);
			Assert.AreSame(otherProxy.DelegateHolder, otherProxy.DelegateHolder.DelegateMember.Target);

			Assert.IsNotNull(otherProxy.DelegateHolder.ComplexTypeMember);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember.Count);
			Assert.AreEqual(1, otherProxy.DelegateHolder.ComplexTypeMember[0]);
			Assert.AreEqual(2, otherProxy.DelegateHolder.ComplexTypeMember[1]);
			Assert.AreEqual(3, otherProxy.DelegateHolder.ComplexTypeMember[2]);
		}

		[Serializable]
		public class ClassWithIndirectSelfReference
		{
			public ArrayList List = new ArrayList();

			public ClassWithIndirectSelfReference()
			{
				List.Add(this);
			}
		}

		[Test]
		public void SerializeClassWithIndirectSelfReference()
		{
			ClassWithIndirectSelfReference proxy =
				(ClassWithIndirectSelfReference) generator.CreateClassProxy(typeof (ClassWithIndirectSelfReference),
				                                                            new Type[0], new StandardInterceptor());
			Assert.AreSame(proxy, proxy.List[0]);

			ClassWithIndirectSelfReference otherProxy = (ClassWithIndirectSelfReference) SerializeAndDeserialize(proxy);
			Assert.AreSame(otherProxy, otherProxy.List[0]);
		}

		[Serializable]
		public class ClassWithDirectAndIndirectSelfReference
		{
			public ClassWithDirectAndIndirectSelfReference This;
			public ArrayList List = new ArrayList();

			public ClassWithDirectAndIndirectSelfReference()
			{
				This = this;
				List.Add(this);
			}
		}

		[Test]
		public void SerializeClassWithDirectAndIndirectSelfReference()
		{
			ClassWithDirectAndIndirectSelfReference proxy =
				(ClassWithDirectAndIndirectSelfReference)
				generator.CreateClassProxy(typeof (ClassWithDirectAndIndirectSelfReference),
				                           new Type[0], new StandardInterceptor());
			Assert.AreSame(proxy, proxy.This);

			ClassWithDirectAndIndirectSelfReference otherProxy =
				(ClassWithDirectAndIndirectSelfReference) SerializeAndDeserialize(proxy);
			Assert.AreSame(otherProxy, otherProxy.List[0]);
			Assert.AreSame(otherProxy, otherProxy.This);
		}

		[Test]
		public void ProxyKnowsItsGenerationOptions()
		{
			MethodFilterHook hook = new MethodFilterHook(".*");
			ProxyGenerationOptions options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());

			object proxy = generator.CreateClassProxy(
				typeof (MySerializableClass),
				new Type[0],
				options,
				new StandardInterceptor());

			FieldInfo field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.IsNotNull(field);
			Assert.AreSame(options, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IService), new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(ProxyGenerationOptions.Default, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithTarget(typeof (IService), new ServiceImpl(), options,
			                                                 new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(options, field.GetValue(proxy));

			base.Init();

			proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof (IService), new ServiceImpl(),
			                                                          new StandardInterceptor());
			field = proxy.GetType().GetField("proxyGenerationOptions");
			Assert.AreSame(ProxyGenerationOptions.Default, field.GetValue(proxy));
		}

		[Test]
		public void BaseTypeForInterfaceProxy_is_honored_after_deserialization()
		{

			var options = new ProxyGenerationOptions
			              	{
			              		BaseTypeForInterfaceProxy = typeof (SimpleClass)
			              	};
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IService), Type.EmptyTypes, options);
			var newProxy = SerializeAndDeserialize(proxy);
			Assert.AreEqual(typeof (SimpleClass), newProxy.GetType().BaseType);
		}

		[Serializable]
		private class MethodFilterHook : IProxyGenerationHook
		{
			private string nameFilter;

			public MethodFilterHook(string nameFilter)
			{
				this.nameFilter = nameFilter;
			}

			public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
			{
				return Regex.IsMatch(memberInfo.Name, nameFilter);
			}

			public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
			{
			}

			public void MethodsInspected()
			{
			}
		}

		public interface IMixedInterface
		{
			object GetExecutingObject();
		}

		[Serializable]
		public class SerializableMixin : IMixedInterface
		{
			public object GetExecutingObject()
			{
				return this;
			}
		}

		[Serializable]
		public class SerializableInterceptorSelector : IInterceptorSelector
		{
			public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
			{
				return interceptors;
			}
		}

		[Test]
		public void ProxyGenerationOptionsRespectedOnDeserialization()
		{
			MethodFilterHook hook = new MethodFilterHook("(get_Current)|(GetExecutingObject)");
			ProxyGenerationOptions options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());
			options.Selector = new SerializableInterceptorSelector();

			MySerializableClass proxy = (MySerializableClass) generator.CreateClassProxy(
			                                                  	typeof (MySerializableClass),
			                                                  	new Type[0],
			                                                  	options,
			                                                  	new StandardInterceptor());

			Assert.AreEqual(proxy.GetType(), proxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(proxy.GetType(), proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(proxy.GetType().BaseType, proxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			ProxyGenerationOptions options2 =
				(ProxyGenerationOptions) proxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);

			MySerializableClass otherProxy = (MySerializableClass) SerializeAndDeserialize(proxy);
			Assert.AreEqual(otherProxy.GetType(), otherProxy.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(otherProxy.GetType(), otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(otherProxy.GetType().BaseType,
			                otherProxy.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions) otherProxy.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);
		}

		[Test]
		public void MixinsAppliedOnDeserialization()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			MySerializableClass proxy = (MySerializableClass) generator.CreateClassProxy(
			                                                  	typeof (MySerializableClass),
			                                                  	new Type[0],
			                                                  	options,
			                                                  	new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);

			MySerializableClass otherProxy = (MySerializableClass) SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_ClassProxy()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			MySerializableClass proxy = (MySerializableClass) generator.CreateClassProxy(
			                                                  	typeof (MySerializableClass),
			                                                  	new Type[0],
			                                                  	options,
			                                                  	new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) proxy).GetExecutingObject() is SerializableMixin);

			MySerializableClass otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTarget()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			IService proxy = (IService) generator.CreateInterfaceProxyWithTarget(
			                            	typeof (IService),
			                            	new ServiceImpl(),
			                            	options,
			                            	new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)proxy).GetExecutingObject() is SerializableMixin);

			IService otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface)otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface)otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithTargetInterface()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			IService proxy = (IService) generator.CreateInterfaceProxyWithTargetInterface(
			                            	typeof (IService),
			                            	new ServiceImpl(),
			                            	options,
			                            	new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) proxy).GetExecutingObject() is SerializableMixin);

			IService otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Test]
		public void MixinFieldsSetOnDeserialization_InterfaceProxy_WithoutTarget()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SerializableMixin());

			IService proxy = (IService) generator.CreateInterfaceProxyWithoutTarget(
			                            	typeof (IService),
			                            	new Type[0],
			                            	options,
			                            	new StandardInterceptor());

			Assert.IsTrue(proxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) proxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) proxy).GetExecutingObject() is SerializableMixin);

			IService otherProxy = SerializeAndDeserialize(proxy);
			Assert.IsTrue(otherProxy is IMixedInterface);
			Assert.IsNotNull(((IMixedInterface) otherProxy).GetExecutingObject());
			Assert.IsTrue(((IMixedInterface) otherProxy).GetExecutingObject() is SerializableMixin);
		}

		[Serializable]
		private class ComplexHolder
		{
			public Type Type;
			public object Element;
		}

		// With naive serialization of ProxyGenerationOptions, the following test case fails due to problems with the order of deserialization:
		// in ProxyObjectReference, the deserialized ProxyGenerationOptions will only contain null and default values. ProxyGenerationOptions must
		// avoid serializing Type objects in order for this test case to pass.
		[Test]
		public void ProxyGenerationOptionsRespectedOnDeserializationComplex()
		{
			MethodFilterHook hook = new MethodFilterHook("(get_Current)|(GetExecutingObject)");
			ProxyGenerationOptions options = new ProxyGenerationOptions(hook);
			options.AddMixinInstance(new SerializableMixin());
			options.Selector = new SerializableInterceptorSelector();

			ComplexHolder holder = new ComplexHolder();
			holder.Type = typeof (MySerializableClass);
			holder.Element = generator.CreateClassProxy(typeof (MySerializableClass), new Type[0], options,
			                                            new StandardInterceptor());

			// check holder elements
			Assert.AreEqual(typeof (MySerializableClass), holder.Type);
			Assert.IsNotNull(holder.Element);
			Assert.IsTrue(holder.Element is MySerializableClass);
			Assert.AreNotEqual(typeof (MySerializableClass), holder.Element.GetType());

			// check whether options were applied correctly
			Assert.AreEqual(holder.Element.GetType(), holder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(holder.Element.GetType(),
			                   holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(holder.Element.GetType().BaseType,
			                holder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			ProxyGenerationOptions options2 =
				(ProxyGenerationOptions) holder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);

			ComplexHolder otherHolder = (ComplexHolder) SerializeAndDeserialize(holder);

			// check holder elements
			Assert.AreEqual(typeof (MySerializableClass), otherHolder.Type);
			Assert.IsNotNull(otherHolder.Element);
			Assert.IsTrue(otherHolder.Element is MySerializableClass);
			Assert.AreNotEqual(typeof (MySerializableClass), otherHolder.Element.GetType());

			// check whether options were applied correctly
			Assert.AreEqual(otherHolder.Element.GetType(), otherHolder.Element.GetType().GetMethod("get_Current").DeclaringType);
			Assert.AreNotEqual(otherHolder.Element.GetType(),
			                   otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			Assert.AreEqual(otherHolder.Element.GetType().BaseType,
			                otherHolder.Element.GetType().GetMethod("CalculateSumDistanceNow").DeclaringType);
			options2 = (ProxyGenerationOptions) otherHolder.Element.GetType().GetField("proxyGenerationOptions").GetValue(null);
			Assert.IsNotNull(Array.Find(options2.MixinsAsArray(), delegate(object o) { return o is SerializableMixin; }));
			Assert.IsNotNull(options2.Selector);
		}

		[Test]
		public void ReusingModuleScopeFromProxyObjectReference()
		{
			ProxyGenerator generatorWithSpecificModuleScope =
				new ProxyGenerator(new DefaultProxyBuilder(ProxyObjectReference.ModuleScope));
			Assert.AreSame(generatorWithSpecificModuleScope.ProxyBuilder.ModuleScope, ProxyObjectReference.ModuleScope);
			MySerializableClass first =
				generatorWithSpecificModuleScope.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			MySerializableClass second = SerializeAndDeserialize(first);
			Assert.AreSame(first.GetType(), second.GetType());
		}

		[Test]
		public void DeserializationWithSpecificModuleScope()
		{
			ProxyObjectReference.SetScope(generator.ProxyBuilder.ModuleScope);
			MySerializableClass first = generator.CreateClassProxy<MySerializableClass>(new StandardInterceptor());
			MySerializableClass second = SerializeAndDeserialize(first);
			Assert.AreSame(first.GetType(), second.GetType());
		}
	}
}
#endif
