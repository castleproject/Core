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
	using System.Collections.Generic;

	/// <summary>
	/// Model for joining an additional table to Active Record class.
	/// </summary>
	public class JoinedTableModel : IVisitable
	{
		private readonly JoinedTableAttribute att;
		private readonly IList<AnyModel> anys = new List<AnyModel>();
		private readonly IList<PropertyModel> properties = new List<PropertyModel>();
		private readonly IList<FieldModel> fields = new List<FieldModel>();
		private readonly IList<BelongsToModel> belongsTo = new List<BelongsToModel>();
		private readonly IList<NestedModel> components = new List<NestedModel>();

		/// <summary>
		/// Initializes a new instance of the <see cref="JoinedTableModel"/> class.
		/// </summary>
		/// <param name="att">The att.</param>
		public JoinedTableModel(JoinedTableAttribute att)
		{
			this.att = att;
		}

		/// <summary>
		/// Gets the joined table attribute
		/// </summary>
		/// <value>The joined table att.</value>
		public JoinedTableAttribute JoinedTableAttribute
		{
			get { return att; }
		}

		/// <summary>
		/// Gets all the properties
		/// </summary>
		/// <value>The properties.</value>
		public IList<PropertyModel> Properties
		{
			get { return properties; }
		}

		/// <summary>
		/// Gets all the fields
		/// </summary>
		/// <value>The fields.</value>
		public IList<FieldModel> Fields
		{
			get { return fields; }
		}

		/// <summary>
		/// Gets the list of components.
		/// </summary>
		/// <value>The components.</value>
		public IList<NestedModel> Components
		{
			get { return components; }
		}

		/// <summary>
		/// Gets the list of [belongs to] models
		/// </summary>
		/// <value>The belongs to.</value>
		public IList<BelongsToModel> BelongsTo
		{
			get { return belongsTo; }
		}

		/// <summary>
		/// Gets the list of [any] model
		/// </summary>
		/// <value>The anys.</value>
		public IList<AnyModel> Anys
		{
			get { return anys; }
		}

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitJoinedTable(this);
		}
	}
}
