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

namespace Castle.MonoRail.ActiveRecordScaffold.Tests
{
	using System;
	using System.Configuration;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using TestScaffolding.Model;


	public class TestDBUtils
	{
		public static void Recreate()
		{
			ActiveRecordStarter.Initialize( 
				ConfigurationSettings.GetConfig( "activerecord" ) as IConfigurationSource, 
				typeof(Blog), typeof(Person), typeof(Customer) );

			ActiveRecordStarter.CreateSchema();

			using(new SessionScope())
			{
				for(int i=1; i <= 5; i++)
				{
					Blog blog = new Blog();
					blog.Name = "Name " + i.ToString();
					blog.Author = "Author " + i.ToString();
					blog.Save();
				}

				for(int i=1; i <= 5; i++)
				{
					Person person = new Person();
					person.Name = "Name " + i.ToString();
					person.Dob = DateTime.Now;
					person.Email = "name" + i.ToString() + "@server.com";
					person.Save();
				}
			}
		}
	}
}
