
using Castle.Igloo.Scopes;
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
            IScopeRegistry scopeRegistry = container.Resolve<IScopeRegistry>();
            Assert.IsNotNull(scopeRegistry);

            //Assert.IsNotNull(contexts.ApplicationContext);
            //Assert.IsNotNull(contexts.ConversationContext);
            //Assert.IsNotNull(contexts.PageContext);
            //Assert.IsNotNull(contexts.RequestContext);
            //Assert.IsNotNull(contexts.SessionContext);
        }

        /// <summary>
        /// Test RequestContext
        /// </summary>
        [Test]
        public void TestRequestContext()
        {
            IScopeRegistry scopeRegistry = container.Resolve<IScopeRegistry>();
            Assert.IsNotNull(scopeRegistry);

            //IScope request = contexts.RequestContext;

            //Assert.IsTrue(request.Contains(NavigationState.NAVIGATION_STATE));

            //Component1 comp1 = new Component1();

            //request.Add("Component1", comp1);
            //Assert.IsTrue(request.Contains("Component1"));

            //Component1 comp = request["Component1"] as Component1;

            //Assert.IsNotNull(comp);
            
            //Assert.AreSame(comp1, comp);
        }
        
        private class Component1
        {
            
        }
    }
}
