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

using System;
namespace Castle.DynamicProxy.Test.Classes
{
	public class RefAndOutClass
	{
		public RefAndOutClass()
		{
		}

		// Primitive values
		public virtual void RefInt(ref int i)
		{
			i += 1;
		}

		public virtual void OutInt(out int i)
		{
			i = 2;
		}
		
		public virtual void RefChar(ref char i)
		{
			i = 'a';
		}

		public virtual void OutChar(out char i)
		{
			i = 'b';
		}

		// Reference types
		public virtual void RefString(ref string s)
		{
			s += "_string";
		}

		public virtual void OutString(out string s)
		{
			s = "string";
		}

		// Struct types
		public virtual void RefDateTime(ref DateTime dt)
		{
			dt = dt.AddYears(1);
		}

		public virtual void OutDateTime(out DateTime dt)
		{
			dt = new DateTime(2005, 1, 1);
		}

		// Enum types
		public virtual void RefSByteEnum(ref SByteEnum en)
		{
			en = en == SByteEnum.One ? SByteEnum.Two : SByteEnum.One;
		}

		public virtual void OutSByteEnum(out SByteEnum en)
		{
			en = SByteEnum.Two;
		}
	}
}
