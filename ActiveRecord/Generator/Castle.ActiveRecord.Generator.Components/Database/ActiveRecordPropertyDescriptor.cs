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

	public abstract class ActiveRecordPropertyDescriptor
	{
		private String _columnName;
		private String _columnTypeName = "VARCHAR";
		private String _propertyName;
		private Type _propertyType;
		private bool _generate = true;

		public ActiveRecordPropertyDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, /*String propertyFieldName,*/ Type propertyType)
		{
			_columnName = columnName;
			_columnTypeName = columnTypeName;
			_propertyName = propertyName;
			_propertyType = propertyType;
		}

		public ActiveRecordPropertyDescriptor(string _columnName, string _columnTypeName, string _propertyName)
		{
			this._columnName = _columnName;
			this._columnTypeName = _columnTypeName;
			this._propertyName = _propertyName;
		}

		public string ColumnName
		{
			get { return _columnName; }
			set { _columnName = value; }
		}

		public string ColumnTypeName
		{
			get { return _columnTypeName; }
			set { _columnTypeName = value; }
		}

		public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

//		public string PropertyFieldName
//		{
//			get { return _propertyFieldName; }
//			set { _propertyFieldName = value; }
//		}

		public Type PropertyType
		{
			get { return _propertyType; }
			set { _propertyType = value; }
		}

		public bool Generate
		{
			get { return _generate; }
			set { _generate = value; }
		}
	}

	public class ActiveRecordPrimaryKeyDescriptor : ActiveRecordPropertyDescriptor
	{
		private String _generatorType;

		public ActiveRecordPrimaryKeyDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, Type propertyType, String _generatorType) : 
			base(columnName, columnTypeName, propertyName, propertyType)
		{
			this._generatorType = _generatorType;
		}
	}

	public class ActiveRecordFieldDescriptor : ActiveRecordPropertyDescriptor
	{
		private bool _nullable = false;

		public ActiveRecordFieldDescriptor(
			String columnName, String columnTypeName, 
			String propertyName, Type propertyType, bool _nullable) : 
			base(columnName, columnTypeName, propertyName, propertyType)
		{
			this._nullable = _nullable;
		}
	}

	public abstract class ActiveRecordPropertyRelationDescriptor : ActiveRecordPropertyDescriptor
	{
		private String _relationType;
		private ActiveRecordDescriptor _targetType;

		public ActiveRecordPropertyRelationDescriptor(string _columnName, string _columnTypeName, 
			string _propertyName, string _relationType, ActiveRecordDescriptor _targetType) : 
			base(_columnName, _columnTypeName, _propertyName)
		{
			this._relationType = _relationType;
			this._targetType = _targetType;
		}

		public string RelationType
		{
			get { return _relationType; }
		}

		public ActiveRecordDescriptor TargetType
		{
			get { return _targetType; }
		}
	}

	public class ActiveRecordBelongsToDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordBelongsToDescriptor(string _columnName, 
			string _propertyName, ActiveRecordDescriptor _targetType) : 
			base(_columnName, "", _propertyName, "BelongsTo", _targetType)
		{
		}
	}

	public class ActiveRecordHasManyDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordHasManyDescriptor(string _columnName, 
			string _propertyName, Type propertyName, ActiveRecordDescriptor _targetType) : 
			base(_columnName, "", _propertyName, "HasMany", _targetType)
		{
			base.PropertyType = propertyName;
		}
	}

	public class ActiveRecordHasOneDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		public ActiveRecordHasOneDescriptor(string _columnName, 
			string _propertyName, ActiveRecordDescriptor _targetType) : 
			base(_columnName, "", _propertyName, "HasOne", _targetType)
		{
		}
	}

	public class ActiveRecordHasAndBelongsToManyDescriptor : ActiveRecordPropertyRelationDescriptor
	{
		private String _columnKey;

		public ActiveRecordHasAndBelongsToManyDescriptor(string _columnName, string _columnTypeName, 
			string _propertyName, ActiveRecordDescriptor _targetType, string _columnKey) : 
			base(_columnName, _columnTypeName, _propertyName, "HasAndBelongsToMany", _targetType)
		{
			this._columnKey = _columnKey;
		}
	}
}
