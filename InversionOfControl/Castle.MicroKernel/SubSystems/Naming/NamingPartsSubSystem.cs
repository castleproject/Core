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

namespace Castle.MicroKernel.SubSystems.Naming
{
	using System;


	[Serializable]
	public class NamingPartsSubSystem : DefaultNamingSubSystem
	{
		private BinaryTreeComponentName _tree;

		public NamingPartsSubSystem()
		{
			_tree = new BinaryTreeComponentName();
		}

		private ComponentName ToComponentName(String key)
		{
			return new ComponentName(key);
		}

		public override bool Contains(String key)
		{
			return _tree.Contains( ToComponentName(key) );
		}

		public override void UnRegister(String key)
		{
			_tree.Remove( ToComponentName(key) );
		}

		public override IHandler GetHandler(String key)
		{
			return _tree.GetHandler( ToComponentName(key) );
		}

		public override IHandler[] GetHandlers(String query)
		{
			return _tree.GetHandlers( ToComponentName(query) );
		}

		public override IHandler[] GetHandlers()
		{
			return _tree.Handlers;
		}

		public override IHandler this[String key]
		{
			set { _tree.Add( ToComponentName(key), value ); }
		}

		public override int ComponentCount
		{
			get { return _tree.Count; }
		}
	}
}
