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

namespace Castle.CastleOnRails.WindsorExtension
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel;

	using Castle.Model.Configuration;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal;
	using Castle.CastleOnRails.Framework.Internal.Graph;


	public class RailsFacility : IFacility
	{
		private ControllerTree _tree;

		public RailsFacility()
		{
		}

		#region IFacility

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			kernel.AddComponent( "rails.controllertree", typeof(ControllerTree) );

			_tree = (ControllerTree) kernel["rails.controllertree"];

			kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);
		}

		public void Terminate()
		{
		}

		#endregion

		private void OnComponentRegistered(String key, IHandler handler)
		{
			if (handler.ComponentModel.Implementation == null)
			{
				return;
			}

			if ( typeof(Controller).IsAssignableFrom(handler.ComponentModel.Implementation) )
			{
				// Ensure its transient
				handler.ComponentModel.LifestyleType = LifestyleType.Transient;

				// Register controller in the tree of controllers
				ControllerDescriptor descriptor = ControllerInspectionUtil.Inspect(
					handler.ComponentModel.Implementation);
				
				_tree.AddController( descriptor.Area, descriptor.Name, key );
			}			
		}
	}
}
