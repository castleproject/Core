using System;
using System.ServiceModel.Channels;

namespace Castle.Facilities.WcfIntegration.Behaviors
{
	public abstract class AbstractMessageFormatter : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			return (typeof(ICustomFormatter).Equals(formatType)) ? this : null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (arg == null)
			{
				return string.Empty;
			}

			Message message = arg as Message;

			if (message != null)
			{
				return FormatMessage(message, format);
			}

			return arg.ToString();
		}

		protected abstract string FormatMessage(Message message, string format);
	}
}
