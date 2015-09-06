// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

namespace CastleTests.DynamicProxyAsync.Test
{
#if DOTNET45
    using System.Threading.Tasks;

    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Generators;

    using NUnit.Framework;

    [TestFixture]
    public class AsyncInterceptorTestCase
    {
        ProxyGenerator proxyGenerator;

#if FEATURE_XUNITNET
		public AsyncInterceptorTestCase()
#else
        [SetUp]
        public void Setup()
#endif
        {
            proxyGenerator = new ProxyGenerator();
        }

        public class MyInterceptorAsync : IInterceptorAsync
        {
            public async Task InterceptAsync(IInvocationAsync invocation)
            {
                await Task.Delay(100);
                invocation.ReturnValue = Task.FromResult("a string");
            }
        }

        [Test]
        public async Task InterceptorTestCase()
        {
            var result = proxyGenerator.CreateAsyncInterfaceProxyWithoutTarget<ITaskString>(new MyInterceptorAsync());

            Assert.IsNotNull(result);

            var returnValue = await result.NormalMethod();
            Assert.AreEqual("a string", returnValue);
        }

        public interface ITaskString
        {
            Task<string> NormalMethod();
        }
    }
#endif
}