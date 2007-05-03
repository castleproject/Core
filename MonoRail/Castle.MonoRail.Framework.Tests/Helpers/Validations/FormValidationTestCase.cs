// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Helpers.Validations
{
	using System.Globalization;
	using System.Threading;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;

	[TestFixture]
	public class FormValidationTestCase : BaseControllerTest
	{
		private FormHelper helper;
		private ModelWithValidation model;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();
			model = new ModelWithValidation();

			HomeController controller = new HomeController();
			PrepareController(controller, "", "Home", "Index");

			controller.PropertyBag.Add("model", model);

			helper.SetController(controller);
		}

		[Test]
		public void ValidationIsGeneratedForModel()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyfield\" " + 
				"name=\"model.nonemptyfield\" value=\"\" class=\"validator-id-prefix-model_ required\" " + 
				"title=\"This is a required field\" />", helper.TextField("model.nonemptyfield"));

			Assert.AreEqual("<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"validate-email\" " +
				"title=\"Email doesnt look right\" />", helper.TextField("model.emailfield"));

			// Attribute order cannot be guaranted, so this test may fail ocasionally
			// Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyemailfield\" " +
			//	"name=\"model.nonemptyemailfield\" value=\"\" class=\"validate-email validator-id-prefix-model_ required\" " +
			//	"title=\"Please enter a valid email address. For example fred@domain.com, This is a required field\" />", helper.TextField("model.nonemptyemailfield"));

			helper.EndFormTag();
		}

	}

	public class ModelWithValidation
	{
		private string nonEmptyField;
		private string emailField;
		private string nonEmptyEmailField;

		[ValidateNonEmpty]
		public string NonEmptyField
		{
			get { return nonEmptyField; }
			set { nonEmptyField = value; }
		}

		[ValidateEmail("Email doesnt look right")]
		public string EmailField
		{
			get { return emailField; }
			set { emailField = value; }
		}

		[ValidateNonEmpty, ValidateEmail]
		public string NonEmptyEmailField
		{
			get { return nonEmptyEmailField; }
			set { nonEmptyEmailField = value; }
		}
	}
}
