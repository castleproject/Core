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

namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
	using System.Collections;
	using System.Reflection;
	using Boo.Lang;
	using Castle.DynamicProxy;
	using Castle.MonoRail.Framework;
    using System;

    [Serializable]
    public class HomeController : Controller
    {
		public void CanUseUrlHelperWithoutPrefix()
		{

		}

        public void Bag()
        {
            this.PropertyBag.Add("CustomerName", "hammett");
            string[] textArray1 = new string[] { "1", "2", "3" };
            this.PropertyBag.Add("List", textArray1);
        }

		public void ComplexNestedExpressions()
		{
			PropertyBag["title"] = "first";
			PropertyBag["pageIndex"] = 5;
		}

		public void ComplexNestedExpressions2()
		{
			PropertyBag["title"] = "first";
			PropertyBag["pageIndex"] = 5;
		}

        public void Bag2()
        {
            this.PropertyBag.Add("CustomerName", "hammett");
            string[] textArray1 = new string[] { "1", "2", "3" };
            this.PropertyBag.Add("List", textArray1);
        }

        public void Empty()
        {
        }

        public void HelloFromCommon()
        {
        }

        public void Index()
        {
        }

        public void WithNothingAfterTheLastSeperator()
        {
            
        }

		public void WithDynamicProxyObject()
		{
			ProxyGenerator generator = new ProxyGenerator();
			object o = generator.CreateClassProxy(typeof(SimpleProxy),new StandardInterceptor());
			try
			{
				o.GetType().GetProperty("Text");
				throw new InvalidOperationException("Should have gotten AmbiguousMatchException  here");
			}
			catch
			{
			}
			PropertyBag["src"] = o;
		}

		public void WithNullableDynamicProxyObject()
		{
			ProxyGenerator generator = new ProxyGenerator();
			SimpleProxy proxy = (SimpleProxy)generator.CreateClassProxy(typeof(SimpleProxy), new StandardInterceptor());
			PropertyBag["src"] = proxy;
		}

		public class SimpleProxy
		{
			private string text = "BarBaz";
			readonly Hashtable items = new Hashtable();

			public virtual object this[string key]
			{
				get { return items[key]; }
				set{ items[key] = value; }
			}

			public virtual string Text
			{
				get { return text; }
				set { text = value;}
			}

			public virtual string Say()
			{
				return "what?";
			}
		}

        public void NullableProperties()
        {
            Foo[] fooArray1 = new Foo[] { new Foo("Bar"), new Foo(null), new Foo("Baz") };
            this.PropertyBag.Add("List", fooArray1);
        }

        public void Nullables()
        {
            this.PropertyBag.Add("doesExists", "foo");
        }

        public void PreProcessor()
        {
            this.PropertyBag.Add("Title", "Ayende");
        }

        public void RedirectAction()
        {
            this.Redirect("home", "index");
        }

        public void RedirectForOtherArea()
        {
            this.Redirect("subrea", "home", "index");
        }

        public void ShowEmptyList()
        {
            this.PropertyBag.Add("dic", new Hash());
            this.RenderView("ShowList");
        }

        public void ShowList()
        {
			Hashtable values = new Hashtable();
            values.Add("Ayende", "Rahien");
            values.Add("Foo", "Bar");
            this.PropertyBag.Add("dic", values);
        }

        public void Welcome()
        {
            this.RenderView("heyhello");
        }

		public void Repeater()
		{
			
		}

		public void NamespacesInConfig()
		{
			
		}

		public void SubView()
		{
			
		}

		public void Javascript()
		{
			
		}

		public void Javascript2()
		{
			
		}

		public void UsingQuotes()
		{
			
		}

        public void DuckOverloadToString()
        {
            PropertyBag["birthday"] = new DateTime(1981, 12, 20);
        }

        public void NullPropagation()
        {
            
        }
    }
}

