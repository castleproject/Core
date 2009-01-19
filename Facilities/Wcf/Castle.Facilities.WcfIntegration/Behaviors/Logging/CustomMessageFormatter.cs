using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System;

namespace Castle.Facilities.WcfIntegration.Behaviors
{
	public class CustomMessageFormatter : AbstractMessageFormatter
	{
		public static readonly CustomMessageFormatter Instance = new CustomMessageFormatter();

		protected override string FormatMessage(Message message, string format)
		{
			StringBuilder output = new StringBuilder();

			if (string.IsNullOrEmpty(format))
			{
				format = "M";
			}

			if (TestFormat(ref format, 'h'))
			{
				FormattedHeaders(message, output);
			}

			if (format.Length > 0)
			{
				FormattedMessage(message, format[0], output);
			}

			return output.ToString();
		}

		private bool TestFormat(ref string format, char test)
		{
			if (format[0] == test)
			{
				format = format.Substring(1);
				return true;
			}
			return false;
		}

		private void FormattedHeaders(Message message, StringBuilder output)
		{
			foreach (var header in message.Headers)
			{
				output.AppendFormat("\n{0}\n", header);
			}
		}

		private void FormattedMessage(Message message, char format, StringBuilder output)
		{
			using (var writer = CreateWriter(message, format, output))
			{
				switch (format)
				{
					case 'b':
						message.WriteBody(writer);
						break;
					case 'B':
						message.WriteBodyContents(writer);
						break;
					case 's':
						message.WriteStartBody(writer);
						break;
					case 'S':
						message.WriteStartEnvelope(writer);
						break;
					case 'm':
					case 'M':
						message.WriteMessage(writer);
						break;
					default:
						return;
				}

				writer.Flush();
			}
		}

		protected virtual XmlWriter CreateWriter(StringBuilder output)
		{
			return XmlWriter.Create(output);
		}

		protected virtual XmlDictionaryWriter CreateWriter(Message message, char format, StringBuilder output)
		{
			return XmlDictionaryWriter.CreateDictionaryWriter(CreateWriter(output));
		}
	}
}
