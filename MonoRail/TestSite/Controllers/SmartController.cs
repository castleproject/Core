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

namespace TestSite.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	public class SmartController : SmartDispatcherController
	{
		public SmartController()
		{
		}

		public void SimpleCall()
		{
			RenderText("SimpleCall");
		}

		public void SimpleCall(DateTime date)
		{
			RenderText("SimpleCall(date)");
		}

		public void SimpleCall(string str)
		{
			RenderText("SimpleCall(str)");
		}

		public void SimpleCall(DateTime date, string str)
		{
			RenderText("SimpleCall(date,str)");
		}

		public void SimpleCall([DataBindAttribute] Person person)
		{
			RenderText("SimpleCall(person)");
		}

		public void SimpleCall([DataBindAttribute(Prefix="Person")] Person person, string str)
		{
			RenderText("SimpleCall(person,str)");
		}

		public void SimpleCall([DataBindAttribute()] Person[] person)
		{
			RenderText("SimpleCall(person[])");
		}

		public void SimpleCall([DataBindAttribute(Prefix="Person")] Person[] person, string str)
		{
			RenderText("SimpleCall(person[],str)");
		}
	}

	public class Person
	{
		public Person()
		{			
		}

		public DateTime DOB
		{
			get { return DateTime.MinValue; }
			set {}
		}

		public String Name
		{
			get { return null; }
			set {}
		}

		public int Age
		{
			get { return 0; }
			set {}
		}
	}
}
