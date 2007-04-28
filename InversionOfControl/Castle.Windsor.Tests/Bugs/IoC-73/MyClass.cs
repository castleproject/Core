using System;

namespace Castle.Windsor.Tests.Bugs.IoC73
{
    public class MyClass : IMyClass
    {
        public void Foo()
        {
            Console.WriteLine("Foo");
        }
    }
}
