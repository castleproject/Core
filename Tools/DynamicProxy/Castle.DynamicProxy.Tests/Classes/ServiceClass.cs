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

namespace Castle.DynamicProxy.Test.Classes
{
	using System;

	/// <summary>
	/// Summary description for ServiceClass.
	/// </summary>
	public class ServiceClass
	{
		public virtual int Sum(int b1, int b2)
		{
			return b1 + b2;
		}

		public virtual byte Sum(byte b1, byte b2)
		{
			return System.Convert.ToByte( b1 + b2 );
		}

		public virtual long Sum(long b1, long b2)
		{
			return b1 + b2;
		}

		public virtual short Sum(short b1, short b2)
		{
			return (short) (b1 + b2);
		}

		public virtual float Sum(float b1, float b2)
		{
			return b1 + b2;
		}

		public virtual double Sum(double b1, double b2)
		{
			return b1 + b2;
		}

		public virtual UInt16 Sum(UInt16 b1, UInt16 b2)
		{
			return (UInt16) (b1 + b2);
		}

		public virtual UInt32 Sum(UInt32 b1, UInt32 b2)
		{
			return b1 + b2;
		}

		public virtual UInt64 Sum(UInt64 b1, UInt64 b2)
		{
			return b1 + b2;
		}

		public virtual bool Valid
		{
			get { return true; }
		}
	}
	
	public class ServiceClass2
	{
		public void DoSomething()
		{
			DoOtherThing();
		}
		
		public virtual void DoOtherThing()
		{
			DoSomethingElse();
		}

		public virtual void DoSomethingElse()
		{
			
		}
	}
}