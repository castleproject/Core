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

	/// <summary>
	/// Represents the information about a <see cref="Controller"/>.
	/// </summary>
	public class ControllerDescriptor
	{
		private Type _controllerType;
		private String _name;
		private String _area;

		public ControllerDescriptor(Type controllerType, String name, String area)
		{
			_controllerType = controllerType;
			_name = name;
			_area = area;
		}

		public Type ControllerType
		{
			get { return _controllerType; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string Area
		{
			get { return _area; }
		}
	}
}
