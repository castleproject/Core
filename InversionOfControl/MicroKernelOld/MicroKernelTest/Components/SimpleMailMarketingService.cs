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

	/// <summary>
	/// Summary description for SimpleMailMarketingService.
	/// </summary>
	public class SimpleMailMarketingService : IMailMarketingService
	{
		public IMailService m_mailService;
		public ICustomerManager m_customerManager;

		public SimpleMailMarketingService()
		{
		}

		#region IMailMarketingService Members

		public ICustomerManager CustomerManager
		{
			get
			{
				return m_customerManager;
			}
			set
			{
				m_customerManager = value;
			}
		}

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

		public void AnnoyMillionsOfPeople(String message)
		{
			if (MailService == null)
			{
				throw new NullReferenceException("IMailService wasn't supplied.");
			}
			if (CustomerManager == null)
			{
				throw new NullReferenceException("ICustomerManager wasn't supplied.");
			}
		}

		#endregion
	}
}
