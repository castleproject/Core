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
	/// This is used in IdBag scenario to specify to collection id.
	/// </summary>
	[Serializable]
	public class CollectionIDModel : IVisitable
	{
		private readonly PropertyInfo propInfo;
		private readonly CollectionIDAttribute collAtt;
		private HiloModel hilo;

		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionIDModel"/> class.
		/// </summary>
		/// <param name="propInfo">The prop info.</param>
		/// <param name="collAtt">The coll att.</param>
		public CollectionIDModel( PropertyInfo propInfo, CollectionIDAttribute collAtt )
		{
			this.collAtt = collAtt;
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
		/// Gets the collection ID att.
		/// </summary>
		/// <value>The collection ID att.</value>
		public CollectionIDAttribute CollectionIDAtt
		{
			get { return collAtt; }
		}

		/// <summary>
		/// Gets or sets the hilo.
		/// </summary>
		/// <value>The hilo.</value>
		public HiloModel Hilo
		{
			get { return hilo; }
			set { hilo = value; }
		}

		#region IVisitable Members

		/// <summary>
		/// Accepts the specified visitor and call the relevant IVisitor.Visit***() method
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		public void Accept(IVisitor visitor)
		{
			visitor.VisitCollectionID(this);
		}

		#endregion
	}
}
