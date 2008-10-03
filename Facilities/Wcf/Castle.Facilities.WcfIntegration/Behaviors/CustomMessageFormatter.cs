using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

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
			foreach (MessageHeader header in message.Headers)
			{
				output.AppendFormat("\n{0}\n", header);
			}
		}

		private void FormattedMessage(Message message, char format, StringBuilder output)
		{
			XmlWriter writer = XmlWriter.Create(output);

			using (XmlDictionaryWriter dictWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer))
			{
				switch (format)
				{
					case 'b':
						message.WriteBody(dictWriter);
						break;
					case 'B':
						message.WriteBodyContents(dictWriter);
						break;
					case 's':
						message.WriteStartBody(dictWriter);
						break;
					case 'S':
						message.WriteStartEnvelope(dictWriter);
						break;
					case 'm':
					case 'M':
						message.WriteMessage(dictWriter);
						break;
					default:
						return;
				}

				dictWriter.Flush();
			}

			output.Append(writer.ToString());
		}
	}
}
