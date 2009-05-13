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

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	using System.Collections;
	using System.ServiceModel.Channels;
	using Castle.Facilities.WcfIntegration.Behaviors;

	public class AddOperationsHeader : AbstractMessageAction
	{
		private readonly string name;
		private readonly object value;

		public AddOperationsHeader(string name, object value) 
			: base(MessageLifecycle.All)
		{
			this.name = name;
			this.value = value;
		}

		public override bool Perform(ref Message message, MessageLifecycle lifecyle, IDictionary state)
		{
			MessageHeader header = MessageHeader.CreateHeader(name, "", value, false);
			message.Headers.Add(header);
			return true;
		}
	}
}
