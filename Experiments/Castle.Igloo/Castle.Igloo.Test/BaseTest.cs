using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;


namespace Castle.Igloo.Test
{
    [TestFixture] 
    public abstract class BaseTest
    {
        protected WindsorContainer container = null;

        #region SetUp & TearDown

        /// <summary>
        /// SetUp
        /// </summary>
        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            DefaultConfigurationStore store = new DefaultConfigurationStore();
            XmlInterpreter interpreter = new XmlInterpreter(new ConfigResource());
            interpreter.ProcessResource(interpreter.Source, store);

            container = new WindsorContainer(interpreter);
        }

        /// <summary>
        /// TearDown
        /// </summary>
        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
            container.Dispose();
        }


        #endregion
    }
}
