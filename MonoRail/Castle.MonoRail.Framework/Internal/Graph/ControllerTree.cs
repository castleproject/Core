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

namespace Castle.MonoRail.Framework.Internal.Graph
{
	using System;
	using System.Collections;

	public sealed class ControllerTree
	{
		private String _area;
		private IDictionary _controllers;
		private ControllerTree _left;
		private ControllerTree _right;

		public ControllerTree() : this(String.Empty)
		{
		}

		public ControllerTree(String area)
		{
			if (area == null) throw new ArgumentNullException("area");

			_area = area;
			_controllers = new Hashtable(
					CaseInsensitiveHashCodeProvider.Default, 
					CaseInsensitiveComparer.Default);
		}

		public void AddController(String area, String controllerName, object controller)
		{
			if (area == null) throw new ArgumentNullException("area");
			if (controllerName == null) throw new ArgumentNullException("controllerName");
			if (controller == null) throw new ArgumentNullException("controller");

			int cmp = String.Compare(area, _area, true);

			if (cmp == 0)
			{
				_controllers[controllerName] = controller;
			}
			else
			{
				ControllerTree node = null;

				if (cmp < 0)
				{
					if (_left == null)
					{
						_left = new ControllerTree(area);
					}
					node = _left;
				}
				else
				{
					if (_right == null)
					{
						_right = new ControllerTree(area);
					}
					node = _right;
				}

				node.AddController(area, controllerName, controller);
			}
		}

		public object GetController(String area, String controllerName)
		{
			if (area == null) throw new ArgumentNullException("area");
			if (controllerName == null) throw new ArgumentNullException("controllerName");

			int cmp = String.Compare(area, _area, true);
			
			if (cmp == 0)
			{
				return _controllers[controllerName];
			}
			else
			{
				ControllerTree node = null;

				if (cmp < 0)
				{
					node = _left;
				}
				else
				{
					node = _right;
				}

				if (node != null)
				{
					return node.GetController(area, controllerName);
				}
			}

			return null;
		}
	}
}
