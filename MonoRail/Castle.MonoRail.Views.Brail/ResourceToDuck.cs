// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.Brail
{
	using System.Collections;
	using Boo.Lang;
	using Castle.MonoRail.Framework.Resources;
	using Framework;

	// This allows to treat resources in a natural way:
	//	text.Hello
	// Instead of:
	// 	text["Hello"]
	public class ResourceToDuck : IQuackFu, IEnumerable
	{
		private IResource resource;

		public ResourceToDuck(IResource resource)
		{
			this.resource = resource;
		}

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return resource.GetEnumerator();
		}

		#endregion

		#region IQuackFu Members

		public object QuackGet(string name, object[] parameters)
		{
			if (IsStringIndexer(name, parameters))
			{
				name = (string) parameters[0];
			}
			object val = resource.GetObject(name);
			if (val == null)
			{
				throw new MonoRailException("Resource " + name + " does not exists");
			}
			return val;
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			throw new MonoRailException("You cannnot set resource " + name);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new MonoRailException("You cannnot invoke resource " + name);
		}

		#endregion

		private static bool IsStringIndexer(string name, object[] parameters)
		{
			return name.Length == 0 && parameters.Length == 1 && parameters[0] is string;
		}
	}
}