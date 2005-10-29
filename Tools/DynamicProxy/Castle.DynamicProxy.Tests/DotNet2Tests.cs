#if dotNet2
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Castle.DynamicProxy.Test
{
	[TestFixture]
	public class DotNet2Tests
	{
		[Test]
		public void ProxyGenericClass()
		{
			 ProxyGenerator pg = new ProxyGenerator();
             GenericClass<int> x = (GenericClass<int>)pg.CreateClassProxy(typeof(GenericClass<int>), 
				 new StandardInterceptor());

			Assert.IsFalse(x.SomeMethod());
		}

		[Test]
		public void ProxyGenericInterface()
		{
			List<int> ints = new List<int>();
			ProxyGenerator pg = new ProxyGenerator();
			IList<int> x = (IList<int>)pg.CreateProxy(typeof(IList<int>),
				new StandardInterceptor(), ints);

			Assert.AreEqual(0, x.Count);
		}

		[Test]
		public void ProxyGenericInterfaceWithTwoGenericParameters()
		{
			IDictionary<int, float> ints = new Dictionary<int, float>();
			ProxyGenerator pg = new ProxyGenerator();
			IDictionary<int, float> x = (IDictionary<int, float>)pg.CreateProxy(typeof(IDictionary<int, float>),
				new StandardInterceptor(), ints);

			Assert.AreEqual(0, x.Count);
		}
	}

	public class GenericClass<T> 
    {
        public virtual bool SomeMethod() { return false; }
    }

}
#endif