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

namespace Castle.CastleOnRails.Engine.WindsorExtension
{
	using System;
	using System.Web;

	using Castle.Windsor;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal;
	using Castle.CastleOnRails.Framework.Internal.Graph;


	public class WindsorControllerFactory : IControllerFactory
	{
		#region IControllerFactory

		public Controller GetController(UrlInfo urlInfo)
		{
			IWindsorContainer container = ContainerAccessorUtil.ObtainContainer();

			ControllerTree tree = (ControllerTree) container["rails.controllertree"];

			String key = (String) tree.GetController(urlInfo.Area, urlInfo.Controller);

			if (container.Kernel.HasComponent(key))
			{
				return (Controller) container[key];
			}
			
			return null;
		}

		public void Release(Controller controller)
		{
			ContainerAccessorUtil.ObtainContainer().Release(controller);
		}

		#endregion

	}
}
