// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System;
    using System.Collections.Generic;
    using MarkupTransformers;
    using PreCompilationSteps;

	public static class Resolve
	{
		static Type preCompilationStepsProviderType;
		static Type markupTransformersProviderType;

		static Resolve()
		{
			InitializeToDefaults();
		}

		public static IPreCompilationStepsProvider PreCompilationStepsProvider
		{
			get { return (IPreCompilationStepsProvider)Activator.CreateInstance(preCompilationStepsProviderType); }
		}

		public static IMarkupTransformersProvider MarkupTransformersProvider
		{
			get { return (IMarkupTransformersProvider)Activator.CreateInstance(markupTransformersProviderType); }
		}

		public static void Initialize(IDictionary<Type, Type> customProviders)
		{
			InitializeToDefaults();
			
			if (customProviders == null)
				return;

			if (customProviders.ContainsKey(typeof(IPreCompilationStepsProvider)))
				preCompilationStepsProviderType = customProviders[typeof(IPreCompilationStepsProvider)];

			if (customProviders.ContainsKey(typeof(IMarkupTransformersProvider)))
				markupTransformersProviderType = customProviders[typeof(IMarkupTransformersProvider)];
		}

		static void InitializeToDefaults()
		{
			preCompilationStepsProviderType = typeof(DefaultPreCompilationStepsProvider);
			markupTransformersProviderType = typeof(DefaultMarkupTransformersProvider);
		}
	}
}
