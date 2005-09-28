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


	public class PropertyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly PropertyAttribute att;

		protected PropertyModel() {}

		public PropertyModel(PropertyInfo prop, PropertyAttribute att)
		{
			this.prop = prop;
			this.att = att;
		}

		public virtual string PropertyName
		{
			get { return prop.Name; }
		}

		public virtual bool CanRead
		{
			get { return prop.CanRead; }
		}
		
		public virtual bool CanWrite
		{
			get { return prop.CanWrite; }
		}

		public virtual bool IsIndexed
		{
			get { return prop.GetIndexParameters().Length != 0; }
		}

		public virtual object GetValue(object instance)
		{
			return prop.GetGetMethod().Invoke( instance, null );
		} 

		public virtual Type PropertyType
		{
			get { return prop.PropertyType; }
		}

		public virtual PropertyAttribute PropertyAtt
		{
			get { return att; }
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
			visitor.VisitProperty(this);
		}

		#endregion
	}
}
