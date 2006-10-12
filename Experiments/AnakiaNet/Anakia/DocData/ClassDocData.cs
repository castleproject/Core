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

namespace Anakia.DocData
{
	using System;
	
	public enum Visibility
	{
		Public,
		NestedPublic,
		Family,
		FamilyOrAssembly
	}
	
	public enum ClassType
	{
		Class,
		Interface
	}

	public class ClassDocData : CommonDocData
	{
		private readonly ClassType classType;
		internal string name;
		internal string id;
		internal Visibility access;
		
		internal ConstructorDocData[] constructors;
		internal PropertyDocData[] properties;
		internal MethodDocData[] methods;

		public ClassDocData(ClassType classType)
		{
			this.classType = classType;
		}

		public ClassType ClassType
		{
			get { return classType; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Id
		{
			get { return id; }
		}

		public Visibility Access
		{
			get { return access; }
		}

		public ConstructorDocData[] Constructors
		{
			get { return constructors; }
		}

		public PropertyDocData[] Properties
		{
			get { return properties; }
		}

		public MethodDocData[] Methods
		{
			get { return methods; }
		}
	}
}
