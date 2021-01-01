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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Reflection.Emit;

	/// <summary>
	///   Provides appropriate Stind.X opcode 
	///   for the type of primitive value to be stored indirectly.
	/// </summary>
	internal sealed class StindOpCodesDictionary : Dictionary<Type, OpCode>
	{
		private static readonly StindOpCodesDictionary dict = new StindOpCodesDictionary();

		// has to be assigned explicitly to suppress compiler warning
		private static readonly OpCode emptyOpCode = new OpCode();

		private StindOpCodesDictionary()
		{
			Add(typeof(bool), OpCodes.Stind_I1);
			Add(typeof(char), OpCodes.Stind_I2);
			Add(typeof(SByte), OpCodes.Stind_I1);
			Add(typeof(Int16), OpCodes.Stind_I2);
			Add(typeof(Int32), OpCodes.Stind_I4);
			Add(typeof(Int64), OpCodes.Stind_I8);
			Add(typeof(float), OpCodes.Stind_R4);
			Add(typeof(double), OpCodes.Stind_R8);
			Add(typeof(byte), OpCodes.Stind_I1);
			Add(typeof(UInt16), OpCodes.Stind_I2);
			Add(typeof(UInt32), OpCodes.Stind_I4);
			Add(typeof(UInt64), OpCodes.Stind_I8);
		}

		public new OpCode this[Type type]
		{
			get
			{
				if (TryGetValue(type, out var opCode))
				{
					return opCode;
				}
				return EmptyOpCode;
			}
		}

		public static OpCode EmptyOpCode
		{
			get { return emptyOpCode; }
		}

		public static StindOpCodesDictionary Instance
		{
			get { return dict; }
		}
	}
}