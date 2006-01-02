// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Tests.Components
{
	using System;

	/// <summary>
	/// Summary description for Employee.
	/// </summary>
	public class Employee : IEmployee
	{
		private string _empID;
		private string _lastName;
		private string _firstName;
		private string _middleName;
		private bool _isProxy;
		private bool _isSupervisor;
		private string _email;
		private string _ntLogin;

		public Employee()
		{
		}

		public string EmployeeID
		{
			get { return _empID; }
			set { _empID = value; }
		}

		public string NTLogin
		{
			get
			{
				if (_ntLogin.Length > 0)
				{
					return _ntLogin;
				}

				try
				{
//					if (Config.IsInPortal)
//					{
//						_ntLogin = User.FindLoginIdFromEmpId(_empID);
//					}
//					else
//					{
//						_ntLogin = Config.DebugNtLogin;
//					}

					return _ntLogin;

				}
				catch (Exception)
				{
//					Logger.Error("NTLogin check failed.", e);
//					Logger.SendMail("ERROR", "NTLogin check failed.", e);
					return null;
				}
			}
		}

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		public string MiddleName
		{
			get { return _middleName; }
			set { _middleName = value; }
		}

		public bool IsProxy
		{
			get { return _isProxy; }
			set { _isProxy = value; }
		}

		public string FullName
		{
			get { return String.Format("{0} {1} {2}", _firstName, _middleName, _lastName); }
		}

		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}

		public bool IsSupervisor
		{
			get { return _isSupervisor; }
			set { _isSupervisor = value; }
		}

		public void SetNTLogin(string ntLogin)
		{
			_ntLogin = ntLogin;
		}
	}
}