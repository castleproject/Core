using System.Collections;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
    [TestFixture]
    public class IoC_147
    {
        [Test]
        public void ShouldBeAbleToSupplyDictionaryAsARuntimeParameter()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<ClassTakingDictionary>());

            var someDictionary = new Hashtable();

            var s = container.Resolve<ClassTakingDictionary>(new { SomeDictionary = someDictionary });
            Assert.IsNotNull(s);
        }


        public class ClassTakingDictionary
        {
            public IDictionary SomeDictionary { get; set; }
        }
    }
}