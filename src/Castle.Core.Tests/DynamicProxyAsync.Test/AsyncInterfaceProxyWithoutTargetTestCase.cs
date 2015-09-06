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
    public class AsyncInterfaceProxyWithoutTargetTestCase
    {
        ProxyGenerator proxyGenerator;

#if FEATURE_XUNITNET
		public GenericInterfaceWithGenericMethod()
#else
        [SetUp]
        public void Setup()
#endif
        {
            proxyGenerator = new ProxyGenerator();
        }

        [Test]
        public void GenerateCase()
        {
            var type = typeof(IMinimum);
            var result = proxyGenerator.CreateAsyncInterfaceProxyWithoutTarget(type);

            Assert.IsNotNull(result as IMinimum);
        }

        public interface IMinimum
        {
            Task NormalMethod();
        }

        [Test, ExpectedException(typeof(GeneratorException))]
        public void GivenRefParameterVerifyExceptionTestCase()
        {
            proxyGenerator.CreateAsyncInterfaceProxyWithoutTarget<IRef>();
        }

        public interface IRef
        {
            Task RefMethod(ref int reference);
        }

        [Test, ExpectedException(typeof(GeneratorException))]
        public void GivenOutParameterVerifyExceptionTestCase()
        {
            proxyGenerator.CreateAsyncInterfaceProxyWithoutTarget<IOut>();
        }

        public interface IOut
        {
            Task RefMethod(out int output);
        }

        [Test, ExpectedException(typeof(GeneratorException))]
        public void GivenNonTaskMethodVerifyExceptionTestCase()
        {
            proxyGenerator.CreateAsyncInterfaceProxyWithoutTarget<INonTask>();
        }

        public interface INonTask
        {
            int RefMethod();
        }
    }
#endif
}