using System.Reflection.Emit;
// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for OpCodesDictionary.
	/// </summary>
	public sealed class OpCodesDictionary : DictionaryBase
	{
		private static readonly OpCodesDictionary m_dict = new OpCodesDictionary();

		private static readonly OpCode m_emptyOpCode = new OpCode();

		private OpCodesDictionary() : base()
		{
			Dictionary[ typeof (Int16) ] = OpCodes.Ldind_I2;
			Dictionary[ typeof (Int32) ] = OpCodes.Ldind_I4;
			Dictionary[ typeof (Int64) ] = OpCodes.Ldind_I8;
			Dictionary[ typeof (float) ] = OpCodes.Ldind_R4;
			Dictionary[ typeof (double) ] = OpCodes.Ldind_R8;
			Dictionary[ typeof (UInt16) ] = OpCodes.Ldind_U2;
			Dictionary[ typeof (UInt32) ] = OpCodes.Ldind_U4;
			Dictionary[ typeof (bool) ] = OpCodes.Ldind_I4;
		}

		public OpCode this[Type type]
		{
			get
			{
				if (Dictionary.Contains(type))
				{
					return (OpCode) Dictionary[ type ];
				}
				return EmptyOpCode;
			}
		}

		public static OpCodesDictionary Instance
		{
			get { return m_dict; }
		}

		public static OpCode EmptyOpCode
		{
			get { return m_emptyOpCode; }
		}
	}
}