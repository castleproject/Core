// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail.Tests
{
    using System;
    using System.Reflection;
    using Castle.MonoRail.Views.Brail.TestSite.Controllers;
    using DynamicProxy;
    using NUnit.Framework;

	[TestFixture]
	public class LanguageFeatures : BaseViewOnlyTestFixture
	{
		[Test]
		public void CanHandleDynamicProxyObjects()
		{
            ProxyGenerator generator = new ProxyGenerator();
            object o = generator.CreateClassProxy(typeof(HomeController.SimpleProxy), new StandardInterceptor());
            try
            {
                o.GetType().GetProperty("Text");
                throw new InvalidOperationException("Should have gotten AmbiguousMatchException  here");
            }
            catch(AmbiguousMatchException)
            {
            }
            PropertyBag["src"] = o;

			string expected = "<?xml version=\"1.0\" ?>\r\n" +
			                  @"<html>
<h1>BarBaz</h1>
</html>";
			// should not raise ambigious match exception
			ProcessView_StripRailsExtension("Home/withDynamicProxyObject.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void NullableProperties()
		{
            Foo[] fooArray1 = new Foo[] { new Foo("Bar"), new Foo(null), new Foo("Baz") };
            this.PropertyBag.Add("List", fooArray1);

			string expected = "<?xml version=\"1.0\" ?>\r\n" +
			                  @"<html>
<h1>BarBaz</h1>
</html>";
			// should not raise null exception
			ProcessView_StripRailsExtension("Home/nullableProperties.rails");
			AssertReplyEqualTo(expected);
		}
	}
}