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

namespace Castle.Facilities.ActiveRecordGenerator.Components.Database.Default
{
	using System;
	using System.Data;
	using System.Data.OleDb;
	
	using Castle.Facilities.ActiveRecordGenerator.Model;

	public class ConnectionFactory : IConnectionFactory
	{
		public OleDbConnection CreateConnection(Project project)
		{
//			Type driverType = Type.GetType(project.Driver, false, false);
//
//			if (driverType == null)
//			{
//				throw new ApplicationException("Could not load driver type");
//			}

			try
			{
				OleDbConnection conn = new OleDbConnection(project.ConnectionString);
				conn.Open();
				return conn;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
	}
}
