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

namespace Castle.Facilities.ActiveRecordGenerator
{
	using System;
	using System.Windows.Forms;

	using Castle.Facilities.ActiveRecordGenerator.Forms;
	using Castle.Facilities.ActiveRecordGenerator.Components;


	public class App
	{
		[STAThread]
		public static void Main() 
		{
			ActiveRecordGeneratorContainer container = new ActiveRecordGeneratorContainer();

			container.AddComponent("newproject.form", typeof(NewProject));
			container.AddComponent("mainform.form", typeof(MainForm));

			MainForm form = (MainForm) container[ typeof(MainForm) ];

			Application.Run(form);
		}
	}
}
