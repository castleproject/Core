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
	using System.Globalization;
	using System.Threading;
	using Controllers;
	using Framework.Helpers;
	using Framework.Helpers.ValidationStrategy;
	using NUnit.Framework;

	#endregion Using Directives

	[TestFixture]
	public class JQueryFormValidationTestCase
	{
		private FormHelper helper;
		private ModelWithValidation model;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture = en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();
			helper.UseWebValidatorProvider(new JQueryValidator());
			model = new ModelWithValidation();

			HomeController controller = new HomeController();
			ControllerContext controllerContext = new ControllerContext();

			controllerContext.PropertyBag.Add("model", model);

			helper.SetController(controller, controllerContext);
		}

		[Test]
		public void ValidationIsGeneratedForModel()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			helper.TextField("model.nonemptyfield");
			helper.TextField("model.emailfield");

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.emailfield"":{ email: ""Email doesnt look right"" }, ""model.nonemptyfield"":{ required: ""This is a required field"" }}, rules:{""model.emailfield"":{ email: true }, ""model.nonemptyfield"":{ required: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void UsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("nonemptyfield");
			helper.TextField("emailfield");

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.emailfield"":{ email: ""Email doesnt look right"" }, ""model.nonemptyfield"":{ required: ""This is a required field"" }}, rules:{""model.emailfield"":{ email: true }, ""model.nonemptyfield"":{ required: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void SameAsUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("EmailField");
			helper.TextField("ConfirmedEmailField");

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.ConfirmedEmailField"":{ equalTo: ""Fields do not match"" }, ""model.EmailField"":{ email: ""Email doesnt look right"" }}, rules:{""model.ConfirmedEmailField"":{ equalTo: ""#model_EmailField"" }, ""model.EmailField"":{ email: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void LessThenUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("FirstValue");
			helper.TextField("SecondValue");

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.FirstValue"":{ digits: ""Please enter a valid integer in this field"" }, ""model.SecondValue"":{ lesserThan: ""This field value must be lesser than the other field value."" }}, rules:{""model.FirstValue"":{ digits: true }, ""model.SecondValue"":{ lesserThan: ""#model_FirstValue"" }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void GreaterThenUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("ThirdValue");
			helper.TextField("ForthValue");

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.ForthValue"":{ greaterThan: ""This field value must be greater than the other field value."" }, ""model.ThirdValue"":{ digits: ""Please enter a valid integer in this field"" }}, rules:{""model.ForthValue"":{ greaterThan: ""#model_ThirdValue"" }, ""model.ThirdValue"":{ digits: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void GroupValidationUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("GroupValue1");
			helper.TextField("GroupValue2");
			helper.TextField("GroupValue3");
			helper.TextField("GroupValue4");

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {groups:{mygroup1: ""model.GroupValue1 model.GroupValue2 "", mygroup2: ""model.GroupValue3 model.GroupValue4 ""}, messages:{""model.GroupValue1"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue2"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue3"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.GroupValue4"":{ requiredmygroup2: ""At least one of the values should not be empty"" }}, rules:{""model.GroupValue1"":{ requiredmygroup1: true }, ""model.GroupValue2"":{ requiredmygroup1: true }, ""model.GroupValue3"":{ requiredmygroup2: true }, ""model.GroupValue4"":{ requiredmygroup2: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' );
jQuery.validator.addMethod('requiredmygroup1',  function() { if($(""#model_GroupValue1"").val()!='' || $(""#model_GroupValue2"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addMethod('requiredmygroup2',  function() { if($(""#model_GroupValue3"").val()!='' || $(""#model_GroupValue4"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addClassRules({requiredmygroup1: {mygroup1:true}});
jQuery.validator.addClassRules({requiredmygroup2: {mygroup2:true}}); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void RegExValidationUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("RegExEmailField");

			helper.Pop();
			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.RegExEmailField"":{ regExp: ""Field has an invalid content"" }}, rules:{""model.RegExEmailField"":{ regExp: [\w-]+@([\w-]+\.)+[\w-]+ }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}


		[Test]
		public void ValidationForSelects()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));


			helper.Select("model.city",
						  new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create("firstoption=---"));

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.city"":{ required: ""This is a required field"" }}, rules:{""model.city"":{ required: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void ValidationAreInherited()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			helper.Select("model.city.id",
						  new string[] { "1", "2" }, DictHelper.Create("firstoption=---"));

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {messages:{""model.city.id"":{ required: ""This is a required field"" }}, rules:{""model.city.id"":{ required: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void FullValidationSameAsUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("NonEmptyField");
			helper.TextField("EmailField");
			helper.TextField("NonEmptyEmailField");
			helper.TextField("FirstValue");
			helper.TextField("SecondValue");
			helper.TextField("ThirdValue");
			helper.TextField("ForthValue");
			helper.TextField("GroupValue1");
			helper.TextField("GroupValue2");
			helper.TextField("GroupValue3");
			helper.TextField("GroupValue4");
			helper.Select("City", new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create("firstoption=---"));
			helper.Select("Country.Id", new string[] { "1", "2" }, DictHelper.Create("firstoption=---"));

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {groups:{mygroup1: ""model.GroupValue1 model.GroupValue2 "", mygroup2: ""model.GroupValue3 model.GroupValue4 ""}, messages:{""model.City"":{ required: ""This is a required field"" }, ""model.Country.Id"":{ required: ""This is a required field"" }, ""model.EmailField"":{ email: ""Email doesnt look right"" }, ""model.FirstValue"":{ digits: ""Please enter a valid integer in this field"" }, ""model.ForthValue"":{ greaterThan: ""This field value must be greater than the other field value."" }, ""model.GroupValue1"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue2"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue3"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.GroupValue4"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.NonEmptyEmailField"":{  email: ""Please enter a valid email address. For example fred@domain.com"" , required: ""This is a required field"" }, ""model.NonEmptyField"":{ required: ""This is a required field"" }, ""model.SecondValue"":{ lesserThan: ""This field value must be lesser than the other field value."" }, ""model.ThirdValue"":{ digits: ""Please enter a valid integer in this field"" }}, rules:{""model.City"":{ required: true }, ""model.Country.Id"":{ required: true }, ""model.EmailField"":{ email: true }, ""model.FirstValue"":{ digits: true }, ""model.ForthValue"":{ greaterThan: ""#model_ThirdValue"" }, ""model.GroupValue1"":{ requiredmygroup1: true }, ""model.GroupValue2"":{ requiredmygroup1: true }, ""model.GroupValue3"":{ requiredmygroup2: true }, ""model.GroupValue4"":{ requiredmygroup2: true }, ""model.NonEmptyEmailField"":{  email: true , required: true }, ""model.NonEmptyField"":{ required: true }, ""model.SecondValue"":{ lesserThan: ""#model_FirstValue"" }, ""model.ThirdValue"":{ digits: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' );
jQuery.validator.addMethod('requiredmygroup1',  function() { if($(""#model_GroupValue1"").val()!='' || $(""#model_GroupValue2"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addMethod('requiredmygroup2',  function() { if($(""#model_GroupValue3"").val()!='' || $(""#model_GroupValue4"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addClassRules({requiredmygroup1: {mygroup1:true}});
jQuery.validator.addClassRules({requiredmygroup2: {mygroup2:true}}); });/*]]>*/</script>
</form>", helper.EndFormTag());
		}

		[Test]
		public void FullValidationUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			helper.TextField("nonemptyfield");
			helper.TextField("emailfield");
			helper.TextField("nonemptyemailfield");
			helper.TextField("firstvalue");
			helper.TextField("secondvalue");
			helper.TextField("thirdvalue");
			helper.TextField("forthvalue");
			helper.TextField("groupvalue1");
			helper.TextField("groupvalue2");
			helper.TextField("groupvalue3");
			helper.TextField("groupvalue4");
			helper.Select("city", new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create("firstoption=---"));
			helper.Select("country.id", new string[] { "1", "2" }, DictHelper.Create("firstoption=---"));

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {groups:{mygroup1: ""model.groupvalue1 model.groupvalue2 "", mygroup2: ""model.groupvalue3 model.groupvalue4 ""}, messages:{""model.city"":{ required: ""This is a required field"" }, ""model.country.id"":{ required: ""This is a required field"" }, ""model.emailfield"":{ email: ""Email doesnt look right"" }, ""model.firstvalue"":{ digits: ""Please enter a valid integer in this field"" }, ""model.forthvalue"":{ greaterThan: ""This field value must be greater than the other field value."" }, ""model.groupvalue1"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.groupvalue2"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.groupvalue3"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.groupvalue4"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.nonemptyemailfield"":{  email: ""Please enter a valid email address. For example fred@domain.com"" , required: ""This is a required field"" }, ""model.nonemptyfield"":{ required: ""This is a required field"" }, ""model.secondvalue"":{ lesserThan: ""This field value must be lesser than the other field value."" }, ""model.thirdvalue"":{ digits: ""Please enter a valid integer in this field"" }}, rules:{""model.city"":{ required: true }, ""model.country.id"":{ required: true }, ""model.emailfield"":{ email: true }, ""model.firstvalue"":{ digits: true }, ""model.forthvalue"":{ greaterThan: ""#model_ThirdValue"" }, ""model.groupvalue1"":{ requiredmygroup1: true }, ""model.groupvalue2"":{ requiredmygroup1: true }, ""model.groupvalue3"":{ requiredmygroup2: true }, ""model.groupvalue4"":{ requiredmygroup2: true }, ""model.nonemptyemailfield"":{  email: true , required: true }, ""model.nonemptyfield"":{ required: true }, ""model.secondvalue"":{ lesserThan: ""#model_FirstValue"" }, ""model.thirdvalue"":{ digits: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' );
jQuery.validator.addMethod('requiredmygroup1',  function() { if($(""#model_groupvalue1"").val()!='' || $(""#model_groupvalue2"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addMethod('requiredmygroup2',  function() { if($(""#model_groupvalue3"").val()!='' || $(""#model_groupvalue4"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addClassRules({requiredmygroup1: {mygroup1:true}});
jQuery.validator.addClassRules({requiredmygroup2: {mygroup2:true}}); });/*]]>*/</script>
</form>"
				, helper.EndFormTag());
		}

		[Test]
		public void FullValidationNotUsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			helper.TextField("model.nonemptyfield");
			helper.TextField("model.emailfield");
			helper.TextField("model.nonemptyemailfield");
			helper.TextField("model.firstvalue");
			helper.TextField("model.secondvalue");
			helper.TextField("model.thirdvalue");
			helper.TextField("model.forthvalue");
			helper.TextField("model.groupvalue1");
			helper.TextField("model.groupvalue2");
			helper.TextField("model.groupvalue3");
			helper.TextField("model.groupvalue4");
			helper.Select("model.city", new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create("firstoption=---"));
			helper.Select("model.country.id", new string[] { "1", "2" }, DictHelper.Create("firstoption=---"));

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {groups:{mygroup1: ""model.groupvalue1 model.groupvalue2 "", mygroup2: ""model.groupvalue3 model.groupvalue4 ""}, messages:{""model.city"":{ required: ""This is a required field"" }, ""model.country.id"":{ required: ""This is a required field"" }, ""model.emailfield"":{ email: ""Email doesnt look right"" }, ""model.firstvalue"":{ digits: ""Please enter a valid integer in this field"" }, ""model.forthvalue"":{ greaterThan: ""This field value must be greater than the other field value."" }, ""model.groupvalue1"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.groupvalue2"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.groupvalue3"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.groupvalue4"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.nonemptyemailfield"":{  email: ""Please enter a valid email address. For example fred@domain.com"" , required: ""This is a required field"" }, ""model.nonemptyfield"":{ required: ""This is a required field"" }, ""model.secondvalue"":{ lesserThan: ""This field value must be lesser than the other field value."" }, ""model.thirdvalue"":{ digits: ""Please enter a valid integer in this field"" }}, rules:{""model.city"":{ required: true }, ""model.country.id"":{ required: true }, ""model.emailfield"":{ email: true }, ""model.firstvalue"":{ digits: true }, ""model.forthvalue"":{ greaterThan: ""#model_ThirdValue"" }, ""model.groupvalue1"":{ requiredmygroup1: true }, ""model.groupvalue2"":{ requiredmygroup1: true }, ""model.groupvalue3"":{ requiredmygroup2: true }, ""model.groupvalue4"":{ requiredmygroup2: true }, ""model.nonemptyemailfield"":{  email: true , required: true }, ""model.nonemptyfield"":{ required: true }, ""model.secondvalue"":{ lesserThan: ""#model_FirstValue"" }, ""model.thirdvalue"":{ digits: true }}} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' );
jQuery.validator.addMethod('requiredmygroup1',  function() { if($(""#model_groupvalue1"").val()!='' || $(""#model_groupvalue2"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addMethod('requiredmygroup2',  function() { if($(""#model_groupvalue3"").val()!='' || $(""#model_groupvalue4"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addClassRules({requiredmygroup1: {mygroup1:true}});
jQuery.validator.addClassRules({requiredmygroup2: {mygroup2:true}}); });/*]]>*/</script>
</form>"
				, helper.EndFormTag());
		}

		[Test]
		public void TestIsAjaxOption()
		{
			helper.FormTag(DictHelper.CreateN("isAjax", true).N("noaction", true));

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {submitHandler:function( form ) { jQuery( form ).ajaxSubmit(); }} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' ); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}

		[Test]
		public void TestIsAjaxOptionAndValidation()
		{
			helper.FormTag(DictHelper.CreateN("isAjax", true).N("noaction", true));
			helper.Push("model");

			helper.TextField("NonEmptyField");
			helper.TextField("EmailField");
			helper.TextField("NonEmptyEmailField");
			helper.TextField("FirstValue");
			helper.TextField("SecondValue");
			helper.TextField("ThirdValue");
			helper.TextField("ForthValue");
			helper.TextField("GroupValue1");
			helper.TextField("GroupValue2");
			helper.TextField("GroupValue3");
			helper.TextField("GroupValue4");
			helper.Select("City", new string[] {"Sao Paulo", "Sao Carlos"}, DictHelper.Create("firstoption=---"));
			helper.Select("Country.Id", new string[] { "1", "2" }, DictHelper.Create("firstoption=---"));

			helper.Pop();

			Assert.AreEqual(
				@"
<script type=""text/javascript"">/*<![CDATA[*/
jQuery( document ).ready( function() { jQuery(""#form1"").validate( {groups:{mygroup1: ""model.GroupValue1 model.GroupValue2 "", mygroup2: ""model.GroupValue3 model.GroupValue4 ""}, messages:{""model.City"":{ required: ""This is a required field"" }, ""model.Country.Id"":{ required: ""This is a required field"" }, ""model.EmailField"":{ email: ""Email doesnt look right"" }, ""model.FirstValue"":{ digits: ""Please enter a valid integer in this field"" }, ""model.ForthValue"":{ greaterThan: ""This field value must be greater than the other field value."" }, ""model.GroupValue1"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue2"":{ requiredmygroup1: ""At least one of the values should not be empty"" }, ""model.GroupValue3"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.GroupValue4"":{ requiredmygroup2: ""At least one of the values should not be empty"" }, ""model.NonEmptyEmailField"":{  email: ""Please enter a valid email address. For example fred@domain.com"" , required: ""This is a required field"" }, ""model.NonEmptyField"":{ required: ""This is a required field"" }, ""model.SecondValue"":{ lesserThan: ""This field value must be lesser than the other field value."" }, ""model.ThirdValue"":{ digits: ""Please enter a valid integer in this field"" }}, rules:{""model.City"":{ required: true }, ""model.Country.Id"":{ required: true }, ""model.EmailField"":{ email: true }, ""model.FirstValue"":{ digits: true }, ""model.ForthValue"":{ greaterThan: ""#model_ThirdValue"" }, ""model.GroupValue1"":{ requiredmygroup1: true }, ""model.GroupValue2"":{ requiredmygroup1: true }, ""model.GroupValue3"":{ requiredmygroup2: true }, ""model.GroupValue4"":{ requiredmygroup2: true }, ""model.NonEmptyEmailField"":{  email: true , required: true }, ""model.NonEmptyField"":{ required: true }, ""model.SecondValue"":{ lesserThan: ""#model_FirstValue"" }, ""model.ThirdValue"":{ digits: true }}, submitHandler:function( form ) { jQuery( form ).ajaxSubmit(); }} );
jQuery.validator.addMethod('greaterThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }, 'Must be greater than {0}.' );
jQuery.validator.addMethod('lesserThan', function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }, 'Must be lesser than {0}.' );
jQuery.validator.addMethod('notEqualTo', function(value, element, param) { return value != jQuery(param).val(); }, 'Must not be equal to {0}.' );
jQuery.validator.addMethod('regExp', function(value, element, param) { return new RegExp(param).text(value); }, 'Must match expression.' );
jQuery.validator.addMethod('requiredmygroup1',  function() { if($(""#model_GroupValue1"").val()!='' || $(""#model_GroupValue2"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addMethod('requiredmygroup2',  function() { if($(""#model_GroupValue3"").val()!='' || $(""#model_GroupValue4"").val()!='') { return true } else { return false; } }, 'At least one of the values should not be empty' );
jQuery.validator.addClassRules({requiredmygroup1: {mygroup1:true}});
jQuery.validator.addClassRules({requiredmygroup2: {mygroup2:true}}); });/*]]>*/</script>
</form>",
				helper.EndFormTag());
		}
	}
}
