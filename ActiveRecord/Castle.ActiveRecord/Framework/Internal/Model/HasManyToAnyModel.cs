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

	using Castle.ActiveRecord;


	[Serializable]
	public class HasManyToAnyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly HasManyToAnyAttribute hasManyToAnyAtt;

		public HasManyToAnyModel(PropertyInfo prop, HasManyToAnyAttribute hasManyToAnyAtt)
		{
			this.prop = prop;
			this.hasManyToAnyAtt = hasManyToAnyAtt;
		}

		public PropertyInfo Property
		{
			get { return prop; }
		}

		public HasManyToAnyAttribute HasManyToAnyAtt
		{
			get { return hasManyToAnyAtt; }
		}

		public Config Configuration
		{
			get { return  new Config(this); }
		}

		#region IVisitable Members

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

			public HasManyToAnyModel Parent
			{
				get { return parent; }
				set { parent = value; }
			}

			internal Config(HasManyToAnyModel parent)
			{
				this.parent = parent;
			}

			#region IVisitable Members

			public void Accept(IVisitor visitor)
			{
				visitor.VisitHasManyToAnyConfig(this);
			}

			#endregion
		}
	}
}
