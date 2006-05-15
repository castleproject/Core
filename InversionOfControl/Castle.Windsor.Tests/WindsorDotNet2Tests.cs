#if DOTNET2

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Castle.Windsor.Tests.Components;

namespace Castle.Windsor.Tests
{
    [TestFixture]
    public class WindsorDotNet2Tests
    {
        private IWindsorContainer container;

        [SetUp]
        public void Init()
        {
            container = new WindsorContainer();
            container.AddComponent("calc", typeof(ICalcService), typeof(CalculatorService));
        }

        [Test]
        public void ResovleGeneric()
        {
            ICalcService svr = container.Resolve<ICalcService>();
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }

        [Test]
        public void ResovleGenericWithId()
        {
            ICalcService svr = container.Resolve<ICalcService>("calc");
            Assert.IsAssignableFrom(typeof(CalculatorService), svr);
        }
    }
}
#endif