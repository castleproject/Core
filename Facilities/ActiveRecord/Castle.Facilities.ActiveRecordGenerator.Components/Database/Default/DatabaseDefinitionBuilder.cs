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
	using Castle.Facilities.ActiveRecordGenerator.Database;


	public class DatabaseDefinitionBuilder : IDatabaseDefinitionBuilder
	{
		private IConnectionFactory _connectionFactory;

		public DatabaseDefinitionBuilder(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public DatabaseDefinition Build(Project project)
		{
			DatabaseDefinition def = new DatabaseDefinition();

			using(OleDbConnection connection = _connectionFactory.CreateConnection(project))
			{
//				object[] objArrRestrict = new object[] {null, null, null, "TABLE_CATALOG"};
//				DataTable catalogs = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Catalogs, objArrRestrict);

				object[] objArrRestrict = new object[] {null, null, null, "TABLE"};
				DataTable tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, objArrRestrict);

				foreach(DataRow table in tables.Rows)
				{
					TableDefinition tableDef = 
						def.AddTable( new TableDefinition( (String) table["TABLE_NAME"] ) );

					objArrRestrict = new object[] {null, null, tableDef.Name, null};

					DataTable columns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, objArrRestrict);
//					DataTable pks = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, objArrRestrict);
//					DataTable fks = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, objArrRestrict);

					foreach(DataRow column in columns.Rows)
					{
						tableDef.AddColumn( 
							new ColumnDefinition( (String) column["COLUMN_NAME"] ) ); 

					}
				}
			}

			return def;
		}
	}
}
