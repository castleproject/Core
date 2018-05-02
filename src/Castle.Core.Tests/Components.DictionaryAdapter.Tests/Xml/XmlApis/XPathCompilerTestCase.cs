// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class XPathCompilerTestCase
	{
		[Test]
		public void RequiresPath()
		{
			Assert.Throws<ArgumentNullException>(() =>
				XPathCompiler.Compile(null));
		}

		[Test]
		public void LocalName()
		{
			var p = XPathCompiler.Compile("aa");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);
		}

		[Test]
		public void QualifiedName()
		{
			var p = XPathCompiler.Compile("pp:aa");

			Assert.NotNull(p.Path);
			Assert.AreEqual("pp:aa", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("pp:aa", s.Path.Expression);
			Assert.AreEqual("pp", s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);
		}

		[Test]
		public void QualifiedAttributeName()
		{
			var p = XPathCompiler.Compile("@pp:aa");

			Assert.NotNull(p.Path);
			Assert.AreEqual("@pp:aa", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("@pp:aa", s.Path.Expression);
			Assert.AreEqual("pp", s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.True(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);
		}

		[Test]
		public void MultipleSteps()
		{
			var p = XPathCompiler.Compile("aa/bb");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa/bb", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(2, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);

			s = s.NextStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("bb", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("bb", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.AreSame(p.FirstStep, s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);
		}

		[Test]
		public void MultipleStepsWithSelfReference()
		{
			var p = XPathCompiler.Compile("aa/./bb");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa/./bb", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(2, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);

			s = s.NextStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("bb", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("bb", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.AreSame(p.FirstStep, s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			CollectionAssert.IsEmpty(s.Dependencies);
		}

		[Test]
		public void PredicateWithSelfReference()
		{
			var p = XPathCompiler.Compile("aa[.]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[.]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[.]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.IsNull(n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateWithLocalName()
		{
			var p = XPathCompiler.Compile("aa[bb]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateWithQualifiedName()
		{
			var p = XPathCompiler.Compile("aa[pp:bb]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[pp:bb]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[pp:bb]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.AreEqual("pp", n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateWithQualifiedAttributeName()
		{
			var p = XPathCompiler.Compile("aa[@pp:bb]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[@pp:bb]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[@pp:bb]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.AreEqual("pp", n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.True(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateComparisonToSingleQuoteString()
		{
			var p = XPathCompiler.Compile("aa[bb/cc='1']");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb/cc='1']", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb/cc='1']", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = n.NextNode;
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.NotNull(n.Value);
			Assert.AreEqual("'1'", n.Value.Expression);
			Assert.IsNull(n.NextNode);
			Assert.AreSame(s.Dependencies[0], n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateComparisonToDoubleQuoteString()
		{
			var p = XPathCompiler.Compile("aa[bb/cc=\"1\"]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb/cc=\"1\"]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb/cc=\"1\"]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = n.NextNode;
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.NotNull(n.Value);
			Assert.AreEqual("\"1\"", n.Value.Expression);
			Assert.IsNull(n.NextNode);
			Assert.AreSame(s.Dependencies[0], n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateComparisonToVariable()
		{
			var p = XPathCompiler.Compile("aa[bb/cc=$dd]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb/cc=$dd]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb/cc=$dd]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = n.NextNode;
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.NotNull(n.Value);
			Assert.AreEqual("$dd", n.Value.Expression);
			Assert.IsNull(n.NextNode);
			Assert.AreSame(s.Dependencies[0], n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateWithComparisonToSelf()
		{
			var p = XPathCompiler.Compile("aa[.='1']");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[.='1']", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[.='1']", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.NotNull(s.Value);
			Assert.AreEqual("'1'", s.Value.Expression);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.IsNull(n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.NotNull(n.Value);
			Assert.AreEqual("'1'", n.Value.Expression);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateCommutativeComparison()
		{
			var p = XPathCompiler.Compile("aa[$dd=bb/cc]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[$dd=bb/cc]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[$dd=bb/cc]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = n.NextNode;
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.NotNull(n.Value);
			Assert.AreEqual("$dd", n.Value.Expression);
			Assert.IsNull(n.NextNode);
			Assert.AreSame(s.Dependencies[0], n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void PredicateWithMultipleTerms()
		{
			var p = XPathCompiler.Compile("aa[bb and cc]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb and cc]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb and cc]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(2, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = s.Dependencies[1];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void MultiplePredicates()
		{
			var p = XPathCompiler.Compile("aa[bb][cc]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb][cc]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb][cc]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(2, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);

			n = s.Dependencies[1];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void NestedPredicates()
		{
			var p = XPathCompiler.Compile("aa[bb[cc]]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("aa[bb[cc]]", p.Path.Expression);
			Assert.True(p.IsCreatable);
			Assert.AreEqual(1, p.Depth);

			var s = p.FirstStep;
			Assert.NotNull(s);
			Assert.NotNull(s.Path);
			Assert.AreEqual("aa[bb[cc]]", s.Path.Expression);
			Assert.IsNull(s.Prefix);
			Assert.AreEqual("aa", s.LocalName);
			Assert.False(s.IsAttribute);
			Assert.IsNull(s.Value);
			Assert.IsNull(s.NextStep);
			Assert.AreSame(s.NextStep, s.NextNode);
			Assert.IsNull(s.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			var n = s.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("bb", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(s.Dependencies);
			Assert.AreEqual(1, s.Dependencies.Count);

			n = n.Dependencies[0];
			Assert.NotNull(n);
			Assert.IsNull(n.Prefix);
			Assert.AreEqual("cc", n.LocalName);
			Assert.False(n.IsAttribute);
			Assert.IsNull(n.Value);
			Assert.IsNull(n.NextNode);
			Assert.IsNull(n.PreviousNode);
			Assert.NotNull(n.Dependencies);
			CollectionAssert.IsEmpty(n.Dependencies);
		}

		[Test]
		public void Node_IsSimple_NoNextNodeOrDependency()
		{
			var p = XPathCompiler.Compile("a");

			Assert.True(p.FirstStep.IsSimple);
		}

		[Test]
		public void Node_IsSimple_NextNode()
		{
			var p = XPathCompiler.Compile("a/b");

			Assert.False(p.FirstStep.IsSimple);
			Assert.True(p.FirstStep.NextStep.IsSimple);
		}

		[Test]
		public void Node_IsSimple_Dependency()
		{
			var p = XPathCompiler.Compile("a[b='v']");

			Assert.False(p.FirstStep.IsSimple);
			Assert.True(p.FirstStep.Dependencies[0].IsSimple);
		}

		[Test]
		public void Node_IsSimple_SelfReference()
		{
			var p = XPathCompiler.Compile("a[.='v']");

			Assert.True(p.FirstStep.IsSimple);
			Assert.True(p.FirstStep.Dependencies[0].IsSimple);
		}

		[Test]
		public void UnsupportedLocalName()
		{
			var p = XPathCompiler.Compile("*");

			Assert.NotNull(p.Path);
			Assert.AreEqual("*", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedLocalName_Attribute()
		{
			var p = XPathCompiler.Compile("@*");

			Assert.NotNull(p.Path);
			Assert.AreEqual("@*", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedLocalName_Child()
		{
			var p = XPathCompiler.Compile("a/*");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a/*", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedLocalName_Predicate()
		{
			var p = XPathCompiler.Compile("a[*]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[*]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedLocalName_PredicateAttribute()
		{
			var p = XPathCompiler.Compile("a[@*]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[@*]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedLocalName_PredicateChild()
		{
			var p = XPathCompiler.Compile("a[b/*]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[b/*]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedQualifiedName()
		{
			var p = XPathCompiler.Compile("a:*");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a:*", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void AttributeParent()
		{
			var p = XPathCompiler.Compile("@a/b");

			Assert.NotNull(p.Path);
			Assert.AreEqual("@a/b", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void AttributeParent_InPredicate()
		{
			var p = XPathCompiler.Compile("a[@b/c]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[@b/c]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedOperator_LeftToRight()
		{
			var p = XPathCompiler.Compile("a[b or c]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[b or c]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedComparand_LeftToRight()
		{
			var p = XPathCompiler.Compile("a[b=c]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[b=c]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedOperator_RightToLeft()
		{
			var p = XPathCompiler.Compile("a[$b or c]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[$b or c]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedComparand_RightToLeft()
		{
			var p = XPathCompiler.Compile("a[$b=$c]");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[$b=$c]", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void MultipleComparisonsWithSelf()
		{
			var p = XPathCompiler.Compile("a[.='1'][.='2']");

			Assert.NotNull(p.Path);
			Assert.AreEqual("a[.='1'][.='2']", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}

		[Test]
		public void UnsupportedTrailingOperators()
		{
			var p = XPathCompiler.Compile("f()");

			Assert.NotNull(p.Path);
			Assert.AreEqual("f()", p.Path.Expression);
			Assert.False(p.IsCreatable);
			Assert.AreEqual(0, p.Depth);
			Assert.IsNull(p.FirstStep);
		}
	}
}
