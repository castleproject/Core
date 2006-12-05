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

namespace Castle.MonoRail.Views.Brail
{
	using System.Reflection;
	using Boo.Lang;
	using Castle.MonoRail.Framework;

	public class IgnoreNull : IQuackFu
	{
		private object target;

		public IgnoreNull(object target)
		{
			this.target = target;
		}

		public object QuackGet(string name)
		{
			if (target == null)
				return null;
			PropertyInfo property = target.GetType().GetProperty(name);
			if (property == null)
				throw new RailsException("Could not find property " + name + " on " + target.GetType().FullName);
			return property.GetValue(target, null);
		}

		public object QuackSet(string name, object obj)
		{
			if (target == null)
				return null;
			PropertyInfo property = target.GetType().GetProperty(name);
			if (property == null)
				throw new RailsException("Could not find property " + name + " on " + target.GetType().FullName);
			property.SetValue(target, obj, null);
			return null;
		}

		public object QuackInvoke(string name, object[] args)
		{
			if (target == null)
				return null;
			MethodInfo method = target.GetType().GetMethod(name);
			if (method == null)
				throw new RailsException("Could not find method " + name + " on " + target.GetType().FullName);
			return method.Invoke(target, args);
		}
	}
}