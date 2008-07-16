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
	using NUnit.Framework;
	using AspView.Compiler.PreCompilationSteps;

	[TestFixture]
	public class EmbededServerScriptStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		#region string constant
		const string SCRIPT_FORMAT = @"<script runat=""server"">{0}</script>";

		const string DEMO_SCRIPT = @"
int evilViewState = 0000000;

public void SayHello(string personToGreet)
{
	++evilViewState;
	return String.Format(@""hello {0} ({1} times)"", personToGreet, evilViewState);
}
";
		#endregion

		protected override void CreateStep()
		{
			step = new EmbededServerScriptStep();
		}

		[Test]
		public void Process_WhenThereIsOneScriptTag_StripsFromBodyAndAddToList()
		{

			#region view contents

			string source = @"<%@ Page Language=""C#"" %>
view content
{0}
view content
";
			file.ViewSource = string.Format(source, 
				string.Format(SCRIPT_FORMAT, DEMO_SCRIPT));

			#endregion

			expected = string.Format(source, "");

			file.RenderBody = file.ViewSource;

			step.Process(file);

			// there is one embeded script
			Assert.AreEqual(file.EmbededScriptBlocks.Count, 1);

			// script content has expected value
			Assert.AreEqual(DEMO_SCRIPT, file.EmbededScriptBlocks[0]);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenThereIsMoreThanOneScriptTag_StripsFromBodyAndAddToList()
		{
			#region view contents
			string source = @"<%@ Page Language=""C#"" %>
view content
{0}
view content
{1}
view content
";
			string DEMO_SCRIPT2 = DEMO_SCRIPT + @"
int anotherVar;
";
			file.ViewSource = string.Format(source,
				string.Format(SCRIPT_FORMAT, DEMO_SCRIPT),
				string.Format(SCRIPT_FORMAT, DEMO_SCRIPT2));


			#endregion

			expected = string.Format(source, "", "");

			file.RenderBody = file.ViewSource;

			step.Process(file);

			// there is one embeded script
			Assert.AreEqual(file.EmbededScriptBlocks.Count, 2);

			// script content has expected values
			Assert.AreEqual(DEMO_SCRIPT, file.EmbededScriptBlocks[0]);
			Assert.AreEqual(DEMO_SCRIPT2, file.EmbededScriptBlocks[1]);

			AssertStepOutput();
		}

		[Test]
		public void Process_WhenScriptTagsAreStrange_StripsFromBodyAndAddToList()
		{

			#region view contents

			string source = @"<%@ Page Language=""C#"" %>
{0}
{1}
{2}
{3}
{4}
{5}
";
			file.ViewSource = string.Format(source,
				@"<script runat=""server"">script0</script>",
				@"<script runat= ""server"">script1</script>",
				@"<script runat= ""server"" >script2</script>",
				@"<script runat = ""server"">script3</script>",
				@"<script RunAt= ""server"" >script4</script>",
				@"<script   Runat= ""server"">script5</script>");

			#endregion

			expected = string.Format(source, "", "", "", "", "", "");

			file.RenderBody = file.ViewSource;

			step.Process(file);

			Assert.AreEqual(file.EmbededScriptBlocks.Count, 6);

			// script content has expected value
			for (int i = 0; i <= 5; ++i)
				Assert.AreEqual("script" + i, file.EmbededScriptBlocks[i]);

			AssertStepOutput();
		}
	}
}
