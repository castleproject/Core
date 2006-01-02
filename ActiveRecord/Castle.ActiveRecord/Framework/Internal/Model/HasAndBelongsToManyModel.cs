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


	[Serializable]
	public class HasAndBelongsToManyModel : IModelNode
	{
		private readonly PropertyInfo propInfo;
		private readonly HasAndBelongsToManyAttribute hasManyAtt;
		private CollectionIDModel collectionID;

		public HasAndBelongsToManyModel( PropertyInfo propInfo, HasAndBelongsToManyAttribute hasManyAtt )
		{
			this.hasManyAtt = hasManyAtt;
			this.propInfo = propInfo;
		}

		public PropertyInfo Property
		{
			get { return propInfo; }
		}

		public HasAndBelongsToManyAttribute HasManyAtt
		{
			get { return hasManyAtt; }
		}

		public CollectionIDModel CollectionID
		{
			get { return collectionID; }
			set { collectionID = value; }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitHasAndBelongsToMany(this);
		}

		#endregion
	}
}
