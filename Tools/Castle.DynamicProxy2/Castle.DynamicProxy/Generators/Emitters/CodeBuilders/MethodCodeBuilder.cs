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

namespace Castle.DynamicProxy.Generators.Emitters.CodeBuilders
{
	using System;
	using System.Reflection.Emit;

	[CLSCompliant(false)]
	public class MethodCodeBuilder : AbstractCodeBuilder
	{
		private readonly Type baseType;
		private readonly MethodBuilder methodbuilder;

		public MethodCodeBuilder(Type baseType, MethodBuilder methodbuilder, ILGenerator generator) : base(generator)
		{
			this.baseType = baseType;
			this.methodbuilder = methodbuilder;
		}

		private MethodBuilder Builder
		{
			get { return methodbuilder; }
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