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

	[Serializable]
	public class ActiveRecordDescriptor : AbstractActiveRecordDescriptor
	{
		private String _discriminatorField;
		private String _discriminatorValue;
		private String _discriminatorType;
		private ArrayList _properties = new ArrayList();
		private ArrayList _propertiesRelations = new ArrayList();
		private ArrayList _operations = new ArrayList();
		private TableDefinition _table;

		public ActiveRecordDescriptor()
		{
		}

		public ActiveRecordDescriptor(TableDefinition table)
		{
			_table = table;

//			if (_table.RelatedDescriptor != null)
//			{
//				throw new ArgumentException("Table has an ARDescriptor already");
//			}

			_table.RelatedDescriptor = this;
		}

		public ActiveRecordDescriptor(String className)
		{
			ClassName = className;
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

		public ArrayList Properties
		{
			get { return _properties; }
		}

		public ArrayList PropertiesRelations
		{
			get { return _propertiesRelations; }
		}

		public ArrayList Operations
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

		public virtual ActiveRecordPrimaryKeyDescriptor PrimaryKeyProperty
		{
			get
			{
				foreach(ActiveRecordPropertyDescriptor prop in _properties)
				{
					if (!(prop is ActiveRecordPrimaryKeyDescriptor)) continue;
					return prop as ActiveRecordPrimaryKeyDescriptor;
				}
			
				return null;
			}
		}
	}
}
