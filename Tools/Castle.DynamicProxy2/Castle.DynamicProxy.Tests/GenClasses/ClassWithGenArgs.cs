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

namespace Castle.DynamicProxy.Tests.GenClasses
{
	public class ClassWithGenArgs<T>
	{
		private bool propVal;
		private bool invoked;

		public bool Invoked
		{
			get { return invoked; }
		}

		public virtual bool AProperty
		{
			set { propVal = value; }
			get { return propVal; }
		}
		
		public virtual void DoSomething()
		{
			invoked = true;
		}
	}

	public class ClassWithGenArgs<T, Z>
	{
		private bool propVal;
		private bool invoked;

		public bool Invoked
		{
			get { return invoked; }
		}

		public virtual bool AProperty
		{
			set { propVal = value; }
			get { return propVal; }
		}

		public virtual void DoSomething()
		{
			invoked = true;
		}
	}
	
	public class SubClassWithGenArgs<T, Z, Y> : ClassWithGenArgs<T, Z>
	{
		public override void DoSomething()
		{
#pragma warning disable 219
			int x = 1 + 10; // Just something to fool the compiler 
#pragma warning restore 219
			base.DoSomething();
		}
	}
}
