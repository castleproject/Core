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

namespace GettingStartedPart1
{
	using System;
	using System.Windows.Forms;
	using Castle.Core.Resource;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class App
	{
		public static void Main()
		{
			IWindsorContainer container = 
				new WindsorContainer(
					new XmlInterpreter(new ConfigResource("castle")));
			
			
			// Request the component to use it
			Form1 form = (Form1) container[typeof(Form1)];
			
			// Use the component
			Application.Run(form);
			
			// Release it
			container.Release(form);
		}
	}
}
