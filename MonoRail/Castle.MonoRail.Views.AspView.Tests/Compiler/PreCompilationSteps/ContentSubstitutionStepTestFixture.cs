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

namespace Castle.MonoRail.Views.AspView.Tests.Compiler.PreCompilationSteps {
	using System;
	using Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps;
	using NUnit.Framework;

	public class ContentSubstitutionStepTestFixture : AbstractPreCompilationStepTestFixture
	{
		protected override void CreateStep() 
		{
			step = new ContentSubstitutionStep();
		}

		[Test, ExpectedException(typeof(AspViewException), ExpectedMessage = ContentSubstitutionStep.ExceptionMessages.ContentPlaceHolderIdAttributeNotFound)]
		public void ThrowsWhen_ContentTagRunatServerAttributeFound_ButNoContentIdAttributeFound()
		{
			file.RenderBody = @"<asp:Content runat=""server""></asp:content>";
			step.Process(file);
		}

		[Test, ExpectedException(typeof(AspViewException), ExpectedMessage = ContentSubstitutionStep.ExceptionMessages.ContentPlaceHolderIdAttributeEmpty)]
		public void ThrowsWhen_ContentTagRunatServerAttributeFound_ButContentIdAttributeValueIsEmpty()
		{
			file.RenderBody = @"<asp:Content runat=""server"" contentplaceholderid=""""></asp:content>";
			step.Process(file);
		}

		[Test]
		public void ContentTagIsSubstitutedWithCaptureForComponent()
		{
			string contentid = "regionname";
			string viewbodyformat = "viewcontent {0} viewcontent";
			string capturedcontent = @"
<h2>some captured content</h2>
<component:name withattribute=""<%=value%>""></component:name>
";
			string contenttagformat = @"<asp:content runat=""server"" contentplaceholderid=""{0}"">{1}</asp:content>";
			string capturefortagformat = @"<component:capturefor id=""{0}"">{1}</component:capturefor>";
			string afterprocessingexpectedviewbody = string.Format(
				viewbodyformat,
				String.Format(capturefortagformat, contentid, capturedcontent)
				);

			file.RenderBody = String.Format(viewbodyformat, String.Format(contenttagformat, contentid, capturedcontent));

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedviewbody, file.RenderBody);
		}

		[Test]
		public void ContentTagIsSubstitutedWithPlainContent_When_ContentPlaceHolderId_Is_ViewContents()
		{
			string contentid = "ViewContents";
			string viewbodyformat = "viewcontent {0} viewcontent";
			string capturedcontent = @"
<h2>some captured content</h2>
<component:name withattribute=""<%=value%>""></component:name>
";
			string contenttagformat = @"<asp:content runat=""server"" contentplaceholderid=""{0}"">{1}</asp:content>";
			string afterprocessingexpectedviewbody = string.Format(
				viewbodyformat,
				capturedcontent
				);

			file.RenderBody = String.Format(viewbodyformat, String.Format(contenttagformat, contentid, capturedcontent));

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedviewbody, file.RenderBody);
		}

		[Test]
		public void ContentTagIsNotSubstituted_When_RunatAttributeNotFound() {
			string contentid = "regionname";
			string viewbodyformat = "viewcontent {0} viewcontent";
			string capturedcontent = @"
<h2>some captured content</h2>
<component:name withattribute=""<%=value%>""></component:name>
";
			string contenttagformat = @"<asp:content contentplaceholderid=""{0}"">{1}</asp:content>";
			string afterprocessingexpectedviewbody = string.Format(
				viewbodyformat,
				String.Format(contenttagformat, contentid, capturedcontent)
				);

			file.RenderBody = String.Format(viewbodyformat, String.Format(contenttagformat, contentid, capturedcontent));

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedviewbody, file.RenderBody);

		}

		[Test]
		public void ContentTagIsNotSubstituted_When_RunatAttributeFound_ButNotServer() {
			string contentid = "regionname";
			string viewbodyformat = "viewcontent {0} viewcontent";
			string capturedcontent = @"
<h2>some captured content</h2>
<component:name withattribute=""<%=value%>""></component:name>
";
			string contenttagformat = @"<asp:content runat=""notserver"" contentplaceholderid=""{0}"">{1}</asp:content>";
			string afterprocessingexpectedviewbody = string.Format(
				viewbodyformat,
				String.Format(contenttagformat, contentid, capturedcontent)
				);

			file.RenderBody = String.Format(viewbodyformat, String.Format(contenttagformat, contentid, capturedcontent));

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedviewbody, file.RenderBody);

		}
	}
}
