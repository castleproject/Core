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

namespace Castle.Windsor.Configuration
{
	using System;

	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Enables a hierarchical configuration store. 
	/// </summary>
	public class CascadeConfigurationStore : DefaultConfigurationStore
	{
		public CascadeConfigurationStore(
			IConfigurationInterpreter parent, IConfigurationInterpreter child)
		{
			if (parent == null) throw new ArgumentNullException("parent");
			if (child == null) throw new ArgumentNullException("child");

			/// The parent configuration add the main entries.
			parent.Process( this );

			// The child may overwrite the config entries.
			child.Process( this );
		}
	}
}
