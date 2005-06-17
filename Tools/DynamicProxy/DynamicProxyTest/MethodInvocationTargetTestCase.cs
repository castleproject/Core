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
	using NUnit.Framework;
	using Castle.DynamicProxy.Test.ClassInterfaces;

	[TestFixture]
	public class MethodInvocationTargetTestCase
	{
		[Test]
		public void AttributesOnTargetClasses()
		{
			ProxyGenerator _generator = new ProxyGenerator();

			AttributeCheckerInterceptor interceptor = new AttributeCheckerInterceptor();

			object proxy = _generator.CreateClassProxy(typeof (MyInterfaceImpl), interceptor);

			IMyInterface inter = (IMyInterface) proxy;

			Assert.AreEqual(45, inter.Calc(20, 25));
			Assert.IsTrue(interceptor.LastTargethasAttribute);
			Assert.IsTrue(interceptor.LastMethodHasAttribute);

			Assert.AreEqual(48, inter.Calc(20, 25, 1, 2));
			Assert.IsTrue(interceptor.LastMethodHasAttribute);
			Assert.IsTrue(interceptor.LastTargethasAttribute);

			inter.Name = "hammett";
			Assert.IsFalse(interceptor.LastMethodHasAttribute);
			Assert.IsTrue(interceptor.LastTargethasAttribute);
		}

		[Test]
		public void AttributesOnTargetClassesWithInterfaceProxy()
		{
			ProxyGenerator _generator = new ProxyGenerator();

			AttributeCheckerInterceptor interceptor = new AttributeCheckerInterceptor();

			object proxy = _generator.CreateProxy(typeof (IMyInterface), interceptor, new MyInterfaceImpl());

			IMyInterface inter = (IMyInterface) proxy;

			Assert.AreEqual(45, inter.Calc(20, 25));
			Assert.IsTrue(interceptor.LastMethodHasAttribute);
			Assert.IsTrue(interceptor.LastTargethasAttribute);

			Assert.AreEqual(48, inter.Calc(20, 25, 1, 2));
			Assert.IsTrue(interceptor.LastMethodHasAttribute);
			Assert.IsTrue(interceptor.LastTargethasAttribute);

			inter.Name = "hammett";
			Assert.IsFalse(interceptor.LastMethodHasAttribute);
			Assert.IsTrue(interceptor.LastTargethasAttribute);
		}

		public class AttributeCheckerInterceptor : StandardInterceptor
		{
			bool lastMethodHasAttribute;
			bool lastTargethasAttribute;

			protected override void PreProceed(IInvocation invocation, params object[] args)
			{
				lastMethodHasAttribute = invocation.MethodInvocationTarget.IsDefined(typeof (MyAttribute), false);
				lastTargethasAttribute = invocation.InvocationTarget.GetType().IsDefined(typeof (MyAttribute), true);
			}

			public bool LastMethodHasAttribute
			{
				get { return lastMethodHasAttribute; }
			}

			public bool LastTargethasAttribute
			{
				get { return lastTargethasAttribute; }
			}
		}
	}
}