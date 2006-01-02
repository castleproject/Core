// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using AspectSharp.Core.Matchers;
	using AspectSharp.Lang.AST;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for SingleTypeMatcherTestCase.
	/// </summary>
	[TestFixture]
	public class SingleTypeMatcherTestCase
	{
		[Test]
		public void MatchSingle()
		{
			AspectDefinition def = new AspectDefinition(LexicalInfo.Empty, "Wilber");

			TypeReference typeRef = new TypeReference(LexicalInfo.Empty, "");
			typeRef.ResolvedType = typeof(Stack);

			def.TargetType = new TargetTypeDefinition( typeRef );

			Assert.IsFalse( SingleTypeMatcher.Instance.Match( typeof(Object), def ) );
			Assert.IsFalse( SingleTypeMatcher.Instance.Match( typeof(IList), def ) );
			Assert.IsFalse( SingleTypeMatcher.Instance.Match( typeof(ArrayList), def ) );
			Assert.IsTrue( SingleTypeMatcher.Instance.Match( typeof(Stack), def ) );
		}
	}
}
