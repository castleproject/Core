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

using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.DynamicProxy.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class ClassEmitterTestCase : BasePEVerifyTestCase
	{
		public override void TearDown ()
		{
			generator.ProxyBuilder.ModuleScope.SaveAssembly ();
			base.TearDown ();
		}

		[Test]
		public void AutomaticDefaultConstructorGeneration ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes);
			Type t = emitter.BuildType();
			Activator.CreateInstance (t);
		}

		[Test]
		public void AutomaticDefaultConstructorGenerationWithClosedGenericType ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>), Type.EmptyTypes);
			Type t = emitter.BuildType ();
			Activator.CreateInstance (t);
		}

		[Test]
		public void StaticMethodArguments ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>), Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod ("StaticMethod", MethodAttributes.Public | MethodAttributes.Static,
					typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement (new ReturnStatement (methodEmitter.Arguments[0]));
			Type t = emitter.BuildType ();
			Assert.AreEqual ("five", t.GetMethod ("StaticMethod").Invoke (null, new object[] { "five" }));
		}

		[Test]
		public void InstanceMethodArguments ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (List<object>), Type.EmptyTypes);
			MethodEmitter methodEmitter = emitter.CreateMethod ("InstanceMethod", MethodAttributes.Public,
					typeof (string), typeof (string));
			methodEmitter.CodeBuilder.AddStatement (new ReturnStatement (methodEmitter.Arguments[0]));
			Type t = emitter.BuildType ();
			object instance = Activator.CreateInstance (t);
			Assert.AreEqual ("six", t.GetMethod ("InstanceMethod").Invoke (instance, new object[] { "six" }));
		}

		[Test]
		public void ForceUnsignedFalseWithSignedTypes ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes, TypeAttributes.Public, false);
			Type t = emitter.BuildType ();
			Assert.IsTrue (StrongNameUtil.IsAssemblySigned (t.Assembly));
		}

		[Test]
		public void ForceUnsignedTrueWithSignedTypes ()
		{
			ClassEmitter emitter = new ClassEmitter (generator.ProxyBuilder.ModuleScope, "Foo", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
			Type t = emitter.BuildType ();
			Assert.IsFalse (StrongNameUtil.IsAssemblySigned (t.Assembly));
		}
	}
}