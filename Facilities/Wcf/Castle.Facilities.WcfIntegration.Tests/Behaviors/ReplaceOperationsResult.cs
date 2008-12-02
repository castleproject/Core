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

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	using System.ServiceModel.Channels;
	using System.Xml;
	using Castle.Facilities.WcfIntegration.Behaviors;

	public class ReplaceOperationsResult : AbstractMessageBodyAction
	{
		private readonly string result;

		public ReplaceOperationsResult(string result) 
			: base(MessageLifecycle.Responses)
		{
			this.result = result;
		}

		public override bool Perform(Message message, XmlDocument body, MessageLifecycle lifecyle)
		{
			XmlNamespaceManager xmlns = new XmlNamespaceManager(body.NameTable);
			xmlns.AddNamespace("tns", "http://tempuri.org/");
			XmlNode node = body.SelectSingleNode("//tns:GetValueFromConstructorResult", xmlns);
			node.InnerText = result;
			return true;
		}
	}
}
