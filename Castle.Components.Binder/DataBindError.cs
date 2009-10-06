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
// 
namespace Castle.Components.Binder
{
	using System;

	/// <summary>
	/// Represents an error that occurred when trying to 
	/// databind a property of an instance.
	/// </summary>
	[Serializable]
	public class DataBindError
	{
		private readonly string parent, property, errorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBindError"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="property">The property.</param>
		public DataBindError(String parent, String property) : this(parent, property, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBindError"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="property">The property.</param>
		/// <param name="exception">The exception.</param>
		public DataBindError(String parent, String property, Exception exception) : this(parent, property, exception.Message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBindError"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="property">The property.</param>
		/// <param name="errorMessage">The error message.</param>
		public DataBindError(String parent, String property, String errorMessage)
		{
			this.parent = parent;
			this.property = property;
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public String Key
		{
			get { return parent + "." + Property; }
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public String Parent
		{
			get { return parent; }
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public String Property
		{
			get { return property; }
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>The error message.</value>
		public String ErrorMessage
		{
			get { return errorMessage; }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override String ToString()
		{
			if (errorMessage != null && errorMessage != String.Empty)
			{
				return errorMessage;
			}

			return "BindError." + Key;
		}
	}
}