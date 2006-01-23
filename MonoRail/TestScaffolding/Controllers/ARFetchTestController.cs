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

namespace TestScaffolding
{
	using System;
	using System.Text;

	using Castle.MonoRail.ActiveRecordSupport;

	using TestScaffolding.Model;


	public class ARFetchTestController : ARSmartDispatcherController
	{
		public void SavePerson([ARFetch("id", Create=true)] SimplePerson person, String name, Int32 age)
		{
			person.Name = name;
			person.Age = age;
			person.Save();

			RenderText(person.ToString());
		}

		public void LoadPerson([ARFetch("id")] SimplePerson person)
		{
			RenderText(person.ToString());
		}

		public void SavePeople([ARFetch("id", Create=true)] SimplePerson[] people)
		{
			StringBuilder buffer = new StringBuilder("Length=");
			buffer.Append(people.Length).Append("\n");

			foreach(SimplePerson person in people)
			{
				buffer.Append(person).Append("\n");
			}

			RenderText(buffer.ToString());
		}
	}
}
