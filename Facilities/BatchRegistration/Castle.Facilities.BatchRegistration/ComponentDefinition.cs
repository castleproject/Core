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

namespace Castle.Facilities.BatchRegistration
{
	using System;

	public sealed class ComponentDefinition
	{
		private String _key;
		private String _service;
		private String _class;
		private Type _serviceType;
		private Type _classType;

		public ComponentDefinition(String key, String service, String className)
		{
			_key = key;
			_service = service;
			_class = className;
		}

		public ComponentDefinition(String key, Type serviceType, Type classType)
		{
			_key = key;
			_serviceType = serviceType;
			_classType = classType;
		}

		public String Key
		{
			get { return _key; }
		}

		public String Service
		{
			get { return _service; }
		}

		public String ClassName
		{
			get { return _class; }
		}

		public Type ServiceType
		{
			get { return _serviceType; }
		}

		public Type ClassType
		{
			get { return _classType; }
		}
	}
}
