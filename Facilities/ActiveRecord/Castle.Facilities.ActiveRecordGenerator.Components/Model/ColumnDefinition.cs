using System.Data.OleDb;
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

namespace Castle.Facilities.ActiveRecordGenerator.Model
{
	using System;


	public class ColumnDefinition
	{
		private String _name;
		private bool _primaryKey;
		private bool _foreignKey;
		private bool _unique;
		private bool _nullable;
		private TableDefinition _relatedTable;
		private OleDbType _type = OleDbType.Empty;

		public ColumnDefinition( String name )
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}

		public bool PrimaryKey
		{
			get { return _primaryKey; }
			set { _primaryKey = value; }
		}

		public bool ForeignKey
		{
			get { return _foreignKey; }
			set { _foreignKey = value; }
		}

		public bool Unique
		{
			get { return _unique; }
			set { _unique = value; }
		}

		public TableDefinition RelatedTable
		{
			get { return _relatedTable; }
			set { _relatedTable = value; }
		}

		public bool Nullable
		{
			get { return _nullable; }
			set { _nullable = value; }
		}

		public OleDbType Type
		{
			get { return _type; }
			set { _type = value; }
		}
	}
}
