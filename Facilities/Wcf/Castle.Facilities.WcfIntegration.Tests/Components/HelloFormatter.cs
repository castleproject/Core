using System.ServiceModel.Channels;
using Castle.Facilities.WcfIntegration.Behaviors;

namespace Castle.Facilities.WcfIntegration.Tests
{
	public class HelloFormatter : AbstractMessageFormatter
	{
		protected override string FormatMessage(Message message, string format)
		{
			return "Hello";
		}
	}

	public class HelloMessageFormat : LogMessageFormat
	{
		public HelloMessageFormat() : base(new HelloFormatter(), "")
		{
		}
	}
}
