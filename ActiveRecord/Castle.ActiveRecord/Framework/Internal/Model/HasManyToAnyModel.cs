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

	using Castle.ActiveRecord;

	/// <summary>
	/// This model represent a &lt;many-to-any/&gt; polymorphic association
	/// </summary>
	[Serializable]
	public class HasManyToAnyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly HasManyToAnyAttribute hasManyToAnyAtt;

		/// <summary>
		/// Initializes a new instance of the <see cref="HasManyToAnyModel"/> class.
		/// </summary>
		/// <param name="prop">The prop.</param>
		/// <param name="hasManyToAnyAtt">The has many to any att.</param>
		public HasManyToAnyModel(PropertyInfo prop, HasManyToAnyAttribute hasManyToAnyAtt)
		{
			this.prop = prop;
			this.hasManyToAnyAtt = hasManyToAnyAtt;
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
		/// Gets the has many to any attribute
		/// </summary>
		/// <value>The has many to any att.</value>
		public HasManyToAnyAttribute HasManyToAnyAtt
		{
			get { return hasManyToAnyAtt; }
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public Config Configuration
		{
			get { return  new Config(this); }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitHasManyToAny(this);
		}

		#endregion

		/// <summary>
		/// I need this class to pass special configuration for the many-to-any
		/// </summary>
		public class Config : IModelNode
		{
			HasManyToAnyModel parent;

			/// <summary>
			/// Gets or sets the parent model
			/// </summary>
			/// <value>The parent.</value>
			public HasManyToAnyModel Parent
			{
				get { return parent; }
				set { parent = value; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Config"/> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			internal Config(HasManyToAnyModel parent)
			{
				this.parent = parent;
			}

			#region IVisitable Members

			/// <summary>
			/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
			/// </summary>
			/// <param name="visitor">The visitor.</param>
			public void Accept(IVisitor visitor)
			{
				visitor.VisitHasManyToAnyConfig(this);
			}

			#endregion
		}
	}
}
