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

using System;
using System.Text;
using NUnit.Framework;

namespace Castle.DynamicProxy.Test
{
    [TestFixture]
    public class DiamondProblem
    {
        public interface IFirst
        {
            void OriginalMethod1();
            void OriginalMethod2();
        }
        public class FirstImpl : IFirst
        {
            // NON-virtual method
            public void OriginalMethod1() { }
            // VIRTUAL method
            public virtual void OriginalMethod2() { }
        }
        public interface ISecond : IFirst
        {
            void ExtraMethod();
        }

        [Test]
        public void CreateProxyOfTypeWithMixinCausingDiamondWhenMethodIsNonVirtual()
        {
            ProxyGenerator generator = new ProxyGenerator();
            object proxy = generator.CreateClassProxy(typeof(FirstImpl), new Type[]{typeof(ISecond)},
                                                   new StandardInterceptor(), false); 
            Assert.IsTrue(proxy is FirstImpl);
            Assert.IsTrue(proxy is IFirst);
            Assert.IsTrue(proxy is ISecond);
        }
    }
}
