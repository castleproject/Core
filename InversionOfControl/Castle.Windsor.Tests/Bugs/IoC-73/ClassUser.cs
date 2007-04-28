using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Windsor.Tests.Bugs.IoC73
{
    public class ClassUser
    {
        IMyClass _myClass;
        
        public ClassUser(IMyClass myClass)
        {
            _myClass = myClass;
        }
        
        public void Bar()
        {
            _myClass.Foo();
        }
    }
}
