using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs.IoC_78
{
    [TestFixture]
    public class IoC78
    {
        [Test]
        public void WillIgnoreComponentsThatAreAlreadyInTheDependencyTracker_Constructor()
        {
            IWindsorContainer container = new WindsorContainer();
            container.AddComponent("chain", typeof(IChain), typeof(MyChain));
            container.AddComponent("chain2", typeof(IChain), typeof(MyChain2));

            IChain resolve = container.Resolve<IChain>("chain2");
            Assert.IsNotNull(resolve);
        }


        [Test]
        public void WillIgnoreComponentsThatAreAlreadyInTheDependencyTracker_Property()
        {
            IWindsorContainer container = new WindsorContainer();
            container.AddComponent("chain", typeof(IChain), typeof(MyChain3));

            IChain resolve = container.Resolve<IChain>("chain");
            Assert.IsNotNull(resolve);
        }
    }

    public interface IChain
    {

    }

    public class MyChain : IChain
    {
        public MyChain()
        {

        }

        public MyChain(IChain chain)
        {

        }
    }

    public class MyChain2 : IChain
    {
        public MyChain2()
        {

        }

        public MyChain2(IChain chain)
        {

        }
    }

    public class MyChain3 : IChain
    {
        private IChain chain;

        public IChain Chain
        {
            get { return chain; }
            set { chain = value; }
        }
    }
}
