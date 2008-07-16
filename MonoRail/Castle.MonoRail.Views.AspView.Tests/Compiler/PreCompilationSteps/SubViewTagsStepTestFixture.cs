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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.PreCompilationSteps
{
	using AspView.Compiler.PreCompilationSteps;
	using NUnit.Framework;

	[TestFixture]
	public class SubViewTagsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new SubViewTagsStep();
		}

		[Test]
		public void Process_WhenThereAreNoSubViewTags_DoesNothing()
		{
			string source = @"
dkllgk
fgkdlfk
dfg
fdslk";
			expected = source;
			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SimpleSubViewTag_Transforms()
		{
			string source = @"
before
<subView:Simple></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple""); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_TwoSubViewTags_Transforms()
		{
			string source = @"
before
<subView:Simple></subView:Simple>
<subView:Simple2></subView:Simple2>
after
";
			expected = @"
before
<% OutputSubView(""Simple""); %>
<% OutputSubView(""Simple2""); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithSimpleStringAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple name=""Ken""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""name"", ""Ken"")); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithSimpleStringAndSlashAttribute_TransformsEndEscapesTheSlash()
		{
			string source = @"
before
<subView:Simple name=""Ken\Egozi""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""name"", ""Ken\\Egozi"")); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithConstantAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= 29 %>""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""age"", 29)); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithStringAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple name=""<%= ""Ken"" %>""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""name"", ""Ken"")); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithVariableAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= myAge %>""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""age"", myAge)); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithVariableAndDotAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= me.Age %>""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""age"", me.Age)); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_SubViewsWithComplexVariableAttribute_Transforms()
		{
			string source = @"
before
<subView:Simple age=""<%= people[index].GetAge(In.Years) %>""></subView:Simple>
after
";
			expected = @"
before
<% OutputSubView(""Simple"", N(""age"", people[index].GetAge(In.Years))); %>
after
";

			file.RenderBody = source;
			step.Process(file);

			AssertStepOutput();
		}
	}
}
