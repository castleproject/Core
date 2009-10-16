// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using NUnit.Framework;

	[TestFixture]
	public class ClassEmitterTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void AutomaticDefaultConstructorGeneration()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			Type t = emitter.BuildType();
			Activator.CreateInstance(t);
		}

		[Test]
		public void AutomaticDefaultConstructorGenerationWithClosedGenericType()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			Type t = emitter.BuildType();
			Activator.CreateInstance(t);
		}

		[Test]
		public void StaticMethodArguments()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod("StaticMethod", MethodAttributes.Public | MethodAttributes.Static,
			                                                   typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(methodEmitter.Arguments[0]));
			Type t = emitter.BuildType();
			Assert.AreEqual("five", t.GetMethod("StaticMethod").Invoke(null, new object[] {"five"}));
		}

		[Test]
		public void InstanceMethodArguments()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod("InstanceMethod", MethodAttributes.Public,
			                                                   typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(methodEmitter.Arguments[0]));
			Type t = emitter.BuildType();
			object instance = Activator.CreateInstance(t);
			Assert.AreEqual("six", t.GetMethod("InstanceMethod").Invoke(instance, new object[] {"six"}));
		}

		[Test]
		public void ForceUnsignedFalseWithSignedTypes()
		{
#if SILVERLIGHT
#warning Silverlight does not allow us to sign generated assemblies
			
			const bool shouldBeSigned = false;
#else
			const bool shouldBeSigned = true;
#endif
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes,
			                                        TypeAttributes.Public, false);
			Type t = emitter.BuildType();
			Assert.AreEqual(shouldBeSigned, StrongNameUtil.IsAssemblySigned(t.Assembly));
		}

		[Test]
		public void ForceUnsignedTrueWithSignedTypes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes,
			                                        TypeAttributes.Public, true);
			Type t = emitter.BuildType();
			Assert.IsFalse(StrongNameUtil.IsAssemblySigned(t.Assembly));
		}

		[Test]
		public void CreateFieldWithAttributes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			emitter.CreateField("myField", typeof (string), FieldAttributes.FamANDAssem | FieldAttributes.InitOnly);
			Type t = emitter.BuildType();
			FieldInfo field = t.GetField("myField", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.IsNotNull(field);
			Assert.AreEqual(FieldAttributes.FamANDAssem | FieldAttributes.InitOnly, field.Attributes);
		}

		[Test]
		public void CreateStaticFieldWithAttributes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			emitter.CreateStaticField("myField", typeof (string), FieldAttributes.FamANDAssem | FieldAttributes.InitOnly);
			Type t = emitter.BuildType();
			FieldInfo field = t.GetField("myField", BindingFlags.NonPublic | BindingFlags.Static);
			Assert.IsNotNull(field);
			Assert.AreEqual(FieldAttributes.Static | FieldAttributes.FamANDAssem | FieldAttributes.InitOnly, field.Attributes);
		}

		[Test]
		public void UsingClassEmitterForInterfaces()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes, 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			emitter.CreateMethod("MyMethod", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, typeof (void));
			Type t = emitter.BuildType();
			Assert.IsTrue(t.IsInterface);
			MethodInfo method = t.GetMethod("MyMethod");
			Assert.IsNotNull(method);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void NoBaseTypeForInterfaces()
		{
			DisableVerification();
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			Type t = emitter.BaseType;
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void NoDefaultCtorForInterfaces()
		{
			DisableVerification();
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			emitter.CreateDefaultConstructor();
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void NoCustomCtorForInterfaces()
		{
			DisableVerification();
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			emitter.CreateConstructor();
		}

		[Test]
		public void NestedInterface()
		{
			ClassEmitter outerEmitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IOuter", null, Type.EmptyTypes, 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			NestedClassEmitter innerEmitter = new NestedClassEmitter(outerEmitter, "IInner", 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.NestedPublic, null, Type.EmptyTypes);
			innerEmitter.CreateMethod("MyMethod", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, typeof(void));
			Type inner = innerEmitter.BuildType();
			Type outer = outerEmitter.BuildType();
			Assert.IsTrue(inner.IsInterface);
			MethodInfo method = inner.GetMethod("MyMethod");
			Assert.IsNotNull(method);
			Assert.AreSame(inner, outer.GetNestedType("IInner", BindingFlags.Public));
		}
	}
}