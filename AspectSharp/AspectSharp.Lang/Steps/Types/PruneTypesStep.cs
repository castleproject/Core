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

namespace AspectSharp.Lang.Steps.Types
{
	using System;

	using AopAlliance.Intercept;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for PruneTypesStep.
	/// </summary>
	public class PruneTypesStep : AbstractVisitorStep
	{
		private static readonly String MIXIN_CANT_BE_INTERFACE = "The specified type for a mixin is an interface which is invalid";
		private static readonly String INVALID_INTERCEPTOR = "The specified type for an interceptor doesn't implement the interceptor interface";

		public override void Process(Context context, EngineConfiguration conf)
		{
			Init(context);
			Visit(conf);

			if (!context.HasErrors)
			{
				base.Process(context, conf);
			}
		}

		#region AbstractVisitorStep overrides

		public override void OnTargetTypeDefinition(TargetTypeDefinition targetType)
		{
			if (targetType.TargetStrategy == TargetStrategyEnum.SingleType)
			{
				AssertNotInterface(
					targetType.LexicalInfo, targetType.SingleType.ResolvedType,
					MIXIN_CANT_BE_INTERFACE);
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Namespace)
			{
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Assignable)
			{
			}
			else if (targetType.TargetStrategy == TargetStrategyEnum.Custom)
			{
				// TODO: Move IClassMatcher and etc to another assembly
				// to prevent cyclic references.
//				AssertAssignableFrom(  
//					targetType.LexicalInfo, typeof(IClassMatcher), 
//					targetType.SingleType.ResolvedType, MIXIN_CANT_BE_INTERFACE);
			}
		}

		public override void OnMixinDefinition(MixinDefinition mixin)
		{
			AssertNotInterface(mixin.LexicalInfo, mixin.TypeReference.ResolvedType, MIXIN_CANT_BE_INTERFACE);
		}

		public override void OnInterceptorDefinition(InterceptorDefinition interceptor)
		{
			AssertIsInterceptor(interceptor.LexicalInfo, interceptor.TypeReference.ResolvedType, INVALID_INTERCEPTOR);
		}

		public override void OnMixinEntryDefinition(MixinEntryDefinition mixin)
		{
			AssertNotInterface(mixin.LexicalInfo, mixin.TypeReference.ResolvedType, MIXIN_CANT_BE_INTERFACE);
		}

		public override void OnInterceptorEntryDefinition(InterceptorEntryDefinition interceptor)
		{
			AssertIsInterceptor(interceptor.LexicalInfo, interceptor.TypeReference.ResolvedType, INVALID_INTERCEPTOR);
		}

		#endregion

		private void AssertNotInterface(LexicalInfo info, Type type, String message)
		{
			if (type.IsInterface)
			{
				Context.RaiseErrorEvent(info, message);
			}
		}

		private void AssertIsInterceptor(LexicalInfo info, Type type, String message)
		{
			if (!typeof (IInterceptor).IsAssignableFrom(type))
			{
				Context.RaiseErrorEvent(info, message);
			}
		}
	}
}