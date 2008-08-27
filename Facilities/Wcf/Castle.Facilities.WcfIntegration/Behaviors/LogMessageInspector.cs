// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Dispatcher;
	using Castle.Core.Logging;

	/// <summary>
	/// Message interceptor for logging client requests.
	/// </summary>
	public class LogMessageInspector : IClientMessageInspector, IDispatchMessageInspector
	{
		private readonly ILogger logger;

		/// <summary>
		/// Constructs a new <see cref="LogMessageInspector"/>
		/// </summary>
		/// <param name="contract">The service contract.</param>
		/// <param name="factory">The logger factory.</param>
		public LogMessageInspector(Type contract, ILoggerFactory factory)
		{
			logger = factory.Create(contract);
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
			logger.Info("Sending request {0} to {1}", request.Headers.MessageId, channel.RemoteAddress);
			LogMessageContents(ref request);
			return request.Headers.MessageId;
		}

		/// <summary>
		/// Logs the incoming response.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			logger.Info("Received response for request {0}", correlationState);
			LogMessageContents(ref reply);
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
			logger.Info("Received request {0} from {1}", request.Headers.MessageId, channel.RemoteAddress);
			LogMessageContents(ref request);
			return request.Headers.MessageId;
		}

		/// <summary>
		/// Logs the outgoing response.
		/// </summary>
		/// <param name="reply"></param>
		/// <param name="correlationState"></param>
		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			logger.Info("Sending response for request {0}", correlationState);
			LogMessageContents(ref reply);
		}

		#endregion

		private void LogMessageContents(ref Message message)
		{
			MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue);
			message = buffer.CreateMessage();
			logger.Info(message.ToString());
		}
	}
}
