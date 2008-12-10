using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_127
    {
        [Test]
        public void AddComponentInstanceAndChildContainers()
        {
            IWindsorContainer parent = new WindsorContainer();
            IWindsorContainer child = new WindsorContainer();
            parent.AddChildContainer(child);

            IClock clock1 = new IsraelClock();
            IClock clock2 = new WorldClock();

            parent.Kernel.AddComponentInstance<IClock>(clock2);
            child.Kernel.AddComponentInstance<IClock>(clock1);

            Assert.AreSame(clock2,parent.Resolve<IClock>());
            Assert.AreSame(clock1, child.Resolve<IClock>());

        }
    }
}