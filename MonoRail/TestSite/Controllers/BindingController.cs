// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace TestSite.Controllers
{
	using Castle.MonoRail.Framework;

	public class BindingController : SmartDispatcherController
	{
		public void Index()
		{
			
		}

		#region Basic Controls

		public void BasicAction()
		{
			
		}

		public void ClickMe(string selection, string text)
		{
			RenderText(string.Format("Button was clicked (\"{0}\" / \"{1}\")", selection, text));
		}

		public void SelectMe()
		{
			RenderText("List was selected");
		}

		public void ChangeMe()
		{
			RenderText("Text was changed");
		}

		public void CheckMe(bool isChecked)
		{
			RenderText(string.Format("Checkbox was {0}checked", isChecked ? "" : "un"));
		}

		public void PickMe(string name)
		{
			RenderText(string.Format("RadioButton '{0}' was checked", name));
		}

		public void Command(string commandName, object commandArg)
		{
			RenderText("Command '{0}' with arg '{1}'", commandName, commandArg);
		}

		#endregion

		#region DataBound Controls

		public void DataBound()
		{
			if (!IsPostBack)
			{
				PropertyBag["Employees"] = new Employee[]
					{
						new Employee(1, "Craig Neuwirt", "USA"),
						new Employee(2, "Hamilton Verissimo", "Brazil"),
						new Employee(3, "Ayende Rahien", "Israel")
					};
			}	
		}

		public void SelectRow(string argument)
		{
			RenderText(string.Format("Row {0} was selected", argument));
		}

		public void EditRow(string argument)
		{
			RenderText(string.Format("Row {0} was edited", argument));
		}

		public void DeleteRow(int rowIndex)
		{
			RenderText(string.Format("Row {0} was deleted", rowIndex));
		}

		public void SortRows(string argument)
		{
			RenderText(string.Format("Sort rows with expression {0}", argument));
		}


		#endregion

		#region User Controls

		public void UserControls()
		{
		}

		public void AddEmployee(string name, string country)
		{
			RenderText("Add employee {0} from {1}", name, country);
		}

		public void RemoveEmployee(int id)
		{
			RenderText("Remove employee with id {0}", id);	
		}

		#endregion
	}

	#region Employee Model

	public class Employee
	{
		private int id;
		private string name;
		private string country;

		public Employee()
		{

		}

		public Employee(int id, string name, string country)
		{
			this.id = id;
			this.name = name;
			this.country = country;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Country
		{
			get { return country; }
			set { country = value; }
		}
	}

	#endregion
}
