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

namespace Castle.CastleOnRails.Engine.ControllerFactory
{
	using System;

	using Castle.CastleOnRails.Framework;

	/// <summary>
	/// Summary description for DefaultControllerFactory.
	/// </summary>
	public class DefaultControllerFactory : IControllerFactory
	{
		private ControllersCache _cache;

		public DefaultControllerFactory(ControllersCache cache)
		{
			_cache = cache;
		}

		public Controller GetController(String name)
		{
			Type controllerType = _cache.GetController( name );

			if (controllerType == null)
			{
				throw new RailsException( String.Format("Could not find controller for {0}",
					name) );
			}

			return (Controller) Activator.CreateInstance( controllerType );
		}

		public void Release(Controller controller)
		{
			
		}
	}
}
