// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Test.Components
{
	using System;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Summary description for AvalonSpamService3.
	/// </summary>
	[AvalonComponent("spamservice2", Lifestyle.Transient)]
	[AvalonService( typeof(ISpamService2) )]
	public class AvalonSpamService3 : IInitializable, ISpamService2, IDisposable
	{
		public IMailService m_mailService;

		public bool disposed = false;

		public AvalonSpamService3()
		{
		}

		#region IInitializable Members

		public void Initialize()
		{
		}

		#endregion

		#region ISpamService2 Members

		public IMailService MailService
		{
			get
			{
				return m_mailService;
			}
			set
			{
				m_mailService = value;
			}
		}

		#endregion

		#region ISpamService Members

		public void AnnoyPeople(String contents)
		{
			if (m_mailService == null)
			{
				throw new Exception("Dependency not satisfied.");
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			disposed = true;
		}

		#endregion
	}
}
