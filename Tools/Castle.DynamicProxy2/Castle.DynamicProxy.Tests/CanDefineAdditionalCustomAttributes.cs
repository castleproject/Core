using System;
using NUnit.Framework;

namespace Castle.DynamicProxy.Tests
{
    [TestFixture]
    public class CanDefineAdditionalCustomAttributes
    {
        [Test]
        public void On_class()
        {
            var proxy = new ProxyGenerator().CreateClassProxy(typeof (CanDefineAdditionalCustomAttributes),
                                                              new ProxyGenerationOptions()
                                                              {
                                                                  AttributesToAddToGeneratedTypes =
                                                                      {
                                                                          new __Protect()
                                                                      }
                                                              });

            Assert.IsTrue(proxy.GetType().IsDefined(typeof (__Protect), false));
        }

        [Test]
        public void On_interfaces()
        {
            var proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(typeof(IDisposable),new Type[0],
                                                              new ProxyGenerationOptions()
                                                              {
                                                                  AttributesToAddToGeneratedTypes =
                                                                      {
                                                                          new __Protect()
                                                                      }
                                                              });

            Assert.IsTrue(proxy.GetType().IsDefined(typeof(__Protect), false));
        }
    }

    public class __Protect : Attribute
    {
    }
}