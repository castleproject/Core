// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	internal class MetaType
	{
		private readonly MetaTypeElementCollection<MetaEvent> events = new MetaTypeElementCollection<MetaEvent>();
		private readonly MetaTypeElementCollection<MetaMethod> methods = new MetaTypeElementCollection<MetaMethod>();
		private readonly Dictionary<MethodInfo, MetaMethod> methodsIndex = new Dictionary<MethodInfo, MetaMethod>();
		private readonly MetaTypeElementCollection<MetaProperty> properties = new MetaTypeElementCollection<MetaProperty>();

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
			methodsIndex.Add(method.Method, method);  // shouldn't get added twice
		}

		public void AddProperty(MetaProperty property)
		{
			properties.Add(property);
		}

		public MetaMethod FindMethod(MethodInfo method)
		{
			return methodsIndex.TryGetValue(method, out var metaMethod) ? metaMethod : null;
		}
	}
}