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

namespace Castle.DynamicProxy.Tests.BugsReported
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class DynProxy230 : BasePEVerifyTestCase
    {
        [Test]
        public void ShouldGenerateTypeWithUnorderedBaseInterfacesClassProxy()
        {
            var options = new Castle.DynamicProxy.ProxyGenerationOptions();
            options.AddMixinInstance(new Foo());
            options.AddMixinInstance(new Bar());
            generator.CreateClassProxy(typeof(Foo), options);

            options = new Castle.DynamicProxy.ProxyGenerationOptions();
            options.AddMixinInstance(new Bar());
            options.AddMixinInstance(new Baz());
            generator.CreateClassProxy(typeof(Baz), options);
        }
    }

    public interface IFoo1
    {
    }

    public class Foo : IFoo1
    {
    }

    public interface IFoo2
    {
    }

    public class Bar : IFoo2
    {
    }

    public interface IFoo3
    {
    }

    public class Baz : IFoo3
    {
    }

}