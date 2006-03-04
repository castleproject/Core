#if dotNet2
namespace Castle.DynamicProxy.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using System.Runtime.CompilerServices;

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

            GenericClass<string> y = (GenericClass<string>)pg.CreateClassProxy(typeof(GenericClass<string>),
                new StandardInterceptor());

            Assert.IsFalse(y.SomeMethod());
        }

        [Test]
        public void ProxyGenericInterface()
        {
            ProxyGenerator pg = new ProxyGenerator();
            IList<int> x = (IList<int>)pg.CreateProxy(typeof(IList<int>),
                new StandardInterceptor(), new List<int>());

            Assert.AreEqual(0, x.Count);

            IList<string> y = (IList<string>)pg.CreateProxy(typeof(IList<string>),
                new StandardInterceptor(), new List<string>());

            Assert.AreEqual(0, y.Count);
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

        [Test]
        public void ProxyInternalClass()
        {
            ProxyGenerator pg = new ProxyGenerator();
            LogInvocationInterceptor logger = new LogInvocationInterceptor();
            InternalClass x = (InternalClass)pg.CreateClassProxy(typeof(InternalClass),
                logger);
            x.Name = "ayende";
            Assert.AreEqual("ayende", x.Name);
            Assert.AreEqual("set_Name", logger.Invocations[0]);
            Assert.AreEqual("get_Name", logger.Invocations[1]);
        }
    }

    public class GenericClass<T>
    {
        public virtual bool SomeMethod() { return false; }
    }

    internal class InternalClass
    {
        String _name; 

        internal virtual String Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

}

#endif