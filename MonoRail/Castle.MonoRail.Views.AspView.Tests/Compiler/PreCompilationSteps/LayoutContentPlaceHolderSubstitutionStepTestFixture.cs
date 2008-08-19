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
	using System.Collections;
	using System.Text.RegularExpressions;
	using Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps;
	using Castle.MonoRail.Views.AspView.Compiler;
	using NUnit.Framework;

	public class LayoutContentPlaceHolderSubstitutionStepTestFixture : AbstractPreCompilationStepTestFixture
	{

		protected override void CreateStep() 
		{
			step = new LayoutContentPlaceHolderSubstitutionStep();
		}

		private const string validplaceholderformat = @"<asp:contentplaceholder runat=""server"" id=""{0}""/>";
		private static readonly string[] invalididattributevalues = new string[]{"123_AZERTY", "aze.qsd"};

		private static void AssertContentPlaceHolderHasBeenRemoved(string viewSource)
		{
			Match match = Internal.RegularExpressions.LayoutContentPlaceHolder.Match(viewSource);
			if(match.Success)
			{
				string parsedAttributes = match.Groups["attributes"].Value;
				IDictionary attributes = Utilities.GetAttributesDictionaryFrom(parsedAttributes);
				if(attributes.Contains("runat") && String.Equals("server",(attributes["runat"] as string), StringComparison.InvariantCultureIgnoreCase))
					Assert.Fail("asp:contentplaceholder tag that have runatat server attribute should have been removed from view source");
			}
		}

		[Test, ExpectedException(typeof(AspViewException), ExpectedMessage = LayoutContentPlaceHolderSubstitutionStep.ExceptionMessages.IdAttributeNotFound)]
		public void Throws_When_RunatAtServerAttributeFound_But_NoIdAttributeFound()
		{
			file.RenderBody = @"<asp:ContentplaceHoLDer rUNaT=""serVer""/>";
			step.Process(file);
		}

		[Test, ExpectedException(typeof(AspViewException), ExpectedMessage = LayoutContentPlaceHolderSubstitutionStep.ExceptionMessages.IdAttributeEmpty)]
		public void Throws_When_RunatServerAttributeFound_But_IdAttributeEmpty()
		{
			file.RenderBody = @"<asp:contentplaceholder runat=""server"" id=""""></asp:contentplaceholder>";
			step.Process(file);
		}

		[Test]
		public void Throws_When_IdAttributeValueIsAlreadyDeclaredAsViewProperty()
		{
			string propertyname = "myproperty";
			string expectedexceptionmessage = String.Format(
				LayoutContentPlaceHolderSubstitutionStep.ExceptionMessages.ViewPropertyAllreadyRegisteredWithOtherTypeFormat,
				propertyname);

			file.RenderBody = String.Format(validplaceholderformat, propertyname);
			file.Properties.Add(propertyname, new ViewProperty(propertyname, "object", null));
			try
			{
					step.Process(file);
				Assert.Fail("should have thrown");
			}
			catch(AspViewException e)
			{
				Assert.AreEqual(expected, e.Message);
			}
		}

		[Test]
		public void ContentPlaceHolder_IsSubstitutedWithPlainViewProperty_AndPropertyIsRegistered()
		{
			string placeholderid = "regionid";
			file.RenderBody = String.Format(validplaceholderformat, placeholderid);

			step.Process(file);

			Assert.IsTrue(file.Properties.ContainsKey(placeholderid));
			Assert.IsTrue(String.Equals("string", file.Properties[placeholderid].Type, StringComparison.InvariantCultureIgnoreCase));

		}

		[Test]
		public void ContentPlaceHolder_IsSubstitutedWithViewProperty_AndPropertyIsNotRegistered_WhenIdIs_ViewContents()
		{
			string placeholderid = "ViewContents";
			file.RenderBody = String.Format(validplaceholderformat, placeholderid);

			step.Process(file);
			Assert.IsFalse(file.Properties.ContainsKey(placeholderid));

		}

		[Test]
		public void ContentPlaceHolder_IsSubstitutedWithViewPropertyOutput()
		{
			string viewbodyformat = "viewcontent {0} viewcontent";
			string placeholderid = "regionid";
			string afterprocessingexpectedbody = string.Format(viewbodyformat, String.Format("<%={0}%>", placeholderid));

			file.RenderBody = string.Format(viewbodyformat, string.Format(validplaceholderformat, placeholderid));

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedbody, file.RenderBody);

			AssertContentPlaceHolderHasBeenRemoved(file.RenderBody);
		}


		[Test]
		public void ContentPlaceHolder_IsNotSubstitutedWithoutRunatAttribute() 
		{
			string viewbodyformat = "viewcontent {0} viewcontent";
			string fakeplaceholder = @"<asp:contentplaceholder id=""stillhere""/>";
			string afterprocessingexpectedbody = String.Format(viewbodyformat, fakeplaceholder);
			file.RenderBody = afterprocessingexpectedbody;

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedbody, file.RenderBody);
		}



		[Test]
		public void ContentPlaceHolder_IsNotSubstitutedWithRunat_ButNotServer() {
			string viewbodyformat = "viewcontent {0} viewcontent";
			string fakeplaceholder = @"<asp:contentplaceholder runat=""notserver"" id=""stillhere""/>";
			string afterprocessingexpectedbody = String.Format(viewbodyformat, fakeplaceholder);
			file.RenderBody = afterprocessingexpectedbody;

			step.Process(file);

			Assert.AreEqual(afterprocessingexpectedbody, file.RenderBody);
		}
	}
}
