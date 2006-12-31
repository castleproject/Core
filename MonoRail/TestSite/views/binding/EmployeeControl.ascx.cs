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

namespace AspnetSample.views
{
	using System;

	public class EmployeeControl : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txtID;
		protected System.Web.UI.WebControls.TextBox txtName;
		protected System.Web.UI.WebControls.TextBox txtCountry;

		public event EventHandler<AddEmployeeEventArgs> AddEmployee;
		public event EventHandler<DeleteEmployeeEventArgs> RemoveEmployee;

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			if (AddEmployee != null)
			{
				AddEmployee(this, new AddEmployeeEventArgs(txtName.Text, txtCountry.Text));
			}
		}

		protected void btnRemove_Click(object sender, EventArgs e)
		{
			if (RemoveEmployee != null)
			{
				RemoveEmployee(this, new DeleteEmployeeEventArgs(txtID.Text));
			}
		}

		public class AddEmployeeEventArgs : EventArgs
		{
			private string name;
			private string country;

			public AddEmployeeEventArgs(string name, string country)
			{
				this.name = name;
				this.country = country;
			}

			public string Name
			{
				get { return name; }
			}

			public string Country
			{
				get { return country; }
			}
		}

		public class DeleteEmployeeEventArgs : EventArgs
		{
			private string id;

			public DeleteEmployeeEventArgs(string id)
			{
				this.id = id;
			}

			public string Id
			{
				get { return id; }
			}
		}
	}
}
