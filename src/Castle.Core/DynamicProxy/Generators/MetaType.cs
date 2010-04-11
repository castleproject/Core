// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;

	public class MetaType
	{
		private readonly string name;
		private readonly Type baseType;
		private readonly IEnumerable<Type> interfaces;

		private readonly ICollection<MetaProperty> properties = new TypeElementCollection<MetaProperty>();
		private readonly ICollection<MetaEvent> events = new TypeElementCollection<MetaEvent>();
		private readonly ICollection<MetaMethod> methods = new TypeElementCollection<MetaMethod>();

		public MetaType(string name, Type baseType, IEnumerable<Type> interfaces)
		{
			this.name = name;
			this.baseType = baseType;
			this.interfaces = interfaces;
		}

		public IEnumerable<MetaMethod> Methods
		{
			get { return methods; // NOTE: should be readonly 
			}
		}

		public IEnumerable<MetaProperty> Properties
		{
			get { return properties; }
		}

		public IEnumerable<MetaEvent> Events
		{
			get { return events; }
		}

		public void AddMethod(MetaMethod method)
		{
			methods.Add(method);
		}

		public void AddEvent(MetaEvent @event)
		{
			events.Add(@event);
		}

		public void AddProperty(MetaProperty property)
		{
			properties.Add(property);
		}
	}
}