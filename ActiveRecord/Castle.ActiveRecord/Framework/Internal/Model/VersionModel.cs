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
	/// Model for version property on an entity
	/// </summary>
	[Serializable]
	public class VersionModel : IVisitable
	{
		private readonly PropertyInfo prop;
		private readonly VersionAttribute att;

		/// <summary>
		/// Initializes a new instance of the <see cref="VersionModel"/> class.
		/// </summary>
		/// <param name="prop">The prop.</param>
		/// <param name="att">The att.</param>
		public VersionModel(PropertyInfo prop, VersionAttribute att)
		{
			this.prop = prop;
			this.att = att;
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
		/// Gets the version attribute
		/// </summary>
		/// <value>The version att.</value>
		public VersionAttribute VersionAtt
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
			visitor.VisitVersion(this);
		}

		#endregion
	}
}
