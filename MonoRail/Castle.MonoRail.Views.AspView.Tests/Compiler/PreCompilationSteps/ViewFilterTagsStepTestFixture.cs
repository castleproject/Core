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
	public class ViewFilterTagsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new ViewFilterTagsStep();
		}
		
		[Test]
		public void Process_WhenThereAreNoViewFilterTags_DoesNothing()
		{
			string source = @"
dkllgk
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.AreEqual(source, file.RenderBody);
		}

		[Test]
		public void Process_WhenThereIsAViewFilterTag_Transforms()
		{
			string source = @"
dkllgk
<filter:Simple>some stuff</filter:Simple>
fgkdlfk
dfg
fdslk";
			expected = @"
dkllgk
<% StartFiltering(""SimpleViewFilter""); %>some stuff<% EndFiltering(); %>
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.AreEqual(expected, file.RenderBody);
		}

		[Test]
		public void Process_WhenThereIsAnEarlyBoundViewFilterTag_Transforms()
		{
			string source = @"
dkllgk
<filter:LowerCase>some stuff</filter:LowerCase>
fgkdlfk
dfg
fdslk";
			expected = @"
dkllgk
<% StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.LowerCaseViewFilter()); %>some stuff<% EndFiltering(); %>
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.AreEqual(expected, file.RenderBody);
		}

		[Test]
		public void Process_WhenNested_Transforms()
		{
			string source = @"
dkllgk
<filter:LowerCase>
some stuff
<filter:Simple>
yoodle doodle
</filter:Simple>
blah blah
</filter:LowerCase>
fgkdlfk
dfg
fdslk";
			expected = @"
dkllgk
<% StartFiltering(new Castle.MonoRail.Views.AspView.ViewFilters.LowerCaseViewFilter()); %>
some stuff
<% StartFiltering(""SimpleViewFilter""); %>
yoodle doodle
<% EndFiltering(); %>
blah blah
<% EndFiltering(); %>
fgkdlfk
dfg
fdslk";

			file.RenderBody = source;
			step.Process(file);

			Assert.AreEqual(expected, file.RenderBody);
		}
	}
}
