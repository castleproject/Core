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
	using System;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class InterfaceProxyBaseTypeTestCase:BasePEVerifyTestCase
	{
		[Test]
		public void Sealed_class_as_base_type()
		{
			var exception = Assert.Throws(typeof(ArgumentException), () =>
			                                                         generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterface),
			                                                                                                     Type.EmptyTypes,
			                                                                                                     WithBase<Sealed>()));
			Assert.AreEqual(
				string.Format(
					"Type {0} is not valid base type for interface proxy, because it is sealed. " +
					"Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. " +
					"Please use some other valid type.",
					typeof(Sealed)), exception.Message);
		}

		[Test]
		public void Interface_as_base_type()
		{
			var exception = Assert.Throws(typeof(ArgumentException), () =>
																	 generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterface),
																												 Type.EmptyTypes,
																												 WithBase<ISomething>()));
			Assert.AreEqual(
				string.Format(
					"Type {0} is not valid base type for interface proxy, because it is not a class type. " +
					"Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. " +
					"Please use some other valid type.",
					typeof(ISomething)), exception.Message);
		}

		[Test]
		public void Class_with_no_default_ctor_as_base_type()
		{
			var exception = Assert.Throws(typeof(ArgumentException), () =>
																	 generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterface),
																												 Type.EmptyTypes,
																												 WithBase<NoDefaultCtor>()));
			Assert.AreEqual(
				string.Format(
					"Type {0} is not valid base type for interface proxy, because it does not have accessible parameterless constructor. " +
					"Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. " +
					"Please use some other valid type.",
					typeof(NoDefaultCtor)), exception.Message);
		}

		[Test]
		public void Class_with_private_default_ctor_as_base_type()
		{
			var exception = Assert.Throws(typeof(ArgumentException), () =>
																	 generator.CreateInterfaceProxyWithoutTarget(typeof(ISimpleInterface),
																												 Type.EmptyTypes,
																												 WithBase<DefaultPrivateCtor>()));
			Assert.AreEqual(
				string.Format(
					"Type {0} is not valid base type for interface proxy, because it does not have accessible parameterless constructor. " +
					"Only a non-sealed class with non-private default constructor can be used as base type for interface proxy. " +
					"Please use some other valid type.",
					typeof(DefaultPrivateCtor)), exception.Message);
		}

		[Test]
		public void Same_Class_as_base_and_target_works_fine()
		{
			var @interface = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface),
			                                                          new ClassWithInterface(),
			                                                          WithBase<ClassWithInterface>()) as ISimpleInterface;
			@interface.Do();
		}
		[Test]
		public void Class_with_protected_default_ctor_as_base_type_is_fine()
		{
			var @interface = generator.CreateInterfaceProxyWithTargetInterface(typeof(ISimpleInterface),
																	  new ClassWithInterface(),
																	  WithBase<DefaultProtectedCtor>()) as ISimpleInterface;
			@interface.Do();
		}

		private ProxyGenerationOptions WithBase<T>()
		{
			return new ProxyGenerationOptions() { BaseTypeForInterfaceProxy = typeof(T) };
		}
	}

	public sealed class Sealed{}

	public class NoDefaultCtor
	{
		public NoDefaultCtor(string someParam){}
	}

	public class DefaultProtectedCtor
	{
		protected DefaultProtectedCtor() { }
	}

	public class DefaultPrivateCtor
	{
		private DefaultPrivateCtor() { }
	}

}