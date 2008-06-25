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

namespace Castle.MonoRail.Framework.Tests.Helpers.Validations
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using System.Threading;

	using NUnit.Framework;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Helpers.ValidationStrategy;
	using Castle.MonoRail.Framework.Tests.Controllers;

	#endregion Using Directives

	[TestFixture]
	public class JQueryFormValidationTestCase
	{
		private FormHelper helper;
		private ModelWithValidation model;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture( "en" );

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();
			helper.UseWebValidatorProvider( new JQueryValidator() );
			model = new ModelWithValidation();

			HomeController controller = new HomeController();
			ControllerContext controllerContext = new ControllerContext();

			controllerContext.PropertyBag.Add( "model", model );

			helper.SetController( controller, controllerContext );
		}

		[Test]
		public void ValidationIsGeneratedForModel()
		{
			helper.FormTag( DictHelper.Create( "noaction=true" ) );

			Assert.AreEqual( "<input type=\"text\" id=\"model_nonemptyfield\" " +
				"name=\"model.nonemptyfield\" value=\"\" class=\"required\" " +
				"title=\"This is a required field.\" />", helper.TextField( "model.nonemptyfield" ) );

			Assert.AreEqual( "<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"email\" " +
				"title=\"Email doesnt look right.\" />", helper.TextField( "model.emailfield" ) );

			// Attribute order cannot be guaranted, so this test may fail ocasionally
			// Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyemailfield\" " +
			//	"name=\"model.nonemptyemailfield\" value=\"\" class=\"validate-email required\" " +
			//	"title=\"Please enter a valid email address. For example fred@domain.com, This is a required field\" />", helper.TextField("model.nonemptyemailfield"));

			helper.EndFormTag();
		}

		[Test]
		public void UsingScopes()
		{
			helper.FormTag( DictHelper.Create( "noaction=true" ) );
			helper.Push( "model" );

			Assert.AreEqual( "<input type=\"text\" id=\"model_nonemptyfield\" " +
				"name=\"model.nonemptyfield\" value=\"\" class=\"required\" " +
				"title=\"This is a required field.\" />", helper.TextField( "nonemptyfield" ) );

			Assert.AreEqual( "<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"email\" " +
				"title=\"Email doesnt look right.\" />", helper.TextField( "emailfield" ) );

			// Attribute order cannot be guaranted, so this test may fail ocasionally
			// Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyemailfield\" " +
			//	"name=\"model.nonemptyemailfield\" value=\"\" class=\"validate-email required\" " +
			//	"title=\"Please enter a valid email address. For example fred@domain.com, This is a required field\" />", helper.TextField("nonemptyemailfield"));

			helper.Pop();
			helper.EndFormTag();
		}

		[Test]
		public void SameAsUsingScopes()
		{
			helper.FormTag( DictHelper.Create( "noaction=true" ) );
			helper.Push( "model" );

			Assert.AreEqual( "<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"email\" " +
				"title=\"Email doesnt look right.\" />", helper.TextField( "emailfield" ) );

			Assert.AreEqual("<input type=\"text\" id=\"model_ConfirmedEmailField\" " +
				"name=\"model.ConfirmedEmailField\" value=\"\" class=\"equalTo\" " +
				"equalto=\"model_EmailField\" " +
				"title=\"Fields do not match.\" />", helper.TextField("ConfirmedEmailField"));
            
			helper.Pop();
			helper.EndFormTag();
		}

		[Test]
		public void ValidationForSelects()
		{
			helper.FormTag( DictHelper.Create( "noaction=true" ) );

			Assert.AreEqual( "<select id=\"model_city\" " +
				"name=\"model.city\" class=\"required\" " +
				"title=\"This is a required field.\" >\r\n" +
				"<option value=\"0\">---</option>\r\n" +
				"<option value=\"Sao Paulo\">Sao Paulo</option>\r\n" +
				"<option value=\"Sao Carlos\">Sao Carlos</option>\r\n" +
				"</select>",
				helper.Select( "model.city",
					new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create( "firstoption=---" ) ) );

			helper.EndFormTag();
		}

		[Test]
		public void ValidationAreInherited()
		{
			helper.FormTag( DictHelper.Create( "noaction=true" ) );

			Assert.AreEqual( "<select id=\"model_city_id\" " +
				"name=\"model.city.id\" class=\"required\" " +
				"title=\"This is a required field.\" >\r\n" +
				"<option value=\"0\">---</option>\r\n" +
				"<option value=\"1\">1</option>\r\n" +
				"<option value=\"2\">2</option>\r\n" +
				"</select>",
				helper.Select( "model.city.id",
					new string[] { "1", "2" }, DictHelper.Create( "firstoption=---" ) ) );

			helper.EndFormTag();
		}

		[Test]
		public void TestIsAjaxOption()
		{
			helper.FormTag( DictHelper.CreateN( "isAjax", true ).N( "noaction", true ) );

			Assert.AreEqual( "\r\n<script type=\"text/javascript\">\r\njQuery(\"#form1\").validate( {submitHandler:function( form ) { jQuery( form ).ajaxSubmit(); }} );\r\njQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );\r\njQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );\r\njQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );</script>\r\n</form>", helper.EndFormTag() );
		}
	}
}
