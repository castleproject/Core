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

namespace Castle.MicroKernel.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class DependencyResolvingTestCase
	{
		[Test]
		public void ContainerShouldUseFirstRegisteredDependencyOfTypeByDefault_SmsRegisteredFirst()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddComponent("sms", typeof(IAlarmSender), typeof(SmsSender));
			kernel.AddComponent("email", typeof(IAlarmSender), typeof(EmailSender));

			kernel.AddComponent("generator", typeof(AlarmGenerator));

			AlarmGenerator gen = (AlarmGenerator) kernel["generator"];
			Assert.AreEqual(typeof(SmsSender), gen.Sender.GetType());

		}

		[Test]
		public void ContainerShouldUseFirstRegisteredDependencyOfTypeByDefault_EmailRegisteredFirst()
		{
			IKernel kernel = new DefaultKernel();
			kernel.AddComponent("email", typeof(IAlarmSender), typeof(EmailSender));
			kernel.AddComponent("sms", typeof(IAlarmSender), typeof(SmsSender));

			kernel.AddComponent("generator", typeof(AlarmGenerator));

			AlarmGenerator gen = (AlarmGenerator)kernel["generator"];
			Assert.AreEqual(typeof(EmailSender), gen.Sender.GetType());
		}
	}

	public interface IAlarmSender
	{
		
	}

	public class EmailSender : IAlarmSender{}
	public class SmsSender : IAlarmSender { }

	public class AlarmGenerator
	{
		private readonly IAlarmSender sender;

		public IAlarmSender Sender
		{
			get { return sender; }
		}

		public AlarmGenerator(IAlarmSender sender)
		{
			this.sender = sender;
		}
	}
}