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
	using System.Reflection;

	/// <summary>
	/// Model for a persitent property that uses a field to get/set the values.
	/// </summary>
	[Serializable]
	public class FieldModel : IModelNode
	{
		private readonly FieldInfo field;
		private readonly FieldAttribute att;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldModel"/> class.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="att">The att.</param>
		public FieldModel(FieldInfo field, FieldAttribute att)
		{
			this.field = field;
			this.att = att;
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		/// <value>The field.</value>
		public FieldInfo Field
		{
			get { return field; }
		}

		/// <summary>
		/// Gets the field attribute
		/// </summary>
		/// <value>The field att.</value>
		public FieldAttribute FieldAtt
		{
			get { return att; }
		}

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitField(this);
		}
	}
}
