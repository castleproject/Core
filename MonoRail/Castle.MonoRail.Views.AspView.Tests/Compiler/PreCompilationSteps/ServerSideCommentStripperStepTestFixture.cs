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
	public class ServerSideCommentStripperStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		#region string constant
		const string COMMENT_FORMAT = @"<%--{0}--%>";
		const string DEMO_COMMENT1 = @"<script runat=""server"">won't be parsed</script><component:isnotaviewcomponent></component:isnotaviewcomponent>";
		const string DEMO_COMMENT2 = @"
<script runat=""server"">
won't be parsed
</script>
<component:isnotaviewcomponent>

</component:isnotaviewcomponent>


";

		#endregion

		protected override void CreateStep()
		{
			step = new ServerSideCommentStripperStep();
		}
		
		[Test]
		public void Process_SingleLineCommentsAreStrippedFromBody()
		{

			#region view contents

			string source = @"<%@ Page Language=""C#"" %>
{0}
view content
{0}
view content
{0}
";
			file.ViewSource = string.Format(source, 
				string.Format(COMMENT_FORMAT, DEMO_COMMENT1));

			#endregion

			expected = string.Format(source, "");

			file.RenderBody = file.ViewSource;

			step.Process(file);

			AssertStepOutput();
		}

		[Test]
		public void Process_MultiLineCommentsAreStrippedFromBody() {

			#region view contents

			string source = @"<%@ Page Language=""C#"" %>
{0}
view content
{0}
view content
{0}
";
			file.ViewSource = string.Format(source,
				string.Format(COMMENT_FORMAT, DEMO_COMMENT2));

			#endregion

			expected = string.Format(source, "");

			file.RenderBody = file.ViewSource;

			step.Process(file);

			AssertStepOutput();
		}

	}
}
