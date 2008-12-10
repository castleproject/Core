using System;
using Castle.Facilities.Startable;
using NUnit.Framework;

namespace Castle.MicroKernel.Tests.Bugs
{
    [TestFixture]
    public class IoC_95
    {
        [Test]
        public void AddingComponentToRootKernelWhenChildKernelHasStartableFacility()
        {
            IKernel kernel = new DefaultKernel();
            IKernel childKernel = new DefaultKernel();
            kernel.AddChildKernel(childKernel);
            childKernel.AddFacility("StartableFacility", new StartableFacility());
            kernel.AddComponent("string", typeof(String)); // exception here
        }
    }
}