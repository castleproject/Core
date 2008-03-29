namespace Castle.MonoRail.Views.Brail.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using Controllers;
    using NUnit.Framework;

    [TestFixture]
    public class DslFixture : BaseViewOnlyTestFixture
    {
        public DslFixture()
			: base(ViewLocations.BrailTestsView)
        {

        }

        [Test]
        public void CanOutputParagraphWithClassAttribute()
        {
            ProcessView("dsl/attributeOutput");
            AssertReplyEqualTo("<html><p class=\"foo\">hello world</p></html>");
        }

        [Test]
        public void ParametersCanBeReferencedByView()
        {
            PropertyBag["SayHelloTo"] = "Harris";
            ProcessView("dsl/expandParameters");
            AssertReplyEqualTo("<html><p>hello, Harris</p></html>");
        }

        [Test]
        public void RegisterHtmlAllowsCallToHtml()
        {
            ProcessView("dsl/registerHtml");
            AssertReplyEqualTo(@"<html>hello world</html>");
        }
    }
}