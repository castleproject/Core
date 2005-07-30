// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for MethodCodeBuilder.
	/// </summary>
	[CLSCompliant(false)]
	public class MethodCodeBuilder : AbstractCodeBuilder
	{
		private Type _baseType;
		private MethodBuilder _methodbuilder;

		public MethodCodeBuilder( Type baseType, MethodBuilder methodbuilder, ILGenerator generator) : 
			base(generator)
		{
			_baseType = baseType;
			_methodbuilder = methodbuilder;
		}

		private MethodBuilder Builder
		{
			get { return _methodbuilder; }
		}

//		public void Nop()
//		{
//			SetNonEmpty();
//
//			if (Builder.ReturnType == typeof(void))
//			{
//				Generator.Emit( OpCodes.Nop );
//			}
//			else
//			{
//				Generator.Emit( LdcOpCodesDictionary.Instance[Builder.ReturnType], 0 );
//			}
//		}
	}
}
