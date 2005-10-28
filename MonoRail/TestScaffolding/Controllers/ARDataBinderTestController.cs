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

namespace TestScaffolding
{
	using System.Text;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.ActiveRecordSupport;

	using TestScaffolding.Model;


	public class ARDataBinderTestController : ARSmartDispatchController
	{
		public ARDataBinderTestController() : base ()
		{
		}

		public void SavePersonNoPrefix([ARDataBind] SimplePerson person)
		{			
			RenderText(person.ToString());
		}
				
		public void SavePerson([ARDataBindAttribute(Prefix="SimplePerson")] SimplePerson person)
		{			
			RenderText(person.ToString());
		}

		[Rescue("DataBinderValidateBadData")]
		public void SavePersonWithValidate([ARDataBindAttribute(Prefix="SimplePerson",Validate=true)] SimplePerson person)
		{			
			RenderText(person.ToString());
		}

		public void SavePeopleNoPrefix([ARDataBindAttribute] SimplePerson[] people)
		{
			SavePeople(people);
		}
						
		public void SavePeople([ARDataBindAttribute(Prefix="SimplePerson")] SimplePerson[] people)
		{			
			StringBuilder buffer = new StringBuilder("Length=");
			buffer.Append(people.Length).Append("\n");
			foreach(SimplePerson person in people)
			{
				buffer.Append(person).Append("\n");
			}
			RenderText(buffer.ToString());
		}

		[Rescue("DataBinderValidateBadData")]
		public void AutoPersistPeople([ARDataBindAttribute(Prefix="SimplePerson",AutoPersist=true)] SimplePerson[] people)
		{			
			StringBuilder buffer = new StringBuilder("Length=");
			buffer.Append(people.Length).Append("\n");
			foreach(SimplePerson person in people)
			{
				buffer.Append(person).Append("\n");
			}
			RenderText(buffer.ToString());
		}
				
		[Rescue("DataBinderValidateBadData")]
		public void SavePeopleWithValidate([ARDataBindAttribute(Prefix="SimplePerson", Validate=true)] SimplePerson[] people)
		{	
			StringBuilder buffer = new StringBuilder("Length=");
			buffer.Append(people.Length).Append("\n");
			foreach(SimplePerson person in people)
			{
				if( person.IsValid() )					
					buffer.Append(person).Append("\n");
				else
					buffer.Append(person.Id).Append(" is not valid\n");
			}
			RenderText(buffer.ToString());
		}		
	}
}
