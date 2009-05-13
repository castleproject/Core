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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;

	/// <summary>
	/// This model is used to represent a nested value type's parent (&lt;parent /&gt; - in NHibernate talk).
	/// </summary>
	[Serializable]
	public class NestedParentReferenceModel : IVisitable
	{
		private readonly PropertyInfo propInfo;
		private readonly NestedParentReferenceAttribute nestedParentAtt;

		/// <summary>
		/// Initializes a new instance of the <see cref="NestedParentReferenceModel"/> class.
		/// </summary>
		/// <param name="propInfo">The prop info.</param>
		/// <param name="nestedParentAtt">The parent att.</param>
		public NestedParentReferenceModel(PropertyInfo propInfo, NestedParentReferenceAttribute nestedParentAtt)
		{
			this.nestedParentAtt = nestedParentAtt;
			this.propInfo = propInfo;
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return propInfo; }
		}

		/// <summary>
		/// Gets the nested attribute
		/// </summary>
		/// <value>The nested att.</value>
		public NestedParentReferenceAttribute NestedParentAtt
		{
			get { return nestedParentAtt; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitNestedParentReference(this);
		}

		#endregion
	}
}