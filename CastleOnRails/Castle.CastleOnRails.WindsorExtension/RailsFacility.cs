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

namespace Castle.CastleOnRails.WindsorExtension
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel;

	using Castle.Model.Configuration;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal;
	using Castle.CastleOnRails.Framework.Internal.Graph;


	/// <summary>
	/// Facility responsible for registering the controllers in
	/// a tree.
	/// </summary>
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

			kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
		}

		public void Terminate()
		{
		}

		#endregion

		private void OnComponentModelCreated(ComponentModel model)
		{
			if ( !typeof(Controller).IsAssignableFrom(model.Implementation) )
			{
				return;
			}

			// Ensure its transient
			model.LifestyleType = LifestyleType.Transient;

			// Register controller in the tree of controllers
			ControllerDescriptor descriptor = ControllerInspectionUtil.Inspect(model.Implementation);
			
			_tree.AddController( descriptor.Area, descriptor.Name, model.Name );
		}
	}
}
