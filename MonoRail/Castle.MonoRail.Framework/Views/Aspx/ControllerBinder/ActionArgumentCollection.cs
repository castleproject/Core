// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.ComponentModel;
	using System.Drawing.Design;

	[Editor(typeof(Design.ActionArgumentEditor), typeof(UITypeEditor))]
	public class ActionArgumentCollection : TypedCollection<ActionArgument>
	{
		public ActionArgument Add(string name, object value)
		{
			ActionArgument actionArg = new ActionArgument();
			actionArg.Name = name;
			actionArg.Value = value;
			Add(actionArg);
			return actionArg;
		}

		public bool Remove(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			for (int i = 0; i < List.Count; ++i)
			{
				if (this[i].Name == name)
				{
					RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		protected override void OnInsert(int index, object value)
		{
			ActionArgument newActionArg = (ActionArgument) value;

			foreach (ActionArgument actionArg in List)
			{
				if (string.Compare(actionArg.Name, newActionArg.Name, true) == 0)
				{
					throw new ArgumentException(
						string.Format("Argument Action '{0}' already exists.  Please remove it first.",
						actionArg.Name));
				}
			}

			base.OnInsert(index, value);
		}
	}
}

#endif
