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

namespace Castle.DynamicProxy.Tests.Classes
{
	using System;

	public class DiffAccessLevelOnProperties
	{
		private int age, age2;
		private int maxval, maxval2;
		private String name;

		public void SetProperties()
		{
			Age = 10;
			Age2 = 11;
			Maxval = 12;
			Maxval2 = 13;
			name = "name";
		}

		public virtual int Age
		{
			protected get { return age; }
			set { age = value; }
		}

		public virtual int Age2
		{
			get { return age2; }
			protected set { age2 = value; }
		}

		public virtual int Maxval
		{
			private get { return maxval; }
			set { maxval = value; }
		}

		public int Maxval2
		{
			get { return maxval2; }
			private set { maxval2 = value; }
		}

		public virtual string Name
		{
			internal get { return name; }
			set { name = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4}", Age, Age2, Maxval, Maxval2, Name);
		}
	}
}