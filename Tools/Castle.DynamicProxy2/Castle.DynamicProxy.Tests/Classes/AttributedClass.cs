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

namespace Castle.DynamicProxy.Tests.Classes
{
	using System;
	using System.IO;

	[NonInheritableAttribute]
	public class AttributedClass
	{
		[NonInheritableAttribute]
		public virtual void Do1()
		{
		}
	}

	[ComplexNonInheritableAttribute(1, 2, true, "class", FileAccess.Write)]
	public class AttributedClass2
	{
		[ComplexNonInheritableAttribute(2, 3, "Do1", Access = FileAccess.ReadWrite)]
		public virtual void Do1()
		{
		}

		[ComplexNonInheritableAttribute(3, 4, "Do2", IsSomething=true)]
		public virtual void Do2()
		{
		}
	}
	
	[Serializable]
	public class NonInheritableAttribute : Attribute
	{
	}

	[Serializable]
	public class ComplexNonInheritableAttribute : Attribute
	{
		public int id, num;
		public bool isSomething;
		public String name;
		public FileAccess access;

		public ComplexNonInheritableAttribute(int id, int num, string name)
		{
			this.id = id;
			this.num = num;
			this.name = name;
		}

		public ComplexNonInheritableAttribute(int id, int num, bool isSomething, string name, FileAccess access)
		{
			this.id = id;
			this.num = num;
			this.isSomething = isSomething;
			this.name = name;
			this.access = access;
		}

		public int Id
		{
			get { return id; }
		}

		public int Num
		{
			get { return num; }
		}

		public bool IsSomething
		{
			get { return isSomething; }
			set { isSomething = value; }
		}

		public string Name
		{
			get { return name; }
		}

		public FileAccess Access
		{
			get { return access; }
			set { access = value; }
		}
	}
}
