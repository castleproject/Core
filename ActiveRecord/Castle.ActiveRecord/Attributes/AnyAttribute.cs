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
	/// This attribute is used to create &lt;any/&gt; assoication, a polymorphic assoication to classes that
	/// do not share a common base class.
	/// <example>
	/// Assuming we have two classes that implement IPayment, CreditCard and BankAccount, and we want a property
	/// that can point ot either one of them. We can map it like this:
	/// <code>
	/// [Any(typeof (long), MetaType=typeof (string),
	///		TypeColumn="BILLING_DETAILS_TYPE",
	///		IdColumn="BILLING_DETAILS_ID",
	///		Cascade=CascadeEnum.SaveUpdate)]
	/// [Any.MetaValue("CREDIT_CARD", typeof (CreditCard))]
	///	[Any.MetaValue("BANK_ACCOUNT", typeof (BankAccount))]
	/// public IPayment Payment { get { ... } set { ... } }
	/// </code>
	///	 The [Any] attribute specify that the id type is long, that the meta type (the type that specify the type of 
	///	 the class) is string.
	///	 The TypeColumn = "BILLING_DETAILS_TYPE" means that Active Record will look in this column to figure out what the type
	///	 of the associated entity is.
	///	 The IdColumn = "BILLING_DETAILS_ID" means that Active Record will use this column in conjuction with the type of the entity
	///	 to find the relevant entity. This is the id of the associated entity (which can point to either back account or credit card).
	///	 Cascade has the usual semantics.
	///	 
	///	 [Any.MetaValue("CREDIT_CARD", typeof (CreditCard))] - means that when Active Record encounters a "CREDIT_CARD" value in 
	///	 the "BILLING_DETAILS_TYPE", is assumes that the id in the "BILLING_DETAILS_ID" is the id of a CreditCard entity.
	///	 
	///	 [Any.MetaValue("BANK_ACCOUNT", typeof (BankAccount))] - same, just for "BANK_ACCOUNT" meaning that the id in "BILLING_DETAILS_ID"
	///	 is an id of a bank account.
	///	 </example>
	/// </summary>
	/// <remarks>
	/// This is supplied for advanced sceanrios.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class AnyAttribute : WithAccessAttribute
	{
		private CascadeEnum cascade;
		private Type idType;
		private Type metaType;
		private string typeColumn, idColumn;
		private string index;
		private bool insert = true, update = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnyAttribute"/> class.
		/// </summary>
		/// <param name="idType">Type of the id.</param>
		public AnyAttribute(Type idType)
		{
			this.idType = idType;
		}

		/// <summary>
		/// Gets or sets the type of the id.
		/// </summary>
		/// <value>The type of the id.</value>
		public Type IdType
		{
			get { return idType; }
			set { idType = value; }
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
		/// Gets or sets the cascade options
		/// </summary>
		/// <value>The cascade.</value>
		public CascadeEnum Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		/// <summary>
		/// Gets or sets the type column name
		/// </summary>
		/// <value>The type column.</value>
		public string TypeColumn
		{
			get { return typeColumn; }
			set { typeColumn = value; }
		}

		/// <summary>
		/// Gets or sets the id column name
		/// </summary>
		/// <value>The id column.</value>
		public string IdColumn
		{
			get { return idColumn; }
			set { idColumn = value; }
		}

		/// <summary>
		/// Gets or sets the index column name
		/// </summary>
		/// <value>The index.</value>
		public string Index
		{
			get { return index; }
			set { index = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column should be inserted when inserting.
		/// </summary>
		/// <value><c>true</c> if should insert; otherwise, <c>false</c>.</value>
		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column should be is updated when updating.
		/// </summary>
		/// <value><c>true</c> if should update; otherwise, <c>false</c>.</value>
		public bool Update
		{
			get { return update; }
			set { update = value; }
		}
	}

	/// <summary>
	/// Avoids the AnyAttribute.MetaValue syntax
	/// </summary>
	public class Any
	{
		/// <summary>
		/// This is used to specify a meta value in an [Any] assoication
		/// Any.MetaValue is used to connect a value (such as "CREDIT_CARD") to its type ( typeof(CreditCard) ).
		/// </summary>
		[AttributeUsage(AttributeTargets.Property, AllowMultiple=true), Serializable]
		public class MetaValueAttribute : Attribute
		{
			private string value;
			private Type clazz;

			/// <summary>
			/// Initializes a new instance of the <see cref="MetaValueAttribute"/> class.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <param name="clazz">The clazz.</param>
			public MetaValueAttribute(string value, Type clazz)
			{
				this.value = value;
				this.clazz = clazz;
			}

			/// <summary>
			/// Gets or sets the value for this class
			/// </summary>
			/// <value>The value.</value>
			public string Value
			{
				get { return value; }
				set { this.value = value; }
			}

			/// <summary>
			/// Gets or sets the class that match this value
			/// </summary>
			/// <value>The class.</value>
			public Type Class
			{
				get { return clazz; }
				set { clazz = value; }
			}
		}
	}
}