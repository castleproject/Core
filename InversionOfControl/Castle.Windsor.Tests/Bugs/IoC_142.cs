using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_142
    {
        [Test]
        public void ShouldBeAbleToSupplyValueForNullableParam()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ClassTakingNullable>());

            var s = container.Resolve<ClassTakingNullable>(new { SomeVal = 5 });
            Assert.IsNotNull(s.SomeVal);
        }

        [Test]
        public void ShouldBeAbleToSupplyValueForNullableParamViaCtor()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ClassTakingNullableViaCtor>());

            var s = container.Resolve<ClassTakingNullableViaCtor>(new { foo = 5d });
            Assert.IsNotNull(s);
        }

        [Test]
        public void ShouldBeAbleToSupplyValueForNullableParamViaCtor_FromConfig()
        {
            var container = new WindsorContainer();
            var configuration = new MutableConfiguration("parameters");
            configuration.CreateChild("foo", "5");
            container.Register(Component.For<ClassTakingNullableViaCtor>().Configuration(configuration));

            var s = container.Resolve<ClassTakingNullableViaCtor>();
            Assert.IsNotNull(s);
        }

        [Test]
        public void ShouldBeAbleToSupplyValueForNullableParam_FromConfig()
        {
            var container = new WindsorContainer();
            var configuration = new MutableConfiguration("parameters");
            configuration.CreateChild("SomeVal","5");
            container.Register(Component.For<ClassTakingNullable>().Configuration(configuration));

            var s = container.Resolve<ClassTakingNullable>();
            Assert.IsNotNull(s.SomeVal);
        }

        public class ClassTakingNullable
        {
            public int? SomeVal { get; set; }
        }

        public class ClassTakingNullableViaCtor
        {
            public ClassTakingNullableViaCtor(double? foo)
            {
                
            }
        }
    }
}