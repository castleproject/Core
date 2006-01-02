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
	public class HiloModel : IModelNode
	{
		private readonly PropertyInfo propInfo;
		private readonly HiloAttribute hiloAtt;

		public HiloModel( PropertyInfo propInfo, HiloAttribute hiloAtt )
		{
			this.hiloAtt = hiloAtt;
			this.propInfo = propInfo;
		}

		public PropertyInfo Property
		{
			get { return propInfo; }
		}

		public HiloAttribute HiloAtt
		{
			get { return hiloAtt; }
		}

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitHilo(this);
		}

		#endregion
	}
}
