﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT && !MONO && !NETCORE // Until support for other platforms is verified
namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Linq;
	using Castle.Components.DictionaryAdapter.Xml;
	using Xunit;

		public class XPathCompilerTestCase
	{
		[Fact]
		public void RequiresPath()
		{
			Assert.Throws<ArgumentNullException>(() =>
				XPathCompiler.Compile(null));
		}

		[Fact]
		public void LocalName()
		{
			var p = XPathCompiler.Compile("aa");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void QualifiedName()
		{
			var p = XPathCompiler.Compile("pp:aa");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("pp:aa"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("pp:aa"));
			Assert.That(s.Prefix,          Is.EqualTo("pp"));
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void QualifiedAttributeName()
		{
			var p = XPathCompiler.Compile("@pp:aa");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("@pp:aa"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("@pp:aa"));
			Assert.That(s.Prefix,          Is.EqualTo("pp"));
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.True);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void MultipleSteps()
		{
			var p = XPathCompiler.Compile("aa/bb");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa/bb"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(2));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);

			s = s.NextStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("bb"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("bb"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.SameAs(p.FirstStep));
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void MultipleStepsWithSelfReference()
		{
			var p = XPathCompiler.Compile("aa/./bb");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa/./bb"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(2));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);

			s = s.NextStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("bb"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("bb"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.SameAs(p.FirstStep));
			Assert.That(s.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithSelfReference()
		{
			var p = XPathCompiler.Compile("aa[.]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[.]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[.]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.Null);
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithLocalName()
		{
			var p = XPathCompiler.Compile("aa[bb]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[bb]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[bb]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithQualifiedName()
		{
			var p = XPathCompiler.Compile("aa[pp:bb]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[pp:bb]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[pp:bb]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.EqualTo("pp"));
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithQualifiedAttributeName()
		{
			var p = XPathCompiler.Compile("aa[@pp:bb]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[@pp:bb]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[@pp:bb]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.EqualTo("pp"));
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.True);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateComparisonToSingleQuoteString()
		{
			var p = XPathCompiler.Compile("aa[bb/cc='1']");

			Assert.That(p.Path,             Is.Not.Null);
			Assert.That(p.Path.Expression,  Is.EqualTo("aa[bb/cc='1']"));
			Assert.That(p.IsCreatable,      Is.True);
			Assert.That(p.Depth,            Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                  Is.Not.Null);
			Assert.That(s.Path,             Is.Not.Null);
			Assert.That(s.Path.Expression,  Is.EqualTo("aa[bb/cc='1']"));
			Assert.That(s.Prefix,           Is.Null);
			Assert.That(s.LocalName,        Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,      Is.False);
			Assert.That(s.Value,            Is.Null);
			Assert.That(s.NextStep,         Is.Null);
			Assert.That(s.NextNode,         Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,     Is.Null);
			Assert.That(s.Dependencies,     Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Null);
			Assert.That(n.PreviousNode,     Is.Null);
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);

			n = n.NextNode;
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Not.Null);
			Assert.That(n.Value.Expression, Is.EqualTo("'1'"));
			Assert.That(n.NextNode,         Is.Null);
			Assert.That(n.PreviousNode,     Is.SameAs(s.Dependencies[0]));
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateComparisonToDoubleQuoteString()
		{
			var p = XPathCompiler.Compile("aa[bb/cc=\"1\"]");

			Assert.That(p.Path,             Is.Not.Null);
			Assert.That(p.Path.Expression,  Is.EqualTo("aa[bb/cc=\"1\"]"));
			Assert.That(p.IsCreatable,      Is.True);
			Assert.That(p.Depth,            Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                  Is.Not.Null);
			Assert.That(s.Path,             Is.Not.Null);
			Assert.That(s.Path.Expression,  Is.EqualTo("aa[bb/cc=\"1\"]"));
			Assert.That(s.Prefix,           Is.Null);
			Assert.That(s.LocalName,        Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,      Is.False);
			Assert.That(s.Value,            Is.Null);
			Assert.That(s.NextStep,         Is.Null);
			Assert.That(s.NextNode,         Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,     Is.Null);
			Assert.That(s.Dependencies,     Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Null);
			Assert.That(n.PreviousNode,     Is.Null);
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);

			n = n.NextNode;
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Not.Null);
			Assert.That(n.Value.Expression, Is.EqualTo("\"1\""));
			Assert.That(n.NextNode,         Is.Null);
			Assert.That(n.PreviousNode,     Is.SameAs(s.Dependencies[0]));
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateComparisonToVariable()
		{
			var p = XPathCompiler.Compile("aa[bb/cc=$dd]");

			Assert.That(p.Path,             Is.Not.Null);
			Assert.That(p.Path.Expression,  Is.EqualTo("aa[bb/cc=$dd]"));
			Assert.That(p.IsCreatable,      Is.True);
			Assert.That(p.Depth,            Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                  Is.Not.Null);
			Assert.That(s.Path,             Is.Not.Null);
			Assert.That(s.Path.Expression,  Is.EqualTo("aa[bb/cc=$dd]"));
			Assert.That(s.Prefix,           Is.Null);
			Assert.That(s.LocalName,        Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,      Is.False);
			Assert.That(s.Value,            Is.Null);
			Assert.That(s.NextStep,         Is.Null);
			Assert.That(s.NextNode,         Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,     Is.Null);
			Assert.That(s.Dependencies,     Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Null);
			Assert.That(n.PreviousNode,     Is.Null);
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);

			n = n.NextNode;
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Not.Null);
			Assert.That(n.Value.Expression, Is.EqualTo("$dd"));
			Assert.That(n.NextNode,         Is.Null);
			Assert.That(n.PreviousNode,     Is.SameAs(s.Dependencies[0]));
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithComparisonToSelf()
		{
			var p = XPathCompiler.Compile("aa[.='1']");

			Assert.That(p.Path,             Is.Not.Null);
			Assert.That(p.Path.Expression,  Is.EqualTo("aa[.='1']"));
			Assert.That(p.IsCreatable,      Is.True);
			Assert.That(p.Depth,            Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                  Is.Not.Null);
			Assert.That(s.Path,             Is.Not.Null);
			Assert.That(s.Path.Expression,  Is.EqualTo("aa[.='1']"));
			Assert.That(s.Prefix,           Is.Null);
			Assert.That(s.LocalName,        Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,      Is.False);
			Assert.That(s.Value,            Is.Not.Null);
			Assert.That(s.Value.Expression, Is.EqualTo("'1'"));
			Assert.That(s.NextStep,         Is.Null);
			Assert.That(s.NextNode,         Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,     Is.Null);
			Assert.That(s.Dependencies,     Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.Null);
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Not.Null);
			Assert.That(n.Value.Expression, Is.EqualTo("'1'"));
			Assert.That(n.NextNode,         Is.Null);
			Assert.That(n.PreviousNode,     Is.Null);
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateCommutativeComparison()
		{
			var p = XPathCompiler.Compile("aa[$dd=bb/cc]");

			Assert.That(p.Path,             Is.Not.Null);
			Assert.That(p.Path.Expression,  Is.EqualTo("aa[$dd=bb/cc]"));
			Assert.That(p.IsCreatable,      Is.True);
			Assert.That(p.Depth,            Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                  Is.Not.Null);
			Assert.That(s.Path,             Is.Not.Null);
			Assert.That(s.Path.Expression,  Is.EqualTo("aa[$dd=bb/cc]"));
			Assert.That(s.Prefix,           Is.Null);
			Assert.That(s.LocalName,        Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,      Is.False);
			Assert.That(s.Value,            Is.Null);
			Assert.That(s.NextStep,         Is.Null);
			Assert.That(s.NextNode,         Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,     Is.Null);
			Assert.That(s.Dependencies,     Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Null);
			Assert.That(n.PreviousNode,     Is.Null);
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);

			n = n.NextNode;
			Assert.That(n,                  Is.Not.Null);
			Assert.That(n.Prefix,           Is.Null);
			Assert.That(n.LocalName,        Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,      Is.False);
			Assert.That(n.Value,            Is.Not.Null);
			Assert.That(n.Value.Expression, Is.EqualTo("$dd"));
			Assert.That(n.NextNode,         Is.Null);
			Assert.That(n.PreviousNode,     Is.SameAs(s.Dependencies[0]));
			Assert.That(n.Dependencies,     Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void PredicateWithMultipleTerms()
		{
			var p = XPathCompiler.Compile("aa[bb and cc]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[bb and cc]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[bb and cc]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(2));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);

			n = s.Dependencies[1];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void MultiplePredicates()
		{
			var p = XPathCompiler.Compile("aa[bb][cc]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[bb][cc]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[bb][cc]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(2));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);

			n = s.Dependencies[1];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void NestedPredicates()
		{
			var p = XPathCompiler.Compile("aa[bb[cc]]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("aa[bb[cc]]"));
			Assert.That(p.IsCreatable,     Is.True);
			Assert.That(p.Depth,           Is.EqualTo(1));

			var s = p.FirstStep;
			Assert.That(s,                 Is.Not.Null);
			Assert.That(s.Path,            Is.Not.Null);
			Assert.That(s.Path.Expression, Is.EqualTo("aa[bb[cc]]"));
			Assert.That(s.Prefix,          Is.Null);
			Assert.That(s.LocalName,       Is.EqualTo("aa"));
			Assert.That(s.IsAttribute,     Is.False);
			Assert.That(s.Value,           Is.Null);
			Assert.That(s.NextStep,        Is.Null);
			Assert.That(s.NextNode,        Is.SameAs(s.NextStep));
			Assert.That(s.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			var n = s.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("bb"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(s.Dependencies,    Is.Not.Null & Has.Count.EqualTo(1));

			n = n.Dependencies[0];
			Assert.That(n,                 Is.Not.Null);
			Assert.That(n.Prefix,          Is.Null);
			Assert.That(n.LocalName,       Is.EqualTo("cc"));
			Assert.That(n.IsAttribute,     Is.False);
			Assert.That(n.Value,           Is.Null);
			Assert.That(n.NextNode,        Is.Null);
			Assert.That(n.PreviousNode,    Is.Null);
			Assert.That(n.Dependencies,    Is.Not.Null & Is.Empty);
		}

		[Fact]
		public void Node_IsSimple_NoNextNodeOrDependency()
		{
			var p = XPathCompiler.Compile("a");

			Assert.That(p.FirstStep.IsSimple, Is.True);
		}

		[Fact]
		public void Node_IsSimple_NextNode()
		{
			var p = XPathCompiler.Compile("a/b");

			Assert.That(p.FirstStep         .IsSimple, Is.False);
			Assert.That(p.FirstStep.NextStep.IsSimple, Is.True);
		}

		[Fact]
		public void Node_IsSimple_Dependency()
		{
			var p = XPathCompiler.Compile("a[b='v']");

			Assert.That(p.FirstStep                .IsSimple, Is.False);
			Assert.That(p.FirstStep.Dependencies[0].IsSimple, Is.True);
		}

		[Fact]
		public void Node_IsSimple_SelfReference()
		{
			var p = XPathCompiler.Compile("a[.='v']");

			Assert.That(p.FirstStep                .IsSimple, Is.True);
			Assert.That(p.FirstStep.Dependencies[0].IsSimple, Is.True);
		}

		[Fact]
		public void UnsupportedLocalName()
		{
			var p = XPathCompiler.Compile("*");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("*"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedLocalName_Attribute()
		{
			var p = XPathCompiler.Compile("@*");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("@*"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedLocalName_Child()
		{
			var p = XPathCompiler.Compile("a/*");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a/*"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedLocalName_Predicate()
		{
			var p = XPathCompiler.Compile("a[*]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[*]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedLocalName_PredicateAttribute()
		{
			var p = XPathCompiler.Compile("a[@*]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[@*]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedLocalName_PredicateChild()
		{
			var p = XPathCompiler.Compile("a[b/*]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[b/*]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedQualifiedName()
		{
			var p = XPathCompiler.Compile("a:*");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a:*"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void AttributeParent()
		{
			var p = XPathCompiler.Compile("@a/b");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("@a/b"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void AttributeParent_InPredicate()
		{
			var p = XPathCompiler.Compile("a[@b/c]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[@b/c]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedOperator_LeftToRight()
		{
			var p = XPathCompiler.Compile("a[b or c]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[b or c]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedComparand_LeftToRight()
		{
			var p = XPathCompiler.Compile("a[b=c]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[b=c]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedOperator_RightToLeft()
		{
			var p = XPathCompiler.Compile("a[$b or c]");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("a[$b or c]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedComparand_RightToLeft()
		{
			var p = XPathCompiler.Compile("a[$b=$c]");

			Assert.That(p.Path,            Is.Not.Null);	
			Assert.That(p.Path.Expression, Is.EqualTo("a[$b=$c]"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void MultipleComparisonsWithSelf()
		{
			var p = XPathCompiler.Compile("a[.='1'][.='2']");

			Assert.That(p.Path,            Is.Not.Null);	
			Assert.That(p.Path.Expression, Is.EqualTo("a[.='1'][.='2']"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}

		[Fact]
		public void UnsupportedTrailingOperators()
		{
			var p = XPathCompiler.Compile("f()");

			Assert.That(p.Path,            Is.Not.Null);
			Assert.That(p.Path.Expression, Is.EqualTo("f()"));
			Assert.That(p.IsCreatable,     Is.False);
			Assert.That(p.Depth,           Is.EqualTo(0));
			Assert.That(p.FirstStep,       Is.Null);
		}
	}
}
#endif
