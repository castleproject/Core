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
	public class ImportStatementsStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep()
		{
			step = new ImportStatementsStep();
		}
		private static void AssertImportDirectivesHasBeenRemoved(string viewSource)
		{
			if (Internal.RegularExpressions.ImportDirective.IsMatch(viewSource))
				Assert.Fail("Imports directives were not removed from view source");
		}

		[Test]
		public void Process_WhenImportingCustomNamespace_SetsImport()
		{
			file.RenderBody = @"
<%@ Import Namespace = ""My.Name.Space"" %>
view content";
			step.Process(file);

			Assert.IsTrue(file.Imports.Contains("My.Name.Space"));

			AssertImportDirectivesHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenImportingSameNamespace_WouldStillSetImport()
		{
			file.RenderBody = @"
<%@ Import Namespace = ""My.Name.Space"" %>
<%@ Import Namespace = ""My.Name.Space"" %>
view content";
			step.Process(file);

			Assert.IsTrue(file.Imports.Contains("My.Name.Space"));

			AssertImportDirectivesHasBeenRemoved(file.RenderBody);
		}

		[Test]
		public void Process_WhenImportingDefaultNamespace_WouldStillSetImport()
		{
			file.RenderBody = @"
<%@ Import Namespace = ""System"" %>
<%@ Import Namespace = ""My.Name.Space"" %>
view content";
			step.Process(file);

			Assert.IsTrue(file.Imports.Contains("My.Name.Space"));
			Assert.IsTrue(file.Imports.Contains("System"));

			AssertImportDirectivesHasBeenRemoved(file.RenderBody);
		}
	}
}
