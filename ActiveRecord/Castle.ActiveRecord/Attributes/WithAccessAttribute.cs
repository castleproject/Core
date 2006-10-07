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
	/// Base class that allows specifying an access strategy to get/set the value for an object' property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public abstract class WithAccessAttribute : Attribute
	{
		private PropertyAccess access = PropertyAccess.Property;
		private string customAccess = null;

		/// <summary>
		/// Gets or sets the access strategy for this property
		/// </summary>
		public PropertyAccess Access
		{
			get { return access; }
			set { access = value; }
		}

		/// <summary>
		/// Gets or sets the custom access strategy
		/// </summary>
		/// <value>The custom access.</value>
		public string CustomAccess
		{
			get { return customAccess;}
			set { customAccess=value;}
		}

		/// <summary>
		/// Gets the access strategy string for NHibernate's mapping.
		/// </summary>
		/// <value>The access string.</value>
		public string AccessString
		{
			get
			{
				if (CustomAccess != null)
				{
					return CustomAccess;
				}
				return PropertyAccessHelper.ToString(Access);
			}
		}
	}
}
