// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.CastleOnRails.Framework.Internal
{
	using System;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal.Graph;

	public abstract class AbstractControllerFactory : IControllerFactory
	{
		private readonly ControllerTree _tree;

		public AbstractControllerFactory()
		{
			_tree = new ControllerTree();
		}

		public ControllerTree Tree
		{
			get { return _tree; }
		}

		#region IControllerFactory

		public virtual Controller GetController(UrlInfo urlInfo)
		{
			String area = urlInfo.Area == null ? String.Empty : urlInfo.Area;
			String name = urlInfo.Controller;

			return CreateControllerInstance(area, name);
		}

		public virtual void Release(Controller controller)
		{			
		}

		#endregion

		protected virtual Controller CreateControllerInstance(string area, string name)
		{
			Type type = (Type) Tree.GetController(area, name);
			return (Controller) Activator.CreateInstance( type );
		}
	}
}
