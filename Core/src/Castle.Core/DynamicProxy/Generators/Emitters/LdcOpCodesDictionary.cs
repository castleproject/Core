// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	///   s
	///   Provides appropriate Ldc.X opcode for the type of primitive value to be loaded.
	/// </summary>
	public sealed class LdcOpCodesDictionary : Dictionary<Type, OpCode>
	{
		private static readonly LdcOpCodesDictionary dict = new LdcOpCodesDictionary();

		// has to be assigned explicitly to suppress compiler warning
		private static readonly OpCode emptyOpCode = new OpCode();

		private LdcOpCodesDictionary()
		{
			Add(typeof(bool), OpCodes.Ldc_I4);
			Add(typeof(char), OpCodes.Ldc_I4);
			Add(typeof(SByte), OpCodes.Ldc_I4);
			Add(typeof(Int16), OpCodes.Ldc_I4);
			Add(typeof(Int32), OpCodes.Ldc_I4);
			Add(typeof(Int64), OpCodes.Ldc_I8);
			Add(typeof(float), OpCodes.Ldc_R4);
			Add(typeof(double), OpCodes.Ldc_R8);
			Add(typeof(byte), OpCodes.Ldc_I4_0);
			Add(typeof(UInt16), OpCodes.Ldc_I4_0);
			Add(typeof(UInt32), OpCodes.Ldc_I4_0);
			Add(typeof(UInt64), OpCodes.Ldc_I4_0);
		}

		public new OpCode this[Type type]
		{
			get
			{
				if (ContainsKey(type))
				{
					return base[type];
				}
				return EmptyOpCode;
			}
		}

		public static OpCode EmptyOpCode
		{
			get { return emptyOpCode; }
		}

		public static LdcOpCodesDictionary Instance
		{
			get { return dict; }
		}
	}
}