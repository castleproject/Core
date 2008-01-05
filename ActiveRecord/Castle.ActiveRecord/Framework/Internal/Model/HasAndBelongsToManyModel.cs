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
	/// Model to HasAndBelongsToMany, which is used to model a many to many assoication.
	/// </summary>
	[Serializable]
	public class HasAndBelongsToManyModel : IVisitable
	{
		private readonly PropertyInfo propInfo;
		private readonly HasAndBelongsToManyAttribute hasManyAtt;
		private CollectionIDModel collectionID;

		/// <summary>
		/// Initializes a new instance of the <see cref="HasAndBelongsToManyModel"/> class.
		/// </summary>
		/// <param name="propInfo">The prop info.</param>
		/// <param name="hasManyAtt">The has many att.</param>
		public HasAndBelongsToManyModel( PropertyInfo propInfo, HasAndBelongsToManyAttribute hasManyAtt )
		{
			this.hasManyAtt = hasManyAtt;
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
		/// Gets the has many attribute
		/// </summary>
		/// <value>The has many att.</value>
		public HasAndBelongsToManyAttribute HasManyAtt
		{
			get { return hasManyAtt; }
		}

		/// <summary>
		/// Gets or sets the collection ID.
		/// </summary>
		/// <value>The collection ID.</value>
		public CollectionIDModel CollectionID
		{
			get { return collectionID; }
			set { collectionID = value; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitHasAndBelongsToMany(this);
		}

		#endregion
	}
}
