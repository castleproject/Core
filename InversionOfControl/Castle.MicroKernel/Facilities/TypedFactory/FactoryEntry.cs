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

namespace Castle.Facilities.TypedFactory
{
	using System;

	public class FactoryEntry
	{
		private String _id;
		private Type _factoryInterface;
		private String _creationMethod;
		private String _destructionMethod;

		public FactoryEntry(String id, Type factoryInterface, String creationMethod, String destructionMethod)
		{
			if (id == null || id.Length == 0) throw new ArgumentNullException("id");
			if (factoryInterface == null) throw new ArgumentNullException("factoryInterface");
			if (!factoryInterface.IsInterface) throw new ArgumentException("factoryInterface must be an interface");
			if (creationMethod == null || creationMethod.Length == 0) throw new ArgumentNullException("creationMethod");

			_id = id;
			_factoryInterface = factoryInterface;
			_creationMethod = creationMethod;
			_destructionMethod = destructionMethod;
		}

		public String Id
		{
			get { return _id; }
		}

		public Type FactoryInterface
		{
			get { return _factoryInterface; }
		}

		public String CreationMethod
		{
			get { return _creationMethod; }
		}

		public String DestructionMethod
		{
			get { return _destructionMethod; }
		}
	}
}
