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
	using System.Text;

	using Castle.MonoRail.ActiveRecordSupport;

	using TestScaffolding.Model;

	public class ARDataBinderTestController : ARSmartDispatcherController
	{
		public void SavePerson([ARDataBindAttribute("SimplePerson", AutoLoad=AutoLoadBehavior.Always)] SimplePerson person)
		{			
			RenderText(person.ToString());
		}

		public void SavePersonNull([ARDataBindAttribute("SimplePerson", AutoLoad=AutoLoadBehavior.NullIfInvalidKey)] SimplePerson person)
		{			
			if (person == null)
				RenderText("null");
			else
				RenderText(person.ToString());
		}

		public void SavePersonNew([ARDataBindAttribute("SimplePerson", AutoLoad=AutoLoadBehavior.NewInstanceIfInvalidKey)] SimplePerson person)
		{			
			if (person == null)
				RenderText("null");
			else
				RenderText(person.ToString());
		}

		public void SavePersonNever([ARDataBindAttribute("SimplePerson", AutoLoad=AutoLoadBehavior.Never)] SimplePerson person)
		{			
			if (person == null)
				RenderText("null");
			else
				RenderText(person.ToString());
		}

		public void SavePeople([ARDataBindAttribute("SimplePerson")] SimplePerson[] people)
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
