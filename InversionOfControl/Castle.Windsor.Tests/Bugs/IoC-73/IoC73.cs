// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SILVERLIGHT // we do not support xml config on SL

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

#endif