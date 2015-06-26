// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	using Xunit;

		public class ClassEmitterTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void AutomaticDefaultConstructorGeneration()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			Type t = emitter.BuildType();
			Activator.CreateInstance(t);
		}

		[Fact]
		public void AutomaticDefaultConstructorGenerationWithClosedGenericType()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			Type t = emitter.BuildType();
			Activator.CreateInstance(t);
		}

		[Fact]
		public void StaticMethodArguments()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod("StaticMethod", MethodAttributes.Public | MethodAttributes.Static,
			                                                   typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(methodEmitter.Arguments[0]));
			Type t = emitter.BuildType();
			Assert.Equal("five", t.GetMethod("StaticMethod").Invoke(null, new object[] {"five"}));
		}

		[Fact]
		public void InstanceMethodArguments()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>),
			                                        Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod("InstanceMethod", MethodAttributes.Public,
			                                                   typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(methodEmitter.Arguments[0]));
			Type t = emitter.BuildType();
			object instance = Activator.CreateInstance(t);
			Assert.Equal("six", t.GetMethod("InstanceMethod").Invoke(instance, new object[] {"six"}));
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void ForceUnsignedFalseWithSignedTypes()
		{
			const bool shouldBeSigned = true;
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes,
			                                        TypeAttributes.Public, false);
			Type t = emitter.BuildType();
			Assert.Equal(shouldBeSigned, StrongNameUtil.IsAssemblySigned(t.GetTypeInfo().Assembly));
		}
#endif

		[Fact]
		public void ForceUnsignedTrueWithSignedTypes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes,
			                                        TypeAttributes.Public, true);
			Type t = emitter.BuildType();
			Assert.False(StrongNameUtil.IsAssemblySigned(t.GetTypeInfo().Assembly));
		}

		[Fact]
		public void CreateFieldWithAttributes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			emitter.CreateField("myField", typeof (string), FieldAttributes.FamANDAssem | FieldAttributes.InitOnly);
			Type t = emitter.BuildType();
			FieldInfo field = t.GetField("myField", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.NotNull(field);
			Assert.Equal(FieldAttributes.FamANDAssem | FieldAttributes.InitOnly, field.Attributes);
		}

		[Fact]
		public void CreateStaticFieldWithAttributes()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			emitter.CreateStaticField("myField", typeof (string), FieldAttributes.FamANDAssem | FieldAttributes.InitOnly);
			Type t = emitter.BuildType();
			FieldInfo field = t.GetField("myField", BindingFlags.NonPublic | BindingFlags.Static);
			Assert.NotNull(field);
			Assert.Equal(FieldAttributes.Static | FieldAttributes.FamANDAssem | FieldAttributes.InitOnly, field.Attributes);
		}

		[Fact]
		public void UsingClassEmitterForInterfaces()
		{
			ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes, 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			emitter.CreateMethod("MyMethod", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
			                     typeof(void), Type.EmptyTypes);
			Type t = emitter.BuildType();
			Assert.True(t.GetTypeInfo().IsInterface);
			MethodInfo method = t.GetMethod("MyMethod");
			Assert.NotNull(method);
		}

		[Fact]
		public void NoBaseTypeForInterfaces()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				DisableVerification();
				ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
					TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);

#pragma warning disable 219
				Type t = emitter.BaseType;
#pragma warning restore 219
			});
		}

		[Fact]
		public void NoDefaultCtorForInterfaces()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				DisableVerification();
				ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
					TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
				emitter.CreateDefaultConstructor();
			});
		}

		[Fact]
		public void NoCustomCtorForInterfaces()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				DisableVerification();
				ClassEmitter emitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IFoo", null, Type.EmptyTypes,
					TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
				emitter.CreateConstructor();
			});
		}

		[Fact]
		public void NestedInterface()
		{
			ClassEmitter outerEmitter = new ClassEmitter(generator.ProxyBuilder.ModuleScope, "IOuter", null, Type.EmptyTypes, 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public, false);
			NestedClassEmitter innerEmitter = new NestedClassEmitter(outerEmitter, "IInner", 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.NestedPublic, null, Type.EmptyTypes);
			innerEmitter.CreateMethod("MyMethod", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual,
			                          typeof(void), Type.EmptyTypes);
			Type inner = innerEmitter.BuildType();
			Type outer = outerEmitter.BuildType();
			Assert.True(inner.GetTypeInfo().IsInterface);
			MethodInfo method = inner.GetMethod("MyMethod");
			Assert.NotNull(method);
			Assert.Same(inner, outer.GetNestedType("IInner", BindingFlags.Public));
		}
	}
}