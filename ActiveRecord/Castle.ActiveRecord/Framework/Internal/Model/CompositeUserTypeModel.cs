// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	/// <summary>
	/// Model for representing a Composite User type map.
	/// </summary>
	public class CompositeUserTypeModel : IVisitable
	{
		private readonly PropertyInfo prop;
		private readonly CompositeUserTypeAttribute attribute;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeUserTypeModel"/> class.
		/// </summary>
		/// <param name="prop">The property marked with the attribute.</param>
		/// <param name="attribute">The metadata attribute.</param>
		public CompositeUserTypeModel(PropertyInfo prop, CompositeUserTypeAttribute attribute)
		{
			this.prop = prop;
			this.attribute = attribute;
		}

		/// <summary>
		/// Gets the property marked with the attribute.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return prop; }
		}

		/// <summary>
		/// Gets the attribute instance.
		/// </summary>
		/// <value>The attribute.</value>
		public CompositeUserTypeAttribute Attribute
		{
			get { return attribute; }
		}

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitCompositeUserType(this);
		}
	}
}
