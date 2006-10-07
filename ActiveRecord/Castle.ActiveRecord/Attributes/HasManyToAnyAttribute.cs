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

	/// <summary>
	/// This attribute allows polymorphic association between classes that doesn't have a common root class.
	/// In require two columns that would tell it what is the type of the asssoicated entity, and what is the PK of that entity.
	/// <remarks>
	/// This is supplied for advanced sceanrios.
	/// </remarks>
	/// <example>
	/// For instnace, let assume that you have two classes (that implement a common interface, but have no base classs) called:
	///  - Back Account
	///  - Credit Card
	/// 
	/// And you have a set of Payment methods, that can be either. You would define the mapping so:
	/// <code>
	/// [HasManyToAny(typeof(IPayment), "pay_id", "payments_table", typeof(int), "payment_type", "payment_method_id",
	///		MetaType = typeof(int), RelationType = RelationType.Set)]
	/// </code>
	/// typeof(IPayement) - the common interface tha both classes implement, and the type of all the items in the set.
	/// "pay_id" - the column that hold the PK of this entity (the FK column)
	/// "payments_table" - the table that has the assoication information (in 1:M scenarios - usuaully the same table, in M:N scenarios the link table).
	/// typeof(int) - the type of id column 
	/// "payment_type" - the column used to find out which class is represented by this row.
	/// "payment_method_id" - the column that holds the PK of the assoicated class (either CreditCard or BankAccount).
	/// MetaType = typeof(int) - the type of the meta column (payment_type)
	/// RelationType = RelationType.Set - specify that we use a set here
	/// </example>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class HasManyToAnyAttribute : HasManyAttribute
	{
		private Type idType, metaType;
		private string idColumn, typeColumn;

		/// <summary>
		/// Initializes a new instance of the <see cref="HasManyToAnyAttribute"/> class.
		/// </summary>
		/// <param name="mapType">Type of the map.</param>
		/// <param name="keyColum">The key colum.</param>
		/// <param name="table">The table.</param>
		/// <param name="idType">Type of the id.</param>
		/// <param name="typeColumn">The type column.</param>
		/// <param name="idColumn">The id column.</param>
		public HasManyToAnyAttribute(Type mapType, string keyColum, 
			string table, Type idType,
			string typeColumn, string idColumn) : base(mapType, keyColum, table)
		{
			this.idType = idType;
			this.typeColumn = typeColumn;
			this.idColumn = idColumn;
		}

		/// <summary>
		/// Gets or sets the type column.
		/// </summary>
		/// <value>The type column.</value>
		public string TypeColumn
		{
			get { return typeColumn; }
			set { typeColumn = value; }
		}

		/// <summary>
		/// Gets or sets the id column.
		/// </summary>
		/// <value>The id column.</value>
		public string IdColumn
		{
			get { return idColumn; }
			set { idColumn = value; }
		}

		/// <summary>
		/// Gets or sets the type of the meta column
		/// </summary>
		/// <value>The type of the meta.</value>
		public Type MetaType
		{
			get { return metaType; }
			set { metaType = value; }
		}

		/// <summary>
		/// Gets or sets the type of the id column
		/// </summary>
		/// <value>The type of the id.</value>
		public Type IdType
		{
			get { return idType; }
			set { idType = value; }
		}
	}
}
