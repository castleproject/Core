// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


#if !SILVERLIGHT && !NETCORE

namespace Castle.log4netIntegration
{
	using System.IO;
	using System.Linq;
	using System.Text;

	using Castle.Services.Logging.Log4netIntegration;

	using CastleTests.log4netIntegration;

	using log4net;
	using log4net.Appender;
	using log4net.Repository.Hierarchy;

	using Xunit;

	public class Log4netFactoryTestCase
	{
		private const string logMessage = "testing log4net configuration using a stream for configuration";
		private const string loggerName = "Log4netFactoryTestCase";

		private string GetLogContent()
		{
			var repository = (Hierarchy)LogManager.GetRepository();
			var memoryAppender = (from appender in repository.GetAppenders().OfType<MemoryAppender>() select appender).Single();

			return memoryAppender.GetEvents()[0].RenderedMessage;
		}

		private static Stream StringToStream(string s)
		{
			return new MemoryStream(Encoding.Default.GetBytes(s));
		}

		[Fact]
		public void CanCreateExtendedLog4NetConfigUsingStream()
		{
			ExtendedLog4netFactory factory;
			using (var stream = StringToStream(log4netConfig.Config))
			{
				factory = new ExtendedLog4netFactory(stream);
			}

			var logger = factory.Create(loggerName);
			logger.Debug(logMessage);

			var logContent = GetLogContent();

			Assert.Equal(logMessage, logContent);
		}

		[Fact]
		public void CanCreateLog4NetConfigUsingStream()
		{
			Log4netFactory factory;
			using (var stream = StringToStream(log4netConfig.Config))
			{
				factory = new Log4netFactory(stream);
			}

			var logger = factory.Create(loggerName);
			logger.Debug(logMessage);

			var logContent = GetLogContent();

			Assert.Equal(logMessage, logContent);
		}

		[Fact]
		public void CanCreateStreamFromString()
		{
			var original = log4netConfig.Config;
			using (var stream = StringToStream(original))
			{
				using (var reader = new StreamReader(stream))
				{
					var roundTrip = reader.ReadToEnd();

					Assert.Equal(original, roundTrip);
				}
			}
		}
	}
}

#endif