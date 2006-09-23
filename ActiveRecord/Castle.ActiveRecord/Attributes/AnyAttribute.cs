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

	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class AnyAttribute : WithAccessAttribute
	{		
		private CascadeEnum cascade;
		private Type idType;
		private Type metaType;
		private string typeColumn, idColumn;
		private string index;
		private bool insert = true, update = true;

		public AnyAttribute(Type idType)
		{
			this.idType = idType;
		}

		public Type IdType
		{
			get { return idType;}
			set { idType = value;}
		}

		public Type MetaType
		{
			get { return metaType;}
			set { metaType = value;}
		}

		public CascadeEnum Cascade
		{
			get { return cascade;}
			set { cascade = value;}
		}

		public string TypeColumn
		{
			get { return typeColumn;}
			set { typeColumn = value;}
		}

		public string IdColumn
		{
			get { return idColumn;}
			set { idColumn = value;}
		}

		public string Index
		{
			get { return index;}
			set { index = value;}
		}

		public bool Insert
		{
			get { return insert;}
			set { insert = value;}
		}

		public bool Update
		{
			get { return update;}
			set { update = value;}
		}
	}

	/// <summary>
	/// Avoids the AnyAttribute.MetaValue problem
	/// </summary>
	public class Any
	{
		[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
		public class MetaValueAttribute : Attribute
		{
			private string value;
			private Type clazz;

			public MetaValueAttribute(string value, Type clazz)
			{
				this.value = value;
				this.clazz = clazz;
			}

			public string Value
			{
				get { return value;}
				set { value = value;}
			}

			public Type Class
			{
				get { return clazz;}
				set { clazz = value;}
			}
		}
	}
}
