using Castle.Windsor;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_115
    {
        [Test]
        public void  Cannot_resolve_a_dependency_from_a_parent_container_under_certain_circumstances()
        {
            WindsorContainer parent = new WindsorContainer();
            WindsorContainer child = new WindsorContainer();

            parent.AddChildContainer(child);

            parent.AddComponent("service", typeof(IParentService), typeof(ParentService));
            child.AddComponent("service1", typeof(IChildService1), typeof(ChildService1));
            child.AddComponent("service2", typeof(IChildService2), typeof(ChildService2));


            child.Resolve<IChildService1>();
        }


        [Test]
        public void Should_resolve_child_from_childs_container()
        {
            WindsorContainer parent = new WindsorContainer();
            WindsorContainer child = new WindsorContainer();

            parent.AddChildContainer(child);

            parent.AddComponent("service1", typeof(IParentService), typeof(ParentService));
            parent.AddComponent("service3", typeof(IChildService2), typeof(ChildService2));
            child.AddComponent("service2", typeof(IParentService), typeof(AnotherParentService));

            var resolve = child.Resolve<IChildService2>();
            Assert.IsInstanceOfType(typeof(AnotherParentService),resolve.Parent);
        }

        public interface IParentService
        {
        }

        public class ParentService : IParentService
        {
        }

        public interface IChildService1
        {
        }

        public class ChildService1 : IChildService1
        {
            public ChildService1(IChildService2 xxx)
            {
            }
        }

        public interface IChildService2
        {
            IParentService Parent { get; }
        }

        public class ChildService2 : IChildService2
        {
            private readonly IParentService xxx;

            public ChildService2(IParentService xxx)
            {
                this.xxx = xxx;
            }

            public IParentService Parent
            {
                get { return xxx; }
            }
        }

        public class AnotherParentService : IParentService
        {
        }
    }
}