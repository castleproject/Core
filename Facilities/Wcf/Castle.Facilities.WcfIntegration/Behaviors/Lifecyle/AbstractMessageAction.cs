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
	using System.Collections;
	using System.ServiceModel.Channels;

	public abstract class AbstractMessageAction<T> : AbstractExtension<T>, IMessageAction
		where T : MessageLifecycleBehavior<T>
	{
		private readonly MessageLifecycle lifecycle;

		protected AbstractMessageAction(MessageLifecycle lifecycle)
		{
			this.lifecycle = lifecycle;
		}

		public MessageLifecycle Lifecycle
		{
			get { return lifecycle; }
		}

		public virtual bool ShouldPerform(MessageLifecycle lifecycle)
		{
			return (lifecycle & this.lifecycle) > 0;			
		}

		public abstract bool Perform(ref Message message, MessageLifecycle lifecycle,
									 IDictionary state);
	}

	public abstract class AbstractMessageAction : AbstractMessageAction<MessageLifecycleBehavior>
	{
		protected AbstractMessageAction(MessageLifecycle lifecycle)
			: base(lifecycle)
		{
		}
	}
}
