// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Tests.Model.CompositeNested
{
	using System;
	using System.Collections;

	[ActiveRecord]
	public class CompositeNestedParent : ActiveRecordBase
	{
		private int id;
		private IList dependents;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasMany(typeof(Dependent), "Parent_Id", "CompositeNestedParent_Dependent" , DependentObjects = true)]
		public IList Dependents
		{
			get { return dependents; }
			set { dependents = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(CompositeNestedParent));
		}

		public static CompositeNestedParent[] FindAll()
		{
			return (CompositeNestedParent[])FindAll(typeof(CompositeNestedParent));
		}

		public static CompositeNestedParent Find(int id)
		{
			return (CompositeNestedParent)FindByPrimaryKey(typeof(CompositeNestedParent), id);
		}
	}
	

	public class Dependent
	{
		private DateTime dateProp;
		private NestedDependent nestedDependentProp;
		
		[Nested]
		public NestedDependent NestedDependentProp
		{
			get { return nestedDependentProp; }
			set { nestedDependentProp = value; }
		}

		[Property]
		public DateTime DateProp
		{
			get { return dateProp; }
			set { dateProp = value; }
		}
	}

	public class NestedDependent
	{
		private int intProp;
		private InnerNestedDependant innerNestedDependantProp;

		[Property]
		public int IntProp
		{
			get { return intProp; }
			set { intProp = value; }
		}

		[Nested]
		public InnerNestedDependant InnerNestedDependantProp
		{
			get { return innerNestedDependantProp;}
			set
			{
				innerNestedDependantProp = value;
			}
		}
	}

	public class InnerNestedDependant
	{
		private string stringProp;

		[Property]
		public string StringProp
		{
			get { return stringProp; }
			set { stringProp = value; }
		}
	}
}
