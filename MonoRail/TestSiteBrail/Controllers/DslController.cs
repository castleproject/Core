namespace Castle.MonoRail.Views.Brail.TestSite.Controllers
{
    using System;
    using System.Collections;
    using Castle.MonoRail.Framework;

    [Serializable, Layout("dsl")]
    public class DslController : SmartDispatcherController
    {
        public void TestSubViewOutput()
        {
            PropertyBag["SayHelloTo"] = "Harris";
        }

        public void TestSubViewWithComponents()
        {
            PropertyBag["items"] = new string[]
            {
                "Ayende",
                "Rahien"
            };            
        }

        public void TestXml()
        {
            PropertyBag["items"] = new string[]
            {
                "Ayende",
                "Rahien"
            };
        }
    }
}
