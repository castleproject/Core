// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;


	/// <summary>
	/// Model for a simple persistent property
	/// </summary>
	[Serializable]
	public class PropertyModel : IVisitable
	{
		private readonly PropertyInfo prop;
		private readonly PropertyAttribute att;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyModel"/> class.
		/// </summary>
		protected PropertyModel() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyModel"/> class.
		/// </summary>
		/// <param name="prop">The prop.</param>
		/// <param name="att">The att.</param>
		public PropertyModel(PropertyInfo prop, PropertyAttribute att)
		{
			this.prop = prop;
			this.att = att;
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public virtual PropertyInfo Property
		{
			get { return prop; }
		}

		/// <summary>
		/// Gets the property attribute
		/// </summary>
		/// <value>The property att.</value>
		public virtual PropertyAttribute PropertyAtt
		{
			get { return att; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitProperty(this);
		}

		#endregion
	}
}
