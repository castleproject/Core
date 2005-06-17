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

namespace AspectSharp.Lang.Tests.Types
{
	using System;

	using NUnit.Framework;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.Steps;
	using AspectSharp.Lang.Steps.Semantic;
	using AspectSharp.Lang.Steps.Types;
	using AspectSharp.Lang.Tests.Types.Mixins;
	using AspectSharp.Lang.Tests.Types.Matcher;

	/// <summary>
	/// Summary description for ResolveTypesStepTestCase.
	/// </summary>
	[TestFixture]
	public class ResolveTypesStepTestCase : ParserTestCaseBase
	{
		[Test]
		public void AssembliesResolved()
		{
			String content = "import System.Collection in System";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			foreach(ImportDirective import in conf.Imports)
			{
				Assert.IsNotNull( import.AssemblyReference );
				Assert.IsNotNull( import.AssemblyReference.ResolvedAssembly );
			}
		}

		[Test]
		public void MixinResolvedFullTypeName()
		{
			String content = "mixins \r\n" +
				"[" + 
				"\"customer\" : System.Collections.ArrayList" +
				"]";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);
			MixinEntryDefinition mixin = conf.Mixins[0];
			Assert.IsNotNull( mixin.Key );
			Assert.IsNotNull( mixin.TypeReference );
			Assert.IsNotNull( mixin.TypeReference.ResolvedType );
			Assert.AreEqual( typeof(System.Collections.ArrayList), mixin.TypeReference.ResolvedType );
		}

