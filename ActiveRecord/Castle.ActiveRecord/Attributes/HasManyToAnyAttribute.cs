// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;

	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class HasManyToAnyAttribute : HasManyAttribute
	{
		private Type idType, metaType;
		private string idColumn, typeColumn;

		public HasManyToAnyAttribute(Type mapType, string keyColum, 
			string table, Type idType,
			string typeColumn, string idColumn) : base(mapType, keyColum, table)
		{
			this.idType = idType;
			this.typeColumn = typeColumn;
			this.idColumn = idColumn;
		}

		public string TypeColumn
		{
			get { return typeColumn; }
			set { typeColumn = value; }
		}

		public string IdColumn
		{
			get { return idColumn; }
			set { idColumn = value; }
		}

		public Type MetaType
		{
			get { return metaType; }
			set { metaType = value; }
		}

		public Type IdType
		{
			get { return idType; }
			set { idType = value; }
		}
	}
}
