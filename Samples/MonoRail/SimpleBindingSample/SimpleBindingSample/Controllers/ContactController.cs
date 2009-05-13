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

namespace SimpleBindingSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;


	public class ContactController : SmartDispatcherController
	{
		/// <summary>
		/// Renders some sample forms
		/// </summary>
		public void Index()
		{
		}
		
		public void PostMessage(String name, String email, int age, String countryCode)
		{
			// We add the values to 
			// the PropertyBag so we can use it on the 'PostMessage' view
			
			// This is a good practice, but for this case it is not
			// required as NVelocity view have access to query/post entries
			
			PropertyBag["name"] = name;
			PropertyBag["email"] = email;
			PropertyBag["age"] = age;
			PropertyBag["country"] = countryCode;
		}
		
		public void PostMessages(String[] name)
		{
			PropertyBag["name"] = name;
		}
	}
}
