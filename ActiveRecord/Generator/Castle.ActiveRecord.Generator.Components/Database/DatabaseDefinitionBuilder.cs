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

namespace Castle.ActiveRecord.Generator.Components.Database
{
	using System;
	using System.Data;
	using System.Data.OleDb;


	public class DatabaseDefinitionBuilder : IDatabaseDefinitionBuilder
	{
		private IConnectionFactory _connectionFactory;

		public DatabaseDefinitionBuilder(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public DatabaseDefinition Build(String alias, String connectionString)
		{
			DatabaseDefinition def = new DatabaseDefinition(alias);

			using(OleDbConnection connection = _connectionFactory.CreateConnection(connectionString))
			{
				BuildTables(connection, def);
				BuildPks(connection, def);
				BuildFks(connection, def);
			}

			return def;
		}

		private void BuildPks(OleDbConnection connection, DatabaseDefinition def)
		{
			object[] objArrRestrict = new object[] {null, null, null, null};
			DataTable indexes = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, objArrRestrict);

			foreach(DataRow index in indexes.Rows)
			{
				TableDefinition tableDef = def.Tables[ (String) index["TABLE_NAME"] ];
				
				if (tableDef == null) continue;

				ColumnDefinition colDef = tableDef.Columns[ (String) index["COLUMN_NAME"] ];

				colDef.PrimaryKey = (bool) index["PRIMARY_KEY"];
				colDef.Unique = (bool) index["UNIQUE"];
			}
		}

		private void BuildFks(OleDbConnection connection, DatabaseDefinition def)
		{
			object[] objArrRestrict = new object[] {null, null, null, null};
			DataTable indexes = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, objArrRestrict);

			foreach(DataRow index in indexes.Rows)
			{
				TableDefinition fkTableDef = def.Tables[ (String) index["FK_TABLE_NAME"] ];
				TableDefinition pkTableDef = def.Tables[ (String) index["PK_TABLE_NAME"] ];
				
				if (fkTableDef == null || pkTableDef == null) continue;

				ColumnDefinition colDef = fkTableDef.Columns[ (String) index["FK_COLUMN_NAME"] ];

				colDef.ForeignKey = true;
				colDef.RelatedTable = pkTableDef;
				
				pkTableDef.AddManyRelation(fkTableDef);
			}
		}

		private void BuildTables(OleDbConnection connection, DatabaseDefinition def)
		{
			object[] objArrRestrict = new object[] {null, null, null, "TABLE"};
			DataTable tables = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, objArrRestrict);
	
			foreach(DataRow table in tables.Rows)
			{
				TableDefinition tableDef = 
					def.AddTable( new TableDefinition( (String) table["TABLE_NAME"] ) );

				objArrRestrict = new object[] {null, null, tableDef.Name, null};

				DataTable columns = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, objArrRestrict);

				foreach(DataRow column in columns.Rows)
				{
					ColumnDefinition colDef = tableDef.AddColumn( new ColumnDefinition( (String) column["COLUMN_NAME"] ) );
					colDef.Nullable = (bool) column["IS_NULLABLE"];
					colDef.Type = (OleDbType) Enum.Parse( typeof(OleDbType), column["DATA_TYPE"].ToString() );
				}
			}
		}
	}
}
