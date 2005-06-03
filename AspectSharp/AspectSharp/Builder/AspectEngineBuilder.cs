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

namespace AspectSharp.Builder
{
	using System;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.Steps;
	using AspectSharp.Lang.Steps.Semantic;
	using AspectSharp.Lang.Steps.Types;

	/// <summary>
	/// Summary description for AspectEngineBuilder.
	/// </summary>
	public abstract class AspectEngineBuilder
	{
		private EngineConfiguration _configuration;

		public virtual AspectEngine Build()
		{
			AssertUtil.NotNull( Configuration, "Configuration" );

			ExecuteSteps();

			return new AspectEngine(Configuration);
		}

		protected virtual void ExecuteSteps()
		{
			StepChainBuilder chain = new StepChainBuilder();

			AddSemanticStep(chain);
			AddTypeResolverStep(chain);
			AddPruneTypesStep(chain);

			IStep firstStep = chain.Build();

			Context context = new Context();
			context.Error += new ErrorDelegate(OnError);
			firstStep.Process( context, Configuration );
		}

		protected virtual void AddSemanticStep(StepChainBuilder chain)
		{
			chain.AddStep(new SemanticAnalizerStep());
		}

		protected virtual void AddTypeResolverStep(StepChainBuilder chain)
		{
			chain.AddStep(new ResolveTypesStep());
		}

		protected virtual void AddPruneTypesStep(StepChainBuilder chain)
		{
			chain.AddStep(new PruneTypesStep());
		}

		protected EngineConfiguration Configuration
		{
			get { return _configuration; }
			set { _configuration = value; }
		}

		protected virtual void OnError(LexicalInfo info, String message)
		{
			String detailedMessage = String.Format(
				"{0}. On {1}, Line {2} - from {3} until {4}", message, 
				info.Filename, info.Line, info.StartCol, info.EndCol);

			throw new BuilderException(info, detailedMessage);
		}
	}
}
