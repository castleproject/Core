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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	[DebuggerDisplay("{field.Name} ({field.FieldType})")]
	internal class FieldReference : Reference
	{
		private readonly FieldInfo field;
		private readonly FieldBuilder fieldBuilder;
		private readonly bool isStatic;

		public FieldReference(FieldInfo field)
			: base(field.FieldType)
		{
			this.field = field;
			if ((field.Attributes & FieldAttributes.Static) != 0)
			{
				isStatic = true;
			}
		}

		public FieldReference(FieldBuilder fieldBuilder)
			: base(fieldBuilder.FieldType)
		{
			this.fieldBuilder = fieldBuilder;
			field = fieldBuilder;
			if ((fieldBuilder.Attributes & FieldAttributes.Static) != 0)
			{
				isStatic = true;
			}
		}

		public FieldBuilder FieldBuilder
		{
			get { return fieldBuilder; }
		}

		public FieldInfo Reference
		{
			get { return @field; }
		}

		public override void LoadAddressOfReference(ILGenerator gen)
		{
			if (isStatic)
			{
				gen.Emit(OpCodes.Ldsflda, Reference);
			}
			else
			{
				gen.Emit(OpCodes.Ldflda, Reference);
			}
		}

		public override void LoadReference(ILGenerator gen)
		{
			var owner = isStatic ? null : ThisExpression.Instance;
			owner?.Emit(gen);
			if (isStatic)
			{
				gen.Emit(OpCodes.Ldsfld, Reference);
			}
			else
			{
				gen.Emit(OpCodes.Ldfld, Reference);
			}
		}

		public override void StoreReference(IExpression expression, ILGenerator gen)
		{
			var owner = isStatic ? null : ThisExpression.Instance;
			owner?.Emit(gen);
			expression.Emit(gen);
			if (isStatic)
			{
				gen.Emit(OpCodes.Stsfld, Reference);
			}
			else
			{
				gen.Emit(OpCodes.Stfld, Reference);
			}
		}
	}
}