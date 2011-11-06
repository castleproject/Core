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

namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
	using System.Reflection.Emit;

	public class IfNullExpression : Expression
	{
		private readonly Expression ifNotNull;
		private readonly Expression ifNull;
		private readonly Reference reference;

		public IfNullExpression(Reference reference, Expression ifNull, Expression ifNotNull)
		{
			this.reference = reference;
			this.ifNull = ifNull;
			this.ifNotNull = ifNotNull;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);
			var notNull = gen.DefineLabel();
			gen.Emit(OpCodes.Brtrue_S, notNull);
			ifNull.Emit(member, gen);
			gen.MarkLabel(notNull);
			ifNotNull.Emit(member, gen);
		}
	}
}