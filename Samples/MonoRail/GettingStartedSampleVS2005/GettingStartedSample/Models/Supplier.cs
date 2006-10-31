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

namespace GettingStartedSample.Models
{
	using System;
	using Castle.ActiveRecord;
	using NHibernate.Expression;

	[ActiveRecord]
	public class Supplier : ActiveRecordBase
	{
		private int id;
		private String name;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		/// <summary>
		/// Returns the Suppliers ordered by Name
		/// </summary>
		/// <returns>Suppliers array</returns>
		public static Supplier[] FindAll()
		{
			return (Supplier[]) FindAll(typeof(Supplier), new Order[] { Order.Asc("Name") });
		}
	}
}
