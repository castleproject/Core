// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Behaviors
{
	using System;
	using System.IO;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Dispatcher;
	using System.Xml;
	using Castle.Core.Logging;

	/// <summary>
	/// Message interceptor for logging client requests.
	/// </summary>
	public class LogMessageInspector : IClientMessageInspector, IDispatchMessageInspector
	{
		private readonly IExtendedLogger logger;
		private readonly IFormatProvider formatter;
		private readonly string format;

		/// <summary>
		/// Constructs a new <see cref="LogMessageInspector"/>
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="formatter">The formatter.</param>
		/// <param name="format">The format.</param>
		public LogMessageInspector(IExtendedLogger logger, IFormatProvider formatter, string format)
		{
			this.logger = logger;
			this.formatter = formatter;
			this.format = format;
		}

		#region IClientMessageInspector

		/// <summary>
		/// Logs the outgoing request.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			if (logger.IsInfoEnabled)
			{
				string correlationId = ObtainCorrelationId(request);

				using (logger.ThreadStacks["NDC"].Push(correlationId))
                {
					logger.Info("Sending request to {0}", channel.RemoteAddress);
					LogMessageContents(ref request);                	
                }

				return correlationId;
			}

			return null;
		}

		/// <summary>
		/// Logs the incoming response.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (logger.IsInfoEnabled)
			{
				object correlationId = correlationState ?? string.Empty;

				using (logger.ThreadStacks["NDC"].Push(correlationId.ToString()))
				{
					logger.Info("Received response for request {0}", correlationId);
					LogMessageContents(ref reply);
				}
			}
		}

		#endregion

		#region IDispatchMessageInspector

		/// <summary>
		/// Logs the incoming request.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="channel"></param>
		/// <param name="instanceContext"></param>
		/// <returns></returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			if (logger.IsInfoEnabled)
			{
				string correlationId = ObtainCorrelationId(request);

				using (logger.ThreadStacks["NDC"].Push(correlationId))
				{
					logger.Info("Received request from {0}", channel.RemoteAddress);
					LogMessageContents(ref request);
				}

				return correlationId;
			}

			return null;
		}

		/// <summary>
		/// Logs the outgoing response.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			if (logger.IsInfoEnabled)
			{
				object correlationId = correlationState ?? string.Empty;

				using (logger.ThreadStacks["NDC"].Push(correlationId.ToString()))
				{
					logger.Info("Sending response for request {0}", correlationId);
					LogMessageContents(ref reply);
				}
			}
		}

		#endregion

		private string ObtainCorrelationId(Message message)
		{
			UniqueId correlationId = message.Headers.MessageId;
			return (correlationId != null) ? correlationId.ToString() : Guid.NewGuid().ToString();
		}

		private void LogMessageContents(ref Message message)
		{
			MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue);
			Message forWriting = buffer.CreateMessage();
			message = buffer.CreateMessage();

			logger.InfoFormat(formatter, "{0:" + format + "}", forWriting);
		}
	}
}
