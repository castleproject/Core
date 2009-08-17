// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace NVelocity.Runtime.Directive
{
	using System;
	using System.Collections;

	public class DirectiveManager : IDirectiveManager
	{
		private readonly IDictionary name2Type = Hashtable.Synchronized(new Hashtable());


		public virtual void Register(String directiveTypeName)
		{
			directiveTypeName = directiveTypeName.Replace(';', ',');

			Type type = Type.GetType(directiveTypeName, false, false);

			if (type == null)
			{
				throw new Exception(string.Format("Could not resolve type {0}", directiveTypeName));
			}

			Directive directive = (Directive) Activator.CreateInstance(type);

			name2Type[directive.Name] = type;
		}

		public virtual Directive Create(String name, Stack directiveStack)
		{
			Type type = (Type) name2Type[name];

			if (type == null)
			{
				if (directiveStack.Count != 0)
				{
					Directive parent = (Directive) directiveStack.Peek();

					if (parent.SupportsNestedDirective(name))
					{
						return parent.CreateNestedDirective(name);
					}
				}
			}
			else
			{
				return (Directive) Activator.CreateInstance(type);
			}

			return null;
		}

		public virtual bool Contains(String name)
		{
			return name2Type.Contains(name);
		}
	}
}