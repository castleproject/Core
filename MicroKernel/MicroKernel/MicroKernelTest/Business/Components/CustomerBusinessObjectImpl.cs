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

namespace Castle.MicroKernel.Test.Business.Components
{
	using System;

	/// <summary>
	/// Summary description for CustomerBusinessObject.
	/// </summary>
	public class CustomerBusinessObjectImpl : AbstractBusinessObject, CustomerBusinessObject
	{
		private String m_name;
		private String m_address;

		public CustomerBusinessObjectImpl()
		{
		}

		#region CustomerBusinessObject Members

		public String Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public String Address
		{
			get
			{
				return m_address;
			}
			set
			{
				m_address = value;
			}
		}

		public bool IsAddressValid()
		{
			return false;
		}

		#endregion
	}
}
