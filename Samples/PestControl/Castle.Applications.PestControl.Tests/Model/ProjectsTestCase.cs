// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Applications.PestControl.Tests.Model
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for UsersTestCase.
	/// </summary>
	[TestFixture]
	public class ProjectsTestCase : AbstractPestControlTestCase
	{
		[Test]
		public void CreateProject()
		{
			Assert.AreEqual( 0, _model.Projects.Count );

			User user = (User) _engine.ExecuteCommand( 
				new CreateUserCommand("admin", "pass123", "admin@admin.com") );

			_engine.ExecuteCommand( 
				new CreateProjectCommand(false, "Castle", "svnsc", "nant", 
					"admin@admin.com", new Hashtable()) );

			Assert.AreEqual( 1, _model.Projects.Count );
		}
	}
}
