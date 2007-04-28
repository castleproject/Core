using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace Castle.Windsor.Tests.Bugs.IoC73
{
	using NUnit.Framework;

	[TestFixture]
    public class IoC73
    {
		[Test]
		public void ShouldNotThrowCircularDependencyException()
		{
			string config = @"
<configuration>
    <facilities>
    </facilities>
    <components>
        <component id='MyClass'
            service='Castle.Windsor.Tests.Bugs.IoC73.IMyClass, Castle.Windsor.Tests'
            type='Castle.Windsor.Tests.Bugs.IoC73.MyClass, Castle.Windsor.Tests'/>
        <component id='Proxy'
            service='Castle.Windsor.Tests.Bugs.IoC73.IMyClass, Castle.Windsor.Tests'
            type='Castle.Windsor.Tests.Bugs.IoC73.Proxy, Castle.Windsor.Tests'>
            <parameters>
                <myClass>${MyClass}</myClass>
            </parameters>
        </component>
        <component id='ClassUser'
            type='Castle.Windsor.Tests.Bugs.IoC73.ClassUser, Castle.Windsor.Tests'>
            <parameters>
                <myClass>${Proxy}</myClass>
            </parameters>
        </component>
    </components>
</configuration>";

            IWindsorContainer container = new WindsorContainer
                (new XmlInterpreter(new StaticContentResource(config)));
            ClassUser classUser = container.Resolve<ClassUser>();
            classUser.Bar();
        }
    }
}
