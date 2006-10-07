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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Reflection;

	using Castle.ActiveRecord;

	/// <summary>
	/// Model for [Any] association, a polymorphic assoication without common base class
	/// </summary>
	[Serializable]
	public class AnyModel : IVisitable
	{
		private readonly PropertyInfo prop;
		private readonly AnyAttribute anyAtt;
		private IList metaValues;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnyModel"/> class.
		/// </summary>
		/// <param name="prop">The prop.</param>
		/// <param name="anyAtt">Any att.</param>
		public AnyModel(PropertyInfo prop, AnyAttribute anyAtt)
		{
			this.prop = prop;
			this.anyAtt = anyAtt;
			metaValues = new ArrayList();
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return prop; }
		}

		/// <summary>
		/// Gets the [Any] attribute
		/// </summary>
		/// <value>Any att.</value>
		public AnyAttribute AnyAtt
		{
			get { return anyAtt; }
		}

		/// <summary>
		/// Gets or sets the meta values.
		/// </summary>
		/// <value>The meta values.</value>
		public IList MetaValues
		{
			get { return metaValues; }
			set { metaValues = value; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitAny(this);
		}

		#endregion

	}
}
