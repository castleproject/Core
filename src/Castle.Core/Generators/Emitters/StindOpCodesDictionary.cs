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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection.Emit;
	using System.Collections.Generic;

	/// <summary>
	/// Provides appropriate Stind.X opcode 
	/// for the type of primitive value to be stored indirectly.
	/// </summary>
	public sealed class StindOpCodesDictionary : Dictionary<Type,OpCode>
	{
		private static readonly StindOpCodesDictionary _dict = new StindOpCodesDictionary();

		private static readonly OpCode _emptyOpCode = new OpCode();

		private StindOpCodesDictionary() : base()
		{
			this.Add(typeof (bool), OpCodes.Stind_I1);
			this.Add(typeof (char), OpCodes.Stind_I2);
			this.Add(typeof (SByte), OpCodes.Stind_I1);
			this.Add(typeof (Int16), OpCodes.Stind_I2);
			this.Add(typeof (Int32), OpCodes.Stind_I4);
			this.Add(typeof (Int64), OpCodes.Stind_I8);
			this.Add(typeof (float), OpCodes.Stind_R4);
			this.Add(typeof (double), OpCodes.Stind_R8);
			this.Add(typeof (byte), OpCodes.Stind_I1);
			this.Add(typeof (UInt16), OpCodes.Stind_I2);
			this.Add(typeof (UInt32), OpCodes.Stind_I4);
			this.Add(typeof (UInt64), OpCodes.Stind_I8);
		}

		public new OpCode this[Type type]
		{
			get
			{
				if (this.ContainsKey(type))
				{
					return base[type];
				}
				return EmptyOpCode;
			}
		}

		public static StindOpCodesDictionary Instance
		{
			get { return _dict; }
		}

		public static OpCode EmptyOpCode
		{
			get { return _emptyOpCode; }
		}
	}
}
