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

namespace AspectSharp.Lang.Steps.Semantic
{
	using System;
	using System.Collections;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for SemanticAnalizerStep.
	/// </summary>
	public class SemanticAnalizerStep : SemanticAnalizerBase
	{
		private IDictionary _aspectNames = new Hashtable();
		private IDictionary _globalMixins;
		private IDictionary _globalInterceptors;

		public override void Process(Context context, EngineConfiguration conf)
		{
			base.Init(context);

			CheckImports(conf.Imports);
			CheckGlobals(conf);
			CheckAspects(conf.Aspects);

			if (!context.HasErrors)
			{
				base.Process(context, conf);
			}
		}

		private void CheckGlobals(EngineConfiguration conf)
		{
			CheckMixins(conf.Mixins.ToDictionary());
			CheckInterceptors(conf.Interceptors.ToDictionary());
		}

		private void CheckMixins(IDictionary types)
		{
			AssertEntriesAreValid(types);
			_globalMixins = types;
		}

		private void CheckInterceptors(IDictionary types)
		{
			AssertEntriesAreValid(types);
			_globalInterceptors = types;
		}

		private void CheckAspects(AspectDefinitionCollection aspects)
		{
			foreach(AspectDefinition aspect in aspects)
			{
				AssertUnique( _aspectNames, aspect, aspect.Name, "The name given to an aspect must be unique" );
				AssertNotNull( aspect, aspect.TargetType, "Aspect must have a target type" );
				CheckIncludes( aspect.Mixins );
				CheckPointcuts( aspect.PointCuts );
			}
		}

		private void CheckPointcuts(PointCutDefinitionCollection cuts)
		{
			IDictionary pointcutsDefined = new Hashtable();

			foreach( PointCutDefinition pointcut in cuts )
			{
				AssertUnique( pointcutsDefined, pointcut, pointcut, "Duplicated pointcut definition found" );

				if (pointcut.Flags == (PointCutFlags.Property | PointCutFlags.PropertyRead) || 
					pointcut.Flags == (PointCutFlags.Property | PointCutFlags.PropertyWrite))
				{
					RaiseErrorEvent(pointcut.LexicalInfo, "Meaningless declaration. A pointcut to a property can't be combined with property read or write. This is implied");
				}

				CheckAdvices(pointcut.Advices);
			}
		}

		private void CheckAdvices(InterceptorDefinitionCollection advices)
		{
			IDictionary advicesDefined = new Hashtable();

			foreach(InterceptorDefinition advice in advices)
			{
				if (advice.TypeReference.TargetType == TargetTypeEnum.Link)
				{
					AssertKeyExists( _globalMixins, advice, advice.TypeReference.LinkRef, "The referenced mixin is not declared in the global mixins section" );
				}

				AssertUnique( advicesDefined, advice, advice.TypeReference.ToString(), "Duplicated advices found" );
			}
		}

		private void CheckIncludes( MixinDefinitionCollection includes )
		{
			IDictionary names = new Hashtable();

			foreach( MixinDefinition type in includes )
			{
				AssertNotNull( type, type.TypeReference.ToString(), "Type name must be specified as as 'Typename in AssemblyName' or as a reference to a global type declared" );

				if (type.TypeReference.TargetType == TargetTypeEnum.Link)
				{
					AssertKeyExists( _globalMixins, type, type.TypeReference.LinkRef, "The referenced mixin is not declared in the global mixins section" );
				}

				AssertUnique( names, type, type.TypeReference.ToString(), "You shouldn't include the same mixin more than one time" );
			}
		}

		private void CheckImports(ImportDirectiveCollection imports)
		{
			// Nothing to do .. yet
		}
	}
}