		[Test]
		public void MixinResolvedUsingImport()
		{
			String content = "import System.Collections " + 
				"mixins \r\n" +
				"[" + 
				"\"customer\" : ArrayList" +
				"]";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);
			MixinEntryDefinition mixin = conf.Mixins[0];
			Assert.IsNotNull( mixin.Key );
			Assert.IsNotNull( mixin.TypeReference );
			Assert.IsNotNull( mixin.TypeReference.ResolvedType );
			Assert.AreEqual( typeof(System.Collections.ArrayList), mixin.TypeReference.ResolvedType );
		}

		[Test]
		public void MixinResolvedFullTypeNameAndAssembly()
		{
			String content = " " + 
				"mixins \r\n" +
				"[" + 
				"\"customer\" : AspectSharp.Lang.Tests.Types.Mixins.MockMixin in AspectSharp.Lang.Tests" +
				"]";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);
			MixinEntryDefinition mixin = conf.Mixins[0];
			Assert.IsNotNull( mixin.Key );
			Assert.IsNotNull( mixin.TypeReference );
			Assert.IsNotNull( mixin.TypeReference.ResolvedType );
			Assert.AreEqual( typeof(MockMixin), mixin.TypeReference.ResolvedType );
		}

		[Test]
		public void GlobalsCorrectlyReferenced()
		{
			String content = " " + 
				"interceptors \r\n" +
				"[" + 
				"\"customer\" : System.Collections.Hashtable" +
				"]" + 
				"mixins \r\n" +
				"[" + 
				"\"customer\" : AspectSharp.Lang.Tests.Types.Mixins.MockMixin in AspectSharp.Lang.Tests" +
				"]" + 
				" " + 
				"aspect McBrother for System.Collections.ArrayList " + 
				" " + 
				" include \"customer\"" + 
				" " + 
				" pointcut method(*)" + 
				"   advice(\"customer\")" + 
				" end" + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];
			MixinDefinition mixin = aspect.Mixins[0];
			InterceptorDefinition interceptor = aspect.PointCuts[0].Advices[0];

			Assert.IsNotNull( mixin.TypeReference );
			Assert.IsNotNull( mixin.TypeReference.ResolvedType );
			Assert.AreEqual( typeof(MockMixin), mixin.TypeReference.ResolvedType );

			Assert.IsNotNull( interceptor.TypeReference );
			Assert.IsNotNull( interceptor.TypeReference.ResolvedType );
			Assert.AreEqual( typeof(System.Collections.Hashtable), interceptor.TypeReference.ResolvedType );
		}

		[Test]
		public void AspectTarget()
		{
			String content = "" +
				"aspect McBrother for AspectSharp.Lang.Tests.Types.Mixins.MockMixin in AspectSharp.Lang.Tests " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.IsNotNull( aspect.TargetType.SingleType );
			Assert.IsNotNull( aspect.TargetType.SingleType.ResolvedType );
			Assert.AreEqual( typeof(MockMixin), aspect.TargetType.SingleType.ResolvedType );
		}

		[Test]
		public void AspectTargetingNamespace()
		{
			String content = "" +
				"aspect McBrother for [AspectSharp.Lang.Tests.Types.Mixins] " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.AreEqual( TargetStrategyEnum.Namespace, aspect.TargetType.TargetStrategy );
			Assert.AreEqual( "AspectSharp.Lang.Tests.Types.Mixins", aspect.TargetType.NamespaceRoot );
			Assert.AreEqual( 0, aspect.TargetType.Excludes.Count );
			Assert.IsNull( aspect.TargetType.SingleType );
		}

		[Test]
		public void AspectTargetingNamespaceWithExcludes()
		{
			String content = "import System.Collections " +
				"aspect McBrother for [System.Collections excludes(Stack) ] " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.AreEqual( TargetStrategyEnum.Namespace, aspect.TargetType.TargetStrategy );
			Assert.AreEqual( "System.Collections", aspect.TargetType.NamespaceRoot );
			Assert.AreEqual( 1, aspect.TargetType.Excludes.Count );
			
			TypeReference typeRef = aspect.TargetType.Excludes[0];
			Assert.AreEqual( typeof(System.Collections.Stack), typeRef.ResolvedType );
		}

		[Test]
		public void AspectTargetingNamespaceWithMoreExcludes()
		{
			String content = "import System.Collections " +
				"aspect McBrother for [System.Collections excludes(Stack; ArrayList) ] " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.AreEqual( TargetStrategyEnum.Namespace, aspect.TargetType.TargetStrategy );
			Assert.AreEqual( "System.Collections", aspect.TargetType.NamespaceRoot );
			Assert.AreEqual( 2, aspect.TargetType.Excludes.Count );
			
			TypeReference typeRef = aspect.TargetType.Excludes[0];
			Assert.AreEqual( typeof(System.Collections.Stack), typeRef.ResolvedType );
			typeRef = aspect.TargetType.Excludes[1];
			Assert.AreEqual( typeof(System.Collections.ArrayList), typeRef.ResolvedType );
		}

		[Test]
		public void AspectTargetingCustom()
		{
			String content = "" +
				"aspect McBrother for [ customMatcher(AspectSharp.Lang.Tests.Types.Matcher.ValidMatcher) ] " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.AreEqual( TargetStrategyEnum.Custom, aspect.TargetType.TargetStrategy );
			Assert.IsNotNull( aspect.TargetType.CustomMatcherType );
			Assert.IsNotNull( aspect.TargetType.CustomMatcherType.ResolvedType );
			Assert.AreEqual( typeof(ValidMatcher), aspect.TargetType.CustomMatcherType.ResolvedType );
		}

		[Test]
		public void AspectTargetingAssignable()
		{
			String content = "" +
				"aspect McBrother for [ assignableFrom(System.Collections.IList) ] " + 
				" " + 
				"end " + 
				" ";

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);

			AspectDefinition aspect = conf.Aspects[0];

			Assert.AreEqual( TargetStrategyEnum.Assignable, aspect.TargetType.TargetStrategy );
			Assert.IsNotNull( aspect.TargetType.AssignType );
			Assert.IsNotNull( aspect.TargetType.AssignType.ResolvedType );
			Assert.AreEqual( typeof(System.Collections.IList), aspect.TargetType.AssignType.ResolvedType );
		}

		protected override void AddSteps(StepChainBuilder chain)
		{
			chain.AddStep(new SemanticAnalizerStep());
			chain.AddStep(new ResolveTypesStep());
		}
	}
}
