using System;

namespace CastleTests.DynamicProxy.Tests.Classes
{
    public class ThrowsInCtorClass
    {
        public ThrowsInCtorClass()
        {
            throw new ApplicationException();
        }
    }
}
