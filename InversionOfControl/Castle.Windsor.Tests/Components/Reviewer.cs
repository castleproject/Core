// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	/// <summary>
	/// Summary description for Reviewer.
	/// </summary>
	public class Reviewer : Employee, IReviewer
	{
		// private IIReviewableEmployeeList _employees = null;

		public Reviewer()
		{
		}

//		public IIReviewableEmployeeList Employees
//		{
//			get
//			{
//				if (_employees == null)
//				{
//					_employees = ReviewableEmployeeBuilder.Instance.GetEmployeesUnderManager(this.EmployeeID);
//				}
//				return _employees;
//			}
//			set { _employees = value; }
//		}
	}
}