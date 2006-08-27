// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Winforms.AssemblyResolver
{
	using System;
	using System.Reflection;
	using System.Collections.Specialized;
	using System.Windows.Forms;

	using Castle.Core;

	/// <summary>
	/// This component installs an Assembly Resolver delegate that 
	/// asks the user for the location of the missing assembly. Once provided
	/// it caches the location.
	/// </summary>
	[CastleComponent("assembly.resolver")]
	public class AssemblyResolverComponent : IStartable
	{
		private String lastAssemblyPath;
		private HybridDictionary name2Assembly = new HybridDictionary();

		public AssemblyResolverComponent()
		{
		}

		#region IStartable Members

		public void Start()
		{
			AppDomain.CurrentDomain.AssemblyResolve += 
				new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}

		public void Stop()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= 
				new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}

		#endregion

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly assembly = null;

			String name = AssemblyUtils.Normalize(args.Name);

			// Cache			
			if (name2Assembly.Contains(name))
			{
				return name2Assembly[name] as Assembly;
			}

			// Try to load from the last path
			if (lastAssemblyPath != null)
			{
				assembly = AssemblyUtils.TryToLoadAssembly(lastAssemblyPath, name);

				if (assembly != null)
				{
					UpdateCache(name, assembly);
					return assembly;
				}
			}

			// At last, we ask the user for the location
			assembly = ShowForm(args, name);

			if (assembly != null)
			{
				UpdateCache(name, assembly);
				return assembly;
			}

			// All went wrong. 
			// Could not find it.

			return null;
		}

		private Assembly ShowForm(ResolveEventArgs args, string name)
		{
			LocateAssemblyForm form = new LocateAssemblyForm(args.Name);
			DialogResult result = DialogResult.Abort;
	
			if (Form.ActiveForm != null)
			{
				result = form.ShowDialog(Form.ActiveForm);
			}
			else
			{
				result = form.ShowDialog();
			}
	
			if (result == DialogResult.OK)
			{
				lastAssemblyPath = form.AssemblyLocation;

				return form.AssemblyLoaded;
			}

			return null;
		}

		private void UpdateCache(string name, Assembly loaded)
		{
			name2Assembly[name] = loaded;
		}
	}
}
