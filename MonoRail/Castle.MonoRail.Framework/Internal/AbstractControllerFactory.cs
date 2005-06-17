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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal.Graph;

	/// <summary>
	/// Base implementation of <see cref="IControllerFactory"/>
	/// using the <see cref="ControllerTree"/> to build an hierarchy
	/// of controllers and the areas they belong to.
	/// </summary>
	public abstract class AbstractControllerFactory : IControllerFactory
	{
		private readonly ControllerTree _tree;

		public AbstractControllerFactory()
		{
			_tree = new ControllerTree();
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

		public ControllerTree Tree
		{
			get { return _tree; }
		}

		protected virtual Controller CreateControllerInstance(string area, string name)
		{
			Type type = (Type) Tree.GetController(area, name);

			if (type == null)
			{
				throw new RailsException(
					string.Format("Controller not found. Area: '{0}'. Controller Name: '{0}'.", area, name ) );
			}

			return (Controller) Activator.CreateInstance( type );
		}
	}
}
