
using Castle.DynamicProxy;
using NUnit.Framework;

namespace CastleTests
{
    [TestFixture]
    public class MultipleRepeatedInterface
    {
        [Test]
        public void CanGenerateProxyForInterfaceImplementingMultipleOfRepeatedInterface()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(Interfaces.IMultipleRepeated), new IInterceptor[0]);
            Assert.IsNotNull(proxy);
        }
    }
}
