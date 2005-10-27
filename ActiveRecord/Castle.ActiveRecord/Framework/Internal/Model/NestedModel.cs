// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	public class NestedModel : IModelNode
	{
		private readonly PropertyInfo propInfo;
		private readonly NestedAttribute nestedAtt;
		private readonly ActiveRecordModel nestedModel;

		public NestedModel( PropertyInfo propInfo, NestedAttribute nestedAtt, ActiveRecordModel nestedModel )
		{
			this.nestedAtt = nestedAtt;
			this.nestedModel = nestedModel;
			this.propInfo = propInfo;
		}

		public ActiveRecordModel Model
		{
			get { return nestedModel; }
		}

		public PropertyInfo Property
		{
			get { return propInfo; }
		}

		public NestedAttribute NestedAtt
		{
			get { return nestedAtt; }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitNested(this);
		}

		#endregion
	}
}
