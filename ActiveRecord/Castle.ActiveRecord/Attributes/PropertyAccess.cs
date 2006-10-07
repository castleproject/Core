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
	/// Define the various access strategies NHibernate will use to set/get the value
	/// for this property.
	/// </summary>
	public enum PropertyAccess 
	{
		/// <summary>
		/// Use the property get/set methods to get and set the value of this property
		/// </summary>
		/// <example>
		/// <code>
		/// [Property(Access=PropertyAccess.Property)]
		/// public string UserName { get {... } set { ... } }
		/// </code>
		/// </example>
		Property,
		/// <summary>
		/// Use the field to get/set the value. (Only valid when specify on a field).
		/// </summary>
		/// <example>
		/// <code>
		/// [Property(Access=PropertyAccess.Field)]
		/// public string UserName; // notice this is a field, not property.
		/// </code>
		/// </example>
		Field,
		/// <summary>
		/// Use the field that is the backing store for this property to get/set the value of this property.
		/// The field is using the same name as the property, in camel case.
		/// </summary>
		/// <example>
		/// <code>
		/// string userName;//this will be use to get or set the value
		/// 
		/// [Property(Access=PropertyAccess.FieldCamelCase)]
		/// public string UserName { get {... } set { ... } }
		/// </code>
		/// </example>
		FieldCamelcase,
		/// <summary>
		/// Use the field that is the backing store for this property to get/set the value of this property.
		/// The field is using the same name as the property, in camel case and with an initial underscore
		/// </summary>
		/// <example>
		/// <code>
		/// string _userName;//this will be use to get or set the value
		/// 
		/// [Property(Access=PropertyAccess.FieldCamelcaseUnderscore)]
		/// public string UserName { get {... } set { ... } }
		/// </code>
		/// </example>
		FieldCamelcaseUnderscore,
		/// <summary>
		/// Use the field that is the backing store for this property to get/set the value of this property.
		/// The field is using the same name as the property, in pascal case and with an initial m and then underscore.
		/// m_Name for the property Name.
		/// </summary>
		/// <example>
		/// <code>
		/// string m_UserName;//this will be use to get or set the value
		/// 
		/// [Property(Access=PropertyAccess.FieldPascalcaseMUnderscore)]
		/// public string UserName { get {... } set { ... } }
		/// </code>
		/// </example>
		FieldPascalcaseMUnderscore,
		/// <summary>
		/// Use the field that is the backing store for this property to get/set the value of this property.
		/// The field is using the same name as the property, in all lower case and with inital underscore
		/// </summary>
		/// <example>
		/// <code>
		/// string _username;//this will be use to get or set the value
		/// 
		/// [Property(Access=PropertyAccess.FieldLowercaseUnderscore)]
		/// public string UserName { get {... } set { ... } }
		/// </code>
		/// </example>
		FieldLowercaseUnderscore,
		/// <summary>
		/// Use the property' getter to get the value, and use the field with the same name and in camel case
		/// in order to set it.
		/// </summary>
		/// <example>
		/// <code>
		/// string _userName;//this will be use to set the value
		/// 
		/// [Property(Access=PropertyAccess.NosetterCamelcase)]
		/// public string UserName { get {... } set { ... } } // this will be used just to get the value
		/// </code>
		/// </example>
		NosetterCamelcase,
		/// <summary>
		/// Use the property' getter to get the value, and use the field with the same name and in camle case
		/// with initial "_" in order to set it.
		/// </summary>
		/// <example>
		/// <code>
		/// string _userName;//this will be use to set the value
		/// 
		/// [Property(Access=PropertyAccess.NosetterPascalcaseMUndersc)]
		/// public string UserName { get {... } set { ... } } // this will be used just to get the value
		/// </code>
		/// </example>
		NosetterCamelcaseUnderscore,
		/// <summary>
		/// Use the property' getter to get the value, and use the field with the same name and in pascal case
		/// with initial "m_" in order to set it.
		/// </summary>
		/// <example>
		/// <code>
		/// string m_UserName;//this will be use to set the value
		/// 
		/// [Property(Access=PropertyAccess.NosetterPascalcaseMUndersc)]
		/// public string UserName { get {... } set { ... } } // this will be used just to get the value
		/// </code>
		/// </example>
		NosetterPascalcaseMUndersc,
		/// <summary>
		/// Use the property' getter to get the value, and use the field with the same name and in lower case
		/// in order to set it.
		/// </summary>
		/// <example>
		/// <code>
		/// string username;//this will be use to set the value
		/// 
		/// [Property(Access=PropertyAccess.NosetterPascalcaseMUndersc)]
		/// public string UserName { get {... } set { ... } } // this will be used just to get the value
		/// </code>
		/// </example>
		NosetterLowercaseUnderscore
	}
	
	/// <summary>
	/// Utility class to help convert between <see cref="PropertyAccess"/> values and
	/// NHiberante's access strategies.
	/// </summary>
	public class PropertyAccessHelper
	{
		/// <summary>
		/// Convert <param name="access"/> to its NHibernate string 
		/// </summary>
		public static string ToString(PropertyAccess access)
		{
			switch (access)
			{
				case PropertyAccess.Property:
					return "property";
				case PropertyAccess.Field:
					return "field";
				case PropertyAccess.FieldCamelcase:
					return "field.camelcase";
				case PropertyAccess.FieldCamelcaseUnderscore:
					return "field.camelcase-underscore";
				case PropertyAccess.FieldPascalcaseMUnderscore:
					return "field.pascalcase-m-underscore";
				case PropertyAccess.FieldLowercaseUnderscore:
					return "field.lowercase-underscore";
				case PropertyAccess.NosetterCamelcase:
					return "nosetter.camelcase";
				case PropertyAccess.NosetterCamelcaseUnderscore:
					return "nosetter.camelcase-underscore";
				case PropertyAccess.NosetterPascalcaseMUndersc:
					return "nosetter.pascalcase-m-underscore";
				case PropertyAccess.NosetterLowercaseUnderscore:
					return "nosetter.lowercase-underscore";
				default:
					throw new InvalidOperationException("Invalid value for PropertyAccess");
			}
		}
	}
}
