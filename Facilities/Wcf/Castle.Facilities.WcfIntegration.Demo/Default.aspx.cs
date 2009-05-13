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

namespace Castle.Facilities.WcfIntegration.Demo
{
	using System;
	using System.Web.UI;
	using UsingWindsorSvc;

	public partial class Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			AmUsingWindsorClient usingWindsorClient = new AmUsingWindsorClient();
			ValueFromService.Text = usingWindsorClient.GetValueFromWindsorConfig().ToString();
		}
	}
}
