// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

#nullable enable

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	[DebuggerDisplay("{field.Name} ({field.FieldType})")]
	internal class FieldReference : Reference
	{
		private readonly FieldInfo field;
		private readonly FieldBuilder? fieldBuilder;
		private readonly IExpression? instance;

		public FieldReference(FieldInfo field, IExpression? instance = null)
			: base(field.FieldType)
		{
			this.field = field;
			this.instance = instance ?? ThisExpression.Instance;
			if ((field.Attributes & FieldAttributes.Static) != 0)
			{
				this.instance = null;
			}
		}

		public FieldReference(FieldBuilder fieldBuilder, IExpression? instance = null)
			: base(fieldBuilder.FieldType)
		{
			this.fieldBuilder = fieldBuilder;
			field = fieldBuilder;
			this.instance = instance ?? ThisExpression.Instance;
			if ((fieldBuilder.Attributes & FieldAttributes.Static) != 0)
			{
				this.instance = null;
			}
		}

		public FieldBuilder? FieldBuilder
		{
			get { return fieldBuilder; }
		}

		public FieldInfo Reference
		{
			get { return @field; }
		}

		public override void EmitAddress(ILGenerator gen)
		{
			if (instance == null)
			{
				gen.Emit(OpCodes.Ldsflda, Reference);
			}
			else
			{
				instance.Emit(gen);
				gen.Emit(OpCodes.Ldflda, Reference);
			}
		}

		public override void Emit(ILGenerator gen)
		{
			if (instance == null)
			{
				gen.Emit(OpCodes.Ldsfld, Reference);
			}
			else
			{
				instance.Emit(gen);
				gen.Emit(OpCodes.Ldfld, Reference);
			}
		}

		public override void EmitStore(IExpression value, ILGenerator gen)
		{
			if (instance == null)
			{
				value.Emit(gen);
				gen.Emit(OpCodes.Stsfld, Reference);
			}
			else
			{
				instance.Emit(gen);
				value.Emit(gen);
				gen.Emit(OpCodes.Stfld, Reference);
			}
		}
	}
}