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


	public class PrimaryKeyModel : IModelNode
	{
		private readonly PropertyInfo propInfo;
		private readonly PrimaryKeyAttribute pkAtt;

		public PrimaryKeyModel( PropertyInfo propInfo, PrimaryKeyAttribute pkAtt )
		{
			this.propInfo = propInfo;
			this.pkAtt = pkAtt;
		}

		public PropertyInfo Property
		{
			get { return propInfo; }
		}

		public PrimaryKeyAttribute PrimaryKeyAtt
		{
			get { return pkAtt; }
		}

		#region IModelNode Members

		public String ToXml()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitPrimaryKey(this);
		}

		#endregion
	}
}
