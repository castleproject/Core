using System;
using System.IO;
using System.Text;
using Castle.Services.Logging.Log4netIntegration;
using NUnit.Framework;

namespace Castle.log4netIntegration
{
	[TestFixture]
	public class Log4netFactoryTestCase
	{
		private const string logMessage = "testing log4net configuration using a stream for configuration";
		private const string logFileName = "unit-test-log-file.txt";

		[Test]
		public void CanCreateStreamFromString()
		{
			string original = log4netConfig.Config;
			using (var stream = StringToStream(original))
			using (var reader = new StreamReader(stream))
			{
				string roundTrip = reader.ReadToEnd();

				Assert.AreEqual(original, roundTrip);
			}
		}

		[Test]
		public void CanCreateLog4NetConfigUsingStream()
		{
			Log4netFactory factory;
			using (var stream = StringToStream(log4netConfig.Config))
			{
				factory = new Log4netFactory(stream);
			}

			var logger = factory.Create("UseMessageOnlyFileLog");
			logger.Debug(logMessage);

			var logContent = File.ReadAllText(logFileName);

			Assert.AreEqual(logMessage, logContent);
		}

		[Test]
		public void CanCreateExtendedLog4NetConfigUsingStream()
		{
			ExtendedLog4netFactory factory;
			using (var stream = StringToStream(log4netConfig.Config))
			{
				factory = new ExtendedLog4netFactory(stream);
			}

			var logger = factory.Create("UseMessageOnlyFileLog");
			logger.Debug(logMessage);

			var logContent = File.ReadAllText(logFileName);

			Assert.AreEqual(logMessage, logContent);
		}

		[TearDown]
		public  void TearDown()
		{
			if (File.Exists(logFileName))
			{
				File.Delete(logFileName);
			}
		}

		private static Stream StringToStream(string s)
		{
			return new MemoryStream(Encoding.Default.GetBytes(s));
		}
	}
}