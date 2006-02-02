namespace Castle.Components.Common.EmailSender2.Tests
{
    using Castle.Components.Common.EmailSender2.MockSender;
    using Castle.Components.Common.EmailSender.DotNet2Sender;
    using Castle.MicroKernel;
    using NUnit.Framework;
    using Castle.Windsor;

    /// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class MessageTests
	{
        IKernel kernel = new Castle.MicroKernel.DefaultKernel();

        [SetUp] public void Setup()
        {
            kernel.AddComponent("key", typeof(IEmailSender), typeof(MockSecureSender));
        }
        [TearDown] public void Teardown()
        {
            kernel.Dispose();
        }

        [Test]
        public void TestAuthentication()
        {
            IEmailSender sender = kernel[typeof(IEmailSender)] as IEmailSender;

            Message m = new Message();
            m.AddAuthentication("castle", "rocks");
            sender.Send(m);
        }
	}
}
