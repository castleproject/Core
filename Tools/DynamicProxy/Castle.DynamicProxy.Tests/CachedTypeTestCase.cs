// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Test.Classes;

	/// <summary>
	/// Summary description for CachedTypeTestCase.
	/// </summary>
	[TestFixture]
	public class CachedTypeTestCase
	{
		private ProxyGenerator _generator = new ProxyGenerator();

		[Test]
		public void CachedClassProxies()
		{
			object proxy = _generator.CreateClassProxy( 
				typeof(ServiceClass), new StandardInterceptor( ) );
			
			Assert.IsNotNull(proxy);
			Assert.IsTrue( typeof(ServiceClass).IsAssignableFrom( proxy.GetType() ) );

			proxy = _generator.CreateClassProxy( 
				typeof(ServiceClass), new StandardInterceptor( ) );
			
			Assert.IsNotNull(proxy);
			Assert.IsTrue( typeof(ServiceClass).IsAssignableFrom( proxy.GetType() ) );
		}

		[Test]
		public void GenerateProxyClassSameNameDistinctNamespace()
		{
			object proxyA = _generator.CreateClassProxy(typeof(Classes.SimpleClass),new StandardInterceptor());
			object proxyB = _generator.CreateClassProxy(typeof(Classes.SubNamespace.SimpleClass), new StandardInterceptor());

			Assert.IsTrue(proxyA is Classes.SimpleClass,
				string.Format("Proxy {0} is not a subclass of {1} ", proxyA.GetType(), typeof(Classes.SimpleClass)));

			Assert.IsTrue(proxyB is Classes.SubNamespace.SimpleClass,
				string.Format("Proxy {0} is not a subclass of {1}", proxyA.GetType(), typeof(Classes.SubNamespace.SimpleClass)));
		}

        [Test]
        public void GenerateProxyClassSameNameButOneIsInnerClass()
        {
            object proxyA = _generator.CreateClassProxy(typeof(Classes.Foo), new StandardInterceptor());
            object proxyB = _generator.CreateClassProxy(typeof(Classes.DuplicateNames.Foo), new StandardInterceptor());

            Assert.IsTrue(proxyA is Classes.Foo,
                string.Format("Proxy {0} is not a subclass of {1} ", proxyA.GetType(), typeof(Classes.Foo)));

            Assert.IsTrue(proxyB is Classes.DuplicateNames.Foo,
                string.Format("Proxy {0} is not a subclass of {1}", proxyA.GetType(), typeof(Classes.DuplicateNames.Foo)));
        }

	}
}
