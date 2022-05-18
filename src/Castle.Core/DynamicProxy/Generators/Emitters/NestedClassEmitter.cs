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
	using System.Reflection;
	using System.Reflection.Emit;

	internal class NestedClassEmitter : AbstractTypeEmitter
	{
		public NestedClassEmitter(AbstractTypeEmitter mainType, string name, Type baseType, Type[] interfaces)
			: this(
				mainType,
				CreateTypeBuilder(mainType, name, TypeAttributes.Sealed | TypeAttributes.NestedPublic | TypeAttributes.Class,
				                  baseType, interfaces))
		{
		}

		public NestedClassEmitter(AbstractTypeEmitter mainType, string name, TypeAttributes attributes, Type baseType,
		                          Type[] interfaces)
			: this(mainType, CreateTypeBuilder(mainType, name, attributes, baseType, interfaces))
		{
		}

		public NestedClassEmitter(AbstractTypeEmitter mainType, TypeBuilder typeBuilder)
			: base(typeBuilder)
		{
			mainType.AddNestedClass(this);
		}

		private static TypeBuilder CreateTypeBuilder(AbstractTypeEmitter mainType, string name, TypeAttributes attributes,
		                                             Type baseType, Type[] interfaces)
		{
			return mainType.TypeBuilder.DefineNestedType(
				name,
				attributes,
				baseType, interfaces);
		}
	}
}