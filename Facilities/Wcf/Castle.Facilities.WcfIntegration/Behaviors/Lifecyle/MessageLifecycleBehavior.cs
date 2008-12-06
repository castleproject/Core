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
	using System.Collections.Generic;
	using System.IO;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using System.ServiceModel.Dispatcher;
	using System.Xml;

	public class MessageLifecycleBehavior<T> : AbstractExtensibleObject<T>,
		IEndpointBehavior, IClientMessageInspector, IDispatchMessageInspector 
		where T : MessageLifecycleBehavior<T>
	{
		#region IEndpointBehavior Members 

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(this);
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		#endregion

		#region IClientMessageInspector Members

		/// <summary>
		/// Processes the client response.
		/// </summary>
		/// <param name="reply">The client response.</param>
		/// <param name="correlationState"></param>
		public virtual void AfterReceiveReply(ref Message reply, object correlationState)
		{
			ProcessMessage(ref reply, MessageLifecycle.IncomingResponse);
		}

		/// <summary>
		/// Processes the client request.
		/// </summary>
		/// <param name="request">The client request.</param>
		/// <param name="channel"></param>
		/// <returns></returns>
		public virtual object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			ProcessMessage(ref request, MessageLifecycle.OutgoingRequest);
			return null;
		}

		#endregion

		#region IDispatchMessageInspector Members

		/// <summary>
		/// Processes the server request.
		/// </summary>
		/// <param name="request">The server request.</param>
		/// <param name="channel"></param>
		/// <param name="instanceContext"></param>
		/// <returns></returns>
		public virtual object AfterReceiveRequest(ref Message request, IClientChannel channel,
												  InstanceContext instanceContext)
		{
			ProcessMessage(ref request, MessageLifecycle.IncomingRequest);
			return null;
		}

		/// <summary>
		/// Processes the server response.
		/// </summary>
		/// <param name="reply">The server response.</param>
		/// <param name="correlationState"></param>
		public virtual void BeforeSendReply(ref Message reply, object correlationState)
		{
			ProcessMessage(ref reply, MessageLifecycle.OutgoingResponse);
		}

		#endregion

		protected void ProcessMessage(ref Message message, MessageLifecycle lifecycle)
		{
			XmlDocument envelope = null;

			ICollection<IMessageLifecyleAction> actions = Extensions.FindAll<IMessageLifecyleAction>();

			if (actions.Count > 0)
			{
				List<IMessageLifecyleAction> orderedActions = new List<IMessageLifecyleAction>(actions);
				orderedActions.Sort(ActionComparer.Instance);

				foreach (IMessageLifecyleAction action in orderedActions)
				{
					bool proceed = true;

					if (!action.ShouldPerform(lifecycle)) continue;

					if (action is IMessageEnvelopeAction)
					{
						IMessageEnvelopeAction envelopeAction = (IMessageEnvelopeAction)action;

						if (envelope == null)
						{
							envelope = OpenMessage(message);
						}

						proceed = envelopeAction.Perform(message, envelope, lifecycle);
					}
					else if (action is IMessageAction)
					{
						IMessageAction messageAction = (IMessageAction)action;

						if (envelope != null)
						{
							message = CloseMessage(message, envelope);
							envelope = null;
						}

						proceed = messageAction.Perform(ref message, lifecycle);
					}

					if (!proceed) break;
				}

				if (envelope != null)
				{
					message = CloseMessage(message, envelope);
				}
			}
		}

		private XmlDocument OpenMessage(Message message)
		{
			MemoryStream stream = new MemoryStream();
			XmlWriter writer = XmlWriter.Create(stream);
			message.WriteMessage(writer);
			writer.Flush();
			stream.Position = 0;

			XmlDocument envelope = new XmlDocument();
			envelope.Load(stream);
			return envelope;
		}

		private Message CloseMessage(Message message, XmlDocument envelope)
		{
			MemoryStream stream = new MemoryStream();
			XmlWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream);
			envelope.WriteTo(writer);
			writer.Flush();
			stream.Position = 0;

			XmlReader reader = XmlDictionaryReader.CreateBinaryReader(stream,
				new XmlDictionaryReaderQuotas());
			message = Message.CreateMessage(reader, int.MaxValue, message.Version);
			return message;
		}

		private class ActionComparer : IComparer<IMessageLifecyleAction>
		{
			internal static readonly ActionComparer Instance = new ActionComparer();

			public int Compare(IMessageLifecyleAction left, IMessageLifecyleAction right)
			{
				return left.ExecutionOrder - right.ExecutionOrder;
			}
		}
	}

	public class MessageLifecycleBehavior : MessageLifecycleBehavior<MessageLifecycleBehavior>
	{
	}
}
