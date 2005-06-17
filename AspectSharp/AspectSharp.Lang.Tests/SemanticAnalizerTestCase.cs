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

namespace AspectSharp.Lang.Tests
{
	using System;
	using System.Text;

	using NUnit.Framework;

	using AspectSharp.Lang.AST;
	using AspectSharp.Lang.Steps;
	using AspectSharp.Lang.Steps.Semantic;

	/// <summary>
	/// Summary description for SemanticAnalizerTestCase.
	/// </summary>
	[TestFixture]
	public class SemanticAnalizerTestCase : ParserTestCaseBase
	{
		[Test]
		public void InvalidMixinsDeclaration()
		{
			String content = "mixins [ \"\" : MyType in MyAssemblye ]";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("A key must be specified to identify the type in the map;");
		}

		[Test]
		public void InvalidInterceptorsDeclaration()
		{
			String content = "interceptors [ \"\" : MyType in MyAssemblye ]";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("A key must be specified to identify the type in the map;");
		}

		[Test]
		public void AspectsWithSameName()
		{
			String content = "aspect McBrother for X end  aspect McBrother for Y end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("The name given to an aspect must be unique;");
		}

		[Test]
		public void IncludingTheSameMixinTwice()
		{
			String content = "aspect McBrother for X " +
				"include Customer " +
				"include Author " +
				"include Customer " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("You shouldn't include the same mixin more than one time;");
		}

		[Test]
		public void CorrectIncludeUsage()
		{
			String content = "aspect McBrother for X " +
				"include Customer " +
				"include Author " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsFalse(_context.HasErrors);
		}

		[Test]
		public void DuplicatePointcuts()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(*) " +
				"  end " +
				" " + 
				"  pointcut method(*) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Duplicated pointcut definition found;");
		}

		[Test]
		public void DuplicatePointcutsWithName()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(* Name) " +
				"  end " +
				" " + 
				"  pointcut method(* Name) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Duplicated pointcut definition found;");
		}

		[Test]
		public void DuplicatePointcutsWithNameAndRetType()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(string Name) " +
				"  end " +
				" " + 
				"  pointcut method(String Name) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Duplicated pointcut definition found;");
		}

		[Test]
		public void DuplicatePointcutsWithNameArgumentsAndRetType()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(string Name(string, int)) " +
				"  end " +
				" " + 
				"  pointcut method(String Name(string, int)) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Duplicated pointcut definition found;");
		}

		[Test]
		public void ValidPointcutsWithNameArgumentsAndRetType()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(string Name(string, int)) " +
				"  end " +
				" " + 
				"  pointcut method(String Name(int, int)) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsFalse(_context.HasErrors);
		}

		[Test]
		public void ValidPointcutsWithName()
		{
			String content = "aspect McBrother for X " +
				"  pointcut method(* Name(string, int)) " +
				"  end " +
				" " + 
				"  pointcut method(* Name()) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsFalse(_context.HasErrors);
		}

		[Test]
		public void InvalidPointcutDeclaration()
		{
			String content = "aspect McBrother for X " +
				"  pointcut property|propertyread(*) " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Meaningless declaration. A pointcut to a property can't be combined with property read or write. This is implied;");
		}

		[Test]
		public void DuplicateInterceptorsInPointcut()
		{
			String content = "aspect McBrother for X " +
				"  pointcut property(* Name) " +
				"    " +
				"    advice(Interceptors.SecurityInterceptor in MyAssembly) " +
				"    advice(Interceptors.SecurityInterceptor in MyAssembly) " +
				"    " +
				"  end " +
				"end";

			Analize( CreateEngineConfiguration( content ) );
			Assert.IsTrue(_context.HasErrors);
			AssertOutput("Duplicated advices found;");
		}

		protected EngineConfiguration CreateEngineConfiguration(String content)
		{
			AspectParser parser = base.CreateParser(content);
			return parser.Parse();
		}

		protected SemanticAnalizerStep Analize(EngineConfiguration conf)
		{
			return Analize(conf, null);
		}

		protected SemanticAnalizerStep Analize(EngineConfiguration conf, IStep next)
		{
			SemanticAnalizerStep analizer = new SemanticAnalizerStep();
			analizer.Next = next;
			_context = new Context();
			_context.Error += new ErrorDelegate(OnError);
			analizer.Process(_context, conf);
			return analizer;
		}
	}
}
