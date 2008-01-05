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
	/// Model for a Primary Key
	/// </summary>
	[Serializable]
	public class PrimaryKeyModel : IVisitable
	{
		private readonly PropertyInfo propInfo;
		private readonly PrimaryKeyAttribute pkAtt;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimaryKeyModel"/> class.
		/// </summary>
		/// <param name="propInfo">The prop info.</param>
		/// <param name="pkAtt">The pk att.</param>
		public PrimaryKeyModel(PropertyInfo propInfo, PrimaryKeyAttribute pkAtt)
		{
			this.propInfo = propInfo;
			this.pkAtt = pkAtt;
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
		/// Gets the primary key attribute
		/// </summary>
		/// <value>The primary key att.</value>
		public PrimaryKeyAttribute PrimaryKeyAtt
		{
			get { return pkAtt; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitPrimaryKey(this);
		}

		#endregion
	}
}
