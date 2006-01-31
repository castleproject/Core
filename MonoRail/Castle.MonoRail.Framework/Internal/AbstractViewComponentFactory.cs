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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework.ViewComponents;

	public abstract class AbstractViewComponentFactory : IViewComponentFactory
	{
		private readonly IDictionary components;
		
		public AbstractViewComponentFactory()
		{
			components = new HybridDictionary(true);
			AddBuiltInComponents();
		}

		protected virtual void AddBuiltInComponents()
		{
			RegisterComponent( "CaptureFor", typeof(CaptureFor) );
			RegisterComponent( "SecurityComponent", typeof(SecurityComponent) );
		}

		protected void RegisterComponent(String name, Type type)
		{
			if ( !typeof(ViewComponent).IsAssignableFrom(type) )
			{
				throw new RailsException( "RegisterComponent({0},{1}) failed, components must inherit from ViewComponent", name, type.FullName );
			}

			components[name] = type;
		}

		public abstract IViewEngine ViewEngine { get; set; }

		public virtual ViewComponent Create(String name)
		{
			Type viewCompType = (Type) components[name];

			if (viewCompType == null)
			{
				throw new RailsException("No ViewComponent found for name " + name);
			}

			return (ViewComponent) Activator.CreateInstance( viewCompType );
		}

		public virtual void Release(ViewComponent instance)
		{			
		}
	}
}
