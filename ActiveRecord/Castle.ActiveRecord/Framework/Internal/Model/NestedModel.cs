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
	/// This model is used to represent a nested value type (&lt;component/&gt; - in NHibernate talk).
	/// </summary>
	[Serializable]
	public class NestedModel : IVisitable
	{
		private readonly PropertyInfo propInfo;
		private readonly NestedAttribute nestedAtt;
		private readonly ActiveRecordModel nestedModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="NestedModel"/> class.
		/// </summary>
		/// <param name="propInfo">The prop info.</param>
		/// <param name="nestedAtt">The nested att.</param>
		/// <param name="nestedModel">The nested model.</param>
		public NestedModel( PropertyInfo propInfo, NestedAttribute nestedAtt, ActiveRecordModel nestedModel )
		{
			this.nestedAtt = nestedAtt;
			this.nestedModel = nestedModel;
			this.propInfo = propInfo;
		}

		/// <summary>
		/// Gets the model.
		/// </summary>
		/// <value>The model.</value>
		public ActiveRecordModel Model
		{
			get { return nestedModel; }
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
		public NestedAttribute NestedAtt
		{
			get { return nestedAtt; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitNested(this);
		}

		#endregion
	}
}
