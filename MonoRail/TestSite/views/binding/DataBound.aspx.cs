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

namespace AspnetSample.views
{
	using System;
	using System.Web.UI.WebControls;

	public class DataBound : System.Web.UI.Page
	{
		public System.Collections.ICollection Employees;

		protected System.Web.UI.WebControls.GridView grdEmployees;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				grdEmployees.DataBind();
			}
		}

		protected void grdEmployees_RowEditing(object sender, GridViewEditEventArgs e)
		{

		}

		protected void grdEmployees_Sorting(object sender, GridViewSortEventArgs e)
		{

		}
	}
}
