using System.Runtime.Serialization;
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
	using System.Collections.Specialized;


	[Serializable]
	public class ColumnDefinitionCollection : NameObjectCollectionBase, IEnumerable
	{
		public ColumnDefinitionCollection()
		{

		}

		public ColumnDefinitionCollection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public void Add(ColumnDefinition def)
		{
			BaseAdd(def.Name, def);
		}

		public ColumnDefinition this [String name]
		{
			get { return BaseGet(name) as ColumnDefinition; }
		}

		public new IEnumerator GetEnumerator()
		{
			return base.BaseGetAllValues().GetEnumerator();
		}
	}
}
