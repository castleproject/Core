// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

using System;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace CastleTests.BugsReported
{
   /// <summary>
   /// Contains repro for http://issues.castleproject.org/issue/DYNPROXY-170.
   /// </summary>
   [TestFixture]
   public sealed class DynProxy170 : BasePEVerifyTestCase
   {
      private ProxyGenerator proxyGenerator = new ProxyGenerator();
      private ProxyGenerationOptions proxyGenerationOptions;
      private IProxyGenerationHook generationHook;

      public DynProxy170()
      {
         this.generationHook = new ProxyGenerationHook();
         this.proxyGenerationOptions = new ProxyGenerationOptions(this.generationHook);
      }

      [Test]
      public void InterceptCallToPropertyInNonInterceptedMethod()
      {
         bool wasIntercepted = false;

         MyInterceptor interceptor = new MyInterceptor();
         interceptor.Intercepted += (s, e) =>
         {
            wasIntercepted = true;
         };
         Person p = new Person { Name = "John" };
         p.SetAgeTo(50);
         p = Decorate(p, interceptor);

         p.SetAgeTo(100);

         var originalObject = p.GetType().GetField("__target").GetValue(p);
         var ageInOriginalObject = originalObject.GetType().GetProperty("Age", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(originalObject, null);

         Assert.That(ageInOriginalObject, Is.EqualTo(100)); // The value is still 50
         Assert.That(wasIntercepted, Is.True); // The call was not intercepted
      }

      private Person Decorate(Person person, IInterceptor interceptor)
      {
         var result = proxyGenerator.CreateClassProxyWithTarget(person.GetType(), new Type[0], person, proxyGenerationOptions, interceptor);
         return (Person)result;
      }

      internal class Person
      {
         public virtual string Name { get; set; }
         protected virtual int Age { get; set; } // it works if the property is public

         public void SetAgeTo(int value)
         {
            Age = value;
         }
      }

      private sealed class MyInterceptor : IInterceptor
      {
         public event EventHandler Intercepted;

         public void Intercept(IInvocation invocation)
         {
            if (invocation.Method.Name == "set_Age")
            {
               OnIntercepted();
            }

            invocation.Proceed();
         }

         private void OnIntercepted()
         {
            var tmp = Intercepted;
            if (tmp != null)
            {
               tmp(this, EventArgs.Empty);
            }
         }
      }

      private sealed class ProxyGenerationHook : IProxyGenerationHook
      {
         public void MethodsInspected()
         { }

         public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
         { }

         public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
         {
            if (methodInfo == null)
            {
               throw new ArgumentNullException("methodInfo");
            }

            string methodName = methodInfo.Name;
            bool result = methodName.StartsWith("set_", StringComparison.OrdinalIgnoreCase) ||
                          methodName.StartsWith("get_", StringComparison.OrdinalIgnoreCase);

            return result;
         }

         public override bool Equals(object obj)
         {
            return obj is ProxyGenerationHook; // all instances are equal
         }

         public override int GetHashCode()
         {
            return 7;
         }
      }
   }
}