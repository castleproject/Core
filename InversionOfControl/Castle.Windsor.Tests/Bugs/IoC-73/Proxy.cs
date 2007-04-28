namespace Castle.Windsor.Tests.Bugs.IoC73
{
    public class Proxy : IMyClass
    {
        IMyClass _myClass;
        
        public Proxy(IMyClass myClass)
        {
            _myClass = myClass;
        }
        
        public void Foo()
        {
            _myClass.Foo();
        }
    }
}
