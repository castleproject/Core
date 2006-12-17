
using Castle.Igloo.Contexts;
using Castle.Igloo.Navigation;
using NUnit.Framework;


namespace Castle.Igloo.Test.Contexts
{
    public class ContextsTest : BaseTest
    {

        /// <summary>
        /// Test IContexts
        /// </summary>
        [Test]
        public void TestIContexts()
        {
            IContexts contexts = container.Resolve<IContexts>();
            Assert.IsNotNull(contexts);

            Assert.IsNotNull(contexts.ApplicationContext);
            Assert.IsNotNull(contexts.ConversationContext);
            Assert.IsNotNull(contexts.PageContext);
            Assert.IsNotNull(contexts.RequestContext);
            Assert.IsNotNull(contexts.SessionContext);
        }

        /// <summary>
        /// Test RequestContext
        /// </summary>
        [Test]
        public void TestRequestContext()
        {
            IContexts contexts = container.Resolve<IContexts>();
            Assert.IsNotNull(contexts);

            IContext request = contexts.RequestContext;

            Assert.IsTrue(request.Contains(NavigationContext.NAVIGATION_CONTEXT));

            Component1 comp1 = new Component1();

            request.Add("Component1", comp1);
            Assert.IsTrue(request.Contains("Component1"));

            Component1 comp = request["Component1"] as Component1;

            Assert.IsNotNull(comp);
            
            Assert.AreSame(comp1, comp);
        }
        
        private class Component1
        {
            
        }
    }
}
