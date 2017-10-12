// Copyright 2004-2017 Castle Project - http://www.castleproject.org/
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

	public class IfStatement : Statement
	{
		private readonly Expression condition;
		private readonly Statement then;

		public IfStatement(Expression condition, Statement then)
		{
			this.condition = condition;
			this.then = then;
		}

		public override void Emit(IMemberEmitter member, ILGenerator gen)
		{
			var fi = gen.DefineLabel();
			condition.Emit(member, gen);
			gen.Emit(OpCodes.Brfalse_S, fi);
			then.Emit(member, gen);
			gen.MarkLabel(fi);
		}
	}
}
