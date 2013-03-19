namespace CastleTests.DynamicProxy.Tests.Classes
{
    using System;

    public class UninheritableCtorClass
    {
        //shouldn't have a default ctor

        private UninheritableCtorClass(bool _)
        {
            throw new NotImplementedException();
        }
    }
}
