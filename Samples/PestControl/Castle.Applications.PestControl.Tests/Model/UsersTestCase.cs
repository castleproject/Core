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

	using NUnit.Framework;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for UsersTestCase.
	/// </summary>
	[TestFixture]
	public class UsersTestCase : AbstractPestControlTestCase
	{
		[Test]
		public void CreateUser()
		{
			Assert.AreEqual( 0, _model.Users.Count );

			_engine.ExecuteCommand( new CreateUserCommand("admin", "pass123", "admin@admin.com") );

			Assert.AreEqual( 1, _model.Users.Count );
		}

		[Test]
		public void Authenticate()
		{
			_engine.ExecuteCommand( new CreateUserCommand("admin", "pass1", "admin@nowhere.com") );
			_engine.ExecuteCommand( new CreateUserCommand("hammett", "pass2", "hammett@nowhere.com") );
			_engine.ExecuteCommand( new CreateUserCommand("joe", "pass3", "joe@nowhere.com") );

			Assert.IsTrue( _model.Users.Authenticate( "admin@nowhere.com", "pass1" ) );
			Assert.IsFalse( _model.Users.Authenticate( "admin@nowhere.com", "pass2" ) );
			Assert.IsTrue( _model.Users.Authenticate( "joe@nowhere.com", "pass3" ) );
		}

		[Test]
		public void FindByEmail()
		{
			_engine.ExecuteCommand( new CreateUserCommand("admin", "pass123", "admin@nowhere.com") );
			_engine.ExecuteCommand( new CreateUserCommand("hammett", "pass123", "hammett@nowhere.com") );
			_engine.ExecuteCommand( new CreateUserCommand("joe", "pass123", "joe@nowhere.com") );

			Assert.IsNotNull( _model.Users.FindByEmail("admin@nowhere.com") );
			Assert.IsNotNull( _model.Users.FindByEmail("hammett@nowhere.com") );
			Assert.IsNotNull( _model.Users.FindByEmail("joe@nowhere.com") );

			Assert.AreEqual( "admin",_model.Users.FindByEmail("admin@nowhere.com").Name );
			Assert.AreEqual( "joe",_model.Users.FindByEmail("joe@nowhere.com").Name );
			Assert.AreEqual( "hammett",_model.Users.FindByEmail("hammett@nowhere.com").Name );
		}
	}
}
