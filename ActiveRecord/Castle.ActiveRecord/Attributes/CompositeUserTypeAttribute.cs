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

namespace Castle.ActiveRecord
{
	using System;
	using NHibernate.UserTypes;

	/// <summary>
	/// Maps the property to db using a NHibernate's <see cref="ICompositeUserType"/>.
	/// </summary>
	/// <remarks>
	/// You should specify the column names and the ICompositeUserType implementor.
	/// </remarks>
	/// <example>
	/// <code>
	///		[CompositeUserType(typeof(DoubleStringType), new string[] {"Product_FirstName", "Product_LastName"})]
	///		public string[] Name
	///		{
	///			get { return name; }
	///			set { name = value; }
	///		}
	/// </code>
	/// or 
	/// <code>
	///		[CompositeUserType(
	///			typeof(DoubleStringType), 
	///			new string[]{"Manufacturer_FirstName", "Manufacturer_LastName"}, 
	///			Length = new int[] {4, 5} )]
	///		public string[] ManufacturerName
	///		{
	///			get { return manufacturerName; }
	///			set { manufacturerName = value; }
	///		}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Serializable]
	public class CompositeUserTypeAttribute : WithAccessAttribute
	{
		private Type compositeType;
		private string[] columnNames;

		private int[] length;

		private bool update = true;
		private bool insert = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeUserTypeAttribute"/> class.
		/// </summary>
		public CompositeUserTypeAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeUserTypeAttribute"/> class.
		/// </summary>
		/// <param name="compositeType">Type of the ICompositeUserType implmentor.</param>
		/// <param name="columnNames">The column names.</param>
		public CompositeUserTypeAttribute(Type compositeType, string[] columnNames)
		{
			if (!typeof(ICompositeUserType).IsAssignableFrom(compositeType))
			{
				throw new ArgumentException("The composite type not implements the ICompositeUserType", "compositeType");
			}

			this.compositeType = compositeType;
			this.columnNames = columnNames;
			length = new int[columnNames.Length];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeUserTypeAttribute"/> class.
		/// </summary>
		/// <param name="compositeTypeName">Type name of the ICompositeUserType implmentor.</param>
		/// <param name="columnNames">The column names.</param>
		public CompositeUserTypeAttribute(String compositeTypeName, string[] columnNames)
			: this(Type.GetType(compositeTypeName), columnNames)
		{
		}

		/// <summary>
		/// Gets or sets the type of the ICompositeUserType implementor.
		/// </summary>
		/// <value>The type of the composite.</value>
		public Type CompositeType
		{
			get { return compositeType; }
			set { compositeType = value; }
		}

		/// <summary>
		/// Gets or sets the column names.
		/// </summary>
		/// <value>The column names.</value>
		public string[] ColumnNames
		{
			get { return columnNames; }
			set { columnNames = value; }
		}

		/// <summary>
		/// Gets or sets the length of the columns.
		/// </summary>
		/// <value>The columns length.</value>
		public int[] Length
		{
			get { return length; }
			set { length = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this property when updating entities of this ActiveRecord class.
		/// </summary>
		public bool Update
		{
			get { return update; }
			set { update = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this property when inserting entities of this ActiveRecord class.
		/// </summary>
		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}
	}
}
