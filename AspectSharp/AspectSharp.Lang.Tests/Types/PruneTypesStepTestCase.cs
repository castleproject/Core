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
	using AspectSharp.Lang.Steps.Types;
	using AspectSharp.Lang.Steps.Semantic;

	/// <summary>
	/// Summary description for PruneTypesStepTestCase.
	/// </summary>
	[TestFixture]
	public class PruneTypesStepTestCase : ParserTestCaseBase
	{
		[Test]
		public void InvalidInterceptor()
		{
			String content = " " + 
				"interceptors \r\n" +
				"[" + 
				"\"customer\" : System.Collections.Hashtable" +
				"]"; 

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsTrue(_context.HasErrors);
			AssertOutput( "The specified type for an interceptor doesn't implement the interceptor interface;" );
		}

		[Test]
		public void ValidInterceptor()
		{
			String content = " " + 
				"interceptors \r\n" +
				"[" + 
				"\"customer\" : AspectSharp.Lang.Tests.Types.Interceptors.DummyInterceptor in AspectSharp.Lang.Tests" +
				"]"; 

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsFalse(_context.HasErrors);
		}

		[Test]
		public void InvalidMixin()
		{
			String content = " " + 
				"mixins \r\n" +
				"[" + 
				"\"customer\" : System.Collections.IList" +
				"]"; 

			EngineConfiguration conf = ProcessContent(content);
			Assert.IsTrue(_context.HasErrors);
			AssertOutput( "The specified type for a mixin is an interface which is invalid;" );
		}

		protected override void AddSteps(StepChainBuilder chain)
		{
			chain.AddStep(new SemanticAnalizerStep());
			chain.AddStep(new ResolveTypesStep());
			chain.AddStep(new PruneTypesStep());
		}
	}
}
