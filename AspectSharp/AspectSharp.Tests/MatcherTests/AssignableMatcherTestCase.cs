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

namespace AspectSharp.Tests.MatcherTests
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using AspectSharp.Core.Matchers;
	using AspectSharp.Lang.AST;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for AssignableMatcherTestCase.
	/// </summary>
	[TestFixture]
	public class AssignableMatcherTestCase
	{
		[Test]
		public void MatchAssignable()
		{
			AspectDefinition def = new AspectDefinition(LexicalInfo.Empty, "Wilber");

			def.TargetType = new TargetTypeDefinition();
			def.TargetType.TargetStrategy = TargetStrategyEnum.Assignable;
			
			TypeReference typeRef = new TypeReference(LexicalInfo.Empty, "");
			typeRef.ResolvedType = typeof(IList);

			def.TargetType.AssignType = typeRef;

			Assert.IsFalse( AssignableMatcher.Instance.Match( typeof(Object), def ) );
			Assert.IsFalse( AssignableMatcher.Instance.Match( typeof(HybridDictionary), def ) );
			Assert.IsTrue( AssignableMatcher.Instance.Match( typeof(IList), def ) );
			Assert.IsTrue( AssignableMatcher.Instance.Match( typeof(ArrayList), def ) );
			Assert.IsFalse( AssignableMatcher.Instance.Match( typeof(Stack), def ) );
		}
	}
}
