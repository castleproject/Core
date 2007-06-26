
namespace Castle.MonoRail.Views.Brail.Tests
{
    using Castle.MonoRail.Framework.Tests;
    using NUnit.Framework;
    using Castle.MonoRail.TestSupport;
    using Castle.MonoRail.Views.Brail.Tests.Controllers;
    using System.IO;
    using System.Configuration;

    [TestFixture]
    public class DslFixture : BaseControllerTest
    {
        [Test]
        public void RegisterHtmlAllowsCallToHtml()
        {
            BooViewEngine bve = new BooViewEngine();
            string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
            bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
            bve.Initialize();
            StringWriter sw = new StringWriter();
            DslController controller = new DslController();
            PrepareController(controller, "", "dsl", "registerHtml");
            bve.Process(sw, Context, controller, "dsl/registerHtml");
            Assert.AreEqual(@"<html>hello world</html>", sw.ToString());
        }

        [Test]
        public void CanOutputParagraphWithClassAttribute()
        {
            BooViewEngine bve = new BooViewEngine();
            string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
            bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
            bve.Initialize();
            StringWriter sw = new StringWriter();
            DslController controller = new DslController();
            PrepareController(controller, "", "dsl", "attributeOutput");
            bve.Process(sw, Context, controller, "dsl/attributeOutput");
            Assert.AreEqual("<html><p class=\"foo\">hello world</p></html>", sw.ToString());
        }

        [Test]
        public void ParametersCanBeReferencedByView()
        {
            BooViewEngine bve = new BooViewEngine();
            string viewPath = Path.Combine(ConfigurationManager.AppSettings["tests.src"], "Views");
            bve.Service(new ViewSourceLoaderServiceProvider(viewPath));
            bve.Initialize();
            StringWriter sw = new StringWriter();
            DslController controller = new DslController();
            controller.PropertyBag["SayHelloTo"] = "Harris";
            PrepareController(controller, "", "dsl", "expandParameters");
            bve.Process(sw, Context, controller, "dsl/expandParameters");
            Assert.AreEqual("<html><p>hello, Harris</p></html>", sw.ToString());
        }
    }
}
