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
	using System.Collections;


	public class ActiveRecordDescriptor
	{
		private String _className;
		private String _discriminatorField;
		private String _discriminatorValue;
		private String _discriminatorType;
		private IList _properties = new ArrayList();
		private IList _propertiesRelations = new ArrayList();
		private IList _operations = new ArrayList();
		private TableDefinition _table;

		public ActiveRecordDescriptor()
		{
		}

		public ActiveRecordDescriptor(TableDefinition table)
		{
			_table = table;

			if (_table.RelatedDescriptor != null)
			{
				throw new ArgumentException("Table has an ARDescriptor already");
			}

			_table.RelatedDescriptor = this;
		}

		public ActiveRecordDescriptor(String className)
		{
			_className = className;
		}

		public String ClassName
		{
			get { return _className; }
			set { _className = value; }
		}

		public String DiscriminatorField
		{
			get { return _discriminatorField; }
			set { _discriminatorField = value; }
		}

		public String DiscriminatorValue
		{
			get { return _discriminatorValue; }
			set { _discriminatorValue = value; }
		}

		public String DiscriminatorType
		{
			get { return _discriminatorType; }
			set { _discriminatorType = value; }
		}

		public IList Properties
		{
			get { return _properties; }
		}

		public IList PropertiesRelations
		{
			get { return _propertiesRelations; }
		}

		public IList Operations
		{
			get { return _operations; }
		}

		public TableDefinition Table
		{
			get { return _table; }
			set { _table = value; }
		}

		public void AddProperty( ActiveRecordPropertyDescriptor propertyDescriptor )
		{
			_properties.Add(propertyDescriptor);
		}
	}
}
