// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;

	public class MetaType
	{
		private readonly ICollection<MetaEvent> events = new TypeElementCollection<MetaEvent>();
		private readonly ICollection<MetaMethod> methods = new TypeElementCollection<MetaMethod>();
		private readonly ICollection<MetaProperty> properties = new TypeElementCollection<MetaProperty>();

		public IEnumerable<MetaEvent> Events
		{
			get { return events; }
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

		public void AddEvent(MetaEvent @event)
		{
			events.Add(@event);
		}

		public void AddMethod(MetaMethod method)
		{
			methods.Add(method);
		}

		public void AddProperty(MetaProperty property)
		{
			properties.Add(property);
		}
	}
}