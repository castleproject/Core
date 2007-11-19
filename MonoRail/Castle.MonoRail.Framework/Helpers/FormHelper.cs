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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Services;
	using HtmlTextWriter = System.Web.UI.HtmlTextWriter;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers.ValidationStrategy;
	using Castle.MonoRail.Framework.Internal;
	using Castle.Components.Binder;
	using Castle.Components.Validator;

	/// <summary>
	/// Represents all scopes that the <see cref="FormHelper"/>
	/// uses to search for root values
	/// </summary>
	public enum RequestContext
	{
		/// <summary>
		/// All scopes should be searched
		/// </summary>
		All,
		/// <summary>
		/// Only PropertyBag should be searched
		/// </summary>
		PropertyBag,
		/// <summary>
		/// Only Flash should be searched
		/// </summary>
		Flash,
		/// <summary>
		/// Only Session should be searched
		/// </summary>
		Session,
		/// <summary>
		/// Only Request should be searched
		/// </summary>
		Request,
		/// <summary>
		/// Only Params should be searched
		/// </summary>
		Params
	}

	/// <summary>
	/// The FormHelper allows you to output html input elements using the 
	/// conventions necessary to use the DataBinder on the server side. Ultimately it
	/// allows you do to a bi-directional binding -- if used properly.
	/// </summary>
	/// 
	/// <seealso cref="DataBindAttribute"/>
	/// <seealso cref="Castle.Components.Common"/>
	/// 
	/// <example>
	/// Using simple values:
	/// <para>On the controller:</para>
	/// 
	/// <code>
	/// public void MyAction()
	/// {
	///		PropertyBag["name"] = "John Doe";
	/// }
	/// </code>
	/// 
	/// <para>On the view (using NVelocity syntax)</para>
	/// 
	/// <code lang="none">
	/// $Form.TextField('name') // Renders an input text with value "John Doe"
	/// </code>
	/// 
	/// <para>
	/// Using complex objects:
	/// </para>
	/// 
	/// <code>
	/// public void MyAction()
	/// {
	///		PropertyBag["user"] = new User("John Doe");
	/// }
	/// </code>
	/// 
	/// <para>On the view (using NVelocity syntax)</para>
	/// 
	/// <code lang="none">
	/// $Form.TextField('user.name') // Renders an input text with value "John Doe"
	/// </code>
	/// </example>
	/// 
	/// <remarks>
	/// <b>Elements generation</b> <br/>
	/// <para>
	/// <list type="table">
	/// <item>
	///		<term>Buttons</term>
	///		<description>
	///		<see cref="Submit(string)"/> <br/>
	///		<see cref="Button(string)" /> <br/>
	///		<see cref="ButtonElement(string)" />
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Select</term>
	///		<description>
	///		<see cref="Select(string,IEnumerable)" />
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Text area</term>
	///		<description>
	///		<see cref="TextArea(string)" />
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Hidden field</term>
	///		<description>
	///		<see cref="HiddenField(string)" /> 
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Checkbox field</term>
	///		<description>
	///		<see cref="CheckboxField(string)" />  <br/>
	///		<see cref="CreateCheckboxList(string,IEnumerable)" /> 
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Radio field</term>
	///		<description>
	///		<see cref="RadioField(string,object)" /> 
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>File upload</term>
	///		<description>
	///		<see cref="FileField(string)" /> 
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Text field</term>
	///		<description>
	///		<see cref="TextField(string)" /> <br/>
	///		<see cref="TextFieldValue(string, object)"/> <br/>
	///		<see cref="NumberField(string)" /> <br/>
	///		<see cref="NumberFieldValue(string, object)"/>
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Password field</term>
	///		<description>
	///		<see cref="PasswordField(string)" /> <br/>
	///		<see cref="PasswordNumberField(string)" />
	///		</description>
	/// </item>
	/// 
	/// <item>
	///		<term>Labels</term>
	///		<description>
	///		<see cref="LabelFor(string,string)" /> <br/>
	///		<see cref="LabelFor(string,string,IDictionary)"/>
	///		</description>
	/// </item>
	/// 
	/// </list>
	/// </para>
	/// 
	/// <para>
	/// <b>FormValidation</b> <br/>
	/// The following operations are related to the Form Validation support:
	/// </para>
	/// 
	/// <list type="table">
	/// <item>
	///		<term><see cref="FormTag(IDictionary)"/> and <see cref="EndFormTag"/> </term>
	///		<description>Opens/close the form tag. They are required to use the automatic form validation</description>
	/// </item>
	/// <item>
	///		<term><see cref="DisableValidation"/> </term>
	///		<description>Disables validation altogether</description>
	/// </item>
	/// <item>
	///		<term><see cref="UseWebValidatorProvider"/> </term>
	///		<description>Sets a custom Browser validator provider</description>
	/// </item>
	/// <item>
	///		<term><see cref="UsePrototypeValidation"/> </term>
	///		<description>Configures the helper to use the prototype easy field validation. Must be invoked before FormTag</description>
	/// </item>
	/// <item>
	///		<term><see cref="UsefValidate"/> </term>
	///		<description>Configures the helper to use the fValidate. Deprecated.</description>
	/// </item>
	/// <item>
	///		<term><see cref="UseZebdaValidation"/> </term>
	///		<description>Configures the helper to use the Zebda. Must be invoked before FormTag</description>
	/// </item>
	/// </list>
	/// 
	/// <para>
	/// <b>Mask support</b>. <br/>
	/// For most elements, you can use 
	/// the entries <c>mask</c> and optionally <c>mask_separator</c> to define a 
	/// mask for your inputs. Kudos to mordechai Sandhaus - 52action.com
	/// </para>
	/// 
	/// <para>
	/// For example: mask='2,5',mask_separator='/' will mask the content to '12/34/1234'
	/// </para>
	/// </remarks>
	public class FormHelper : AbstractHelper, IServiceEnabledComponent
	{
		/// <summary>
		/// Common property flags for reflection
		/// </summary>
		protected static readonly BindingFlags PropertyFlags = BindingFlags.GetProperty|BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;
		/// <summary>
		/// Common property flags for reflection (with declared only)
		/// </summary>
		protected static readonly BindingFlags PropertyFlags2 = BindingFlags.GetProperty|BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase|BindingFlags.DeclaredOnly;
		/// <summary>
		/// Common field flags for reflection
		/// </summary>
		protected static readonly BindingFlags FieldFlags = BindingFlags.GetField|BindingFlags.Public|BindingFlags.Instance|BindingFlags.IgnoreCase;

		private int formCount;
		private string currentFormId;
		private bool isValidationDisabled;
		private readonly Stack objectStack = new Stack();
		private IBrowserValidatorProvider validatorProvider = new PrototypeWebValidator();
		private BrowserValidationConfiguration validationConfig;

		/// <summary>
		/// Logger instance
		/// </summary>
		protected static ILogger logger = NullLogger.Instance;

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(FormHelper));
			}

			IBrowserValidatorProvider validatorProv = (IBrowserValidatorProvider)
				provider.GetService(typeof(IBrowserValidatorProvider));

			if (validatorProv != null)
			{
				validatorProvider = validatorProv;
			}
		}

		#endregion

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public string InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/FormHelperScript");
		}

		#region FormTag related

		/// <summary>
		/// Creates a form tag based on the parameters.
		/// <para>
		/// Javascript validation can also be bound to 
		/// the form and|or elements nested as long as the helper is 
		/// able to reach the <see cref="Type"/> of the object used on your view code
		/// </para>
		/// <para>
		/// The action attribute generation will use <see cref="UrlHelper"/>
		/// </para>
		/// </summary>
		/// 
		/// <seealso cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>
		/// 
		/// <example>
		/// 
		/// <code lang="none">
		/// $Form.FormTag("%{action='Save',id='productform'}")
		/// </code>
		/// 
		/// Outputs:
		/// 
		/// <code lang="xml">
		/// &lt;form method='post' action='/[appdir]/[controller]/Save.[extension]' id='productform'&gt;
		/// </code>
		///
		/// </example>
		/// 
		/// <remarks>
		/// The parameters are used to build a url and also to form the tag. For notes on the url 
		/// see <see cref="DefaultUrlBuilder.BuildUrl(UrlInfo,IDictionary)"/>. The other parameters supported 
		/// follows
		/// 
		/// <list type="table">
		/// <term>
		///		<term>noaction</term>
		///		<description>boolean. Disables the generation of an action</description>
		/// </term>
		/// <term>
		///		<term>method</term>
		///		<description>string. The http method to use. Defaults to <c>post</c></description>
		/// </term>
		/// <term>
		///		<term>id</term>
		///		<description>string. The form id.</description>
		/// </term>
		/// </list>
		/// 
		/// More parameters can be accepted depending on the form validation strategy you are using (if any).
		/// 
		/// </remarks>
		/// 
		/// <param name="parameters">The parameters for the tag or for action and form validation generation.</param>
		/// <returns></returns>
		public string FormTag(IDictionary parameters)
		{
			string url = null;

			// Creates action attribute
			if (CommonUtils.ObtainEntryAndRemove(parameters, "noaction", "false") == "false")
			{
				url = UrlHelper.For(parameters);
			}

			return FormTag(url, parameters);
		}

		/// <summary>
		/// Creates a form tag based on the parameters.
		/// <para>
		/// Javascript validation can also be bound to 
		/// the form and|or elements nested as long as the helper is 
		/// able to reach the <see cref="Type"/> of the object used on your view code
		/// </para>
		/// <para>
		/// The action attribute generation will use <see cref="UrlHelper"/>
		/// </para>
		/// </summary>
		/// 
		/// <example>
		/// 
		/// <code lang="none">
		/// $Form.FormTag('mytarget.castle', "%{id='productform'}")
		/// </code>
		/// 
		/// Outputs:
		/// 
		/// <code lang="xml">
		/// &lt;form method='post' action='mytarget.castle' id='productform'&gt;
		/// </code>
		///
		/// </example>
		/// 
		/// <remarks>
		/// The following parameters are accepted.
		/// 
		/// <list type="table">
		/// <term>
		///		<term>method</term>
		///		<description>string. The http method to use. Defaults to <c>post</c></description>
		/// </term>
		/// <term>
		///		<term>id</term>
		///		<description>string. The form id.</description>
		/// </term>
		/// </list>
		/// 
		/// More parameters can be accepted depending on the form validation strategy you are using (if any).
		/// 
		/// </remarks>
		/// 
		/// <param name="url">The hardcoded url.</param>
		/// <param name="parameters">The parameters for the tag or for action and form validation generation.</param>
		/// <returns></returns>
		public string FormTag(string url, IDictionary parameters)
		{
			string method = CommonUtils.ObtainEntryAndRemove(parameters, "method", "post");
			currentFormId = CommonUtils.ObtainEntryAndRemove(parameters, "id", "form" + ++formCount);

			validationConfig = validatorProvider.CreateConfiguration(parameters);

			string afterFormTag = IsValidationEnabled ? 
				validationConfig.CreateAfterFormOpened(currentFormId) : 
				String.Empty;

			string formContent;

			if (url != null)
			{
				formContent = "<form action='" + url + "' method='" + method + "' " +
					"id='" + currentFormId + "' " + GetAttributes(parameters) + ">";
			}
			else
			{
				formContent = "<form method='" + method + "' id='" + currentFormId + "' " + GetAttributes(parameters) + ">";
			}

			return formContent + afterFormTag;
		}

		/// <summary>
		/// Generate Ajax form tag for ajax based form submission. Experimental.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public string AjaxFormTag(IDictionary parameters)
		{
			currentFormId = CommonUtils.ObtainEntryAndRemove(parameters, "id", "form" + ++formCount);
			
			validationConfig = validatorProvider.CreateConfiguration(parameters);

			string afterFormTag = IsValidationEnabled ?
				validationConfig.CreateAfterFormOpened(currentFormId) :
				String.Empty;

			string url = UrlHelper.For(parameters);

			parameters["form"] = true;

			if (parameters.Contains("onsubmit"))
			{
				string onSubmitFunc = CommonUtils.ObtainEntryAndRemove(parameters, "onsubmit");
				//remove return to make it compatible for ajax condition
				if (onSubmitFunc.StartsWith("return ", StringComparison.InvariantCultureIgnoreCase))
				{
					onSubmitFunc = onSubmitFunc.Substring(7);
				}
				if (onSubmitFunc.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
				{
					onSubmitFunc = onSubmitFunc.Remove(onSubmitFunc.Length - 1);
				}
				string conditionFunc = CommonUtils.ObtainEntryAndRemove(parameters, "condition", string.Empty);
				if (!string.IsNullOrEmpty(conditionFunc))
				{
					conditionFunc += " && ";
				}
				conditionFunc += onSubmitFunc;
				
				parameters["condition"] = conditionFunc;
			}
			bool isMethodAssigned = parameters.Contains("method");

			string method = CommonUtils.ObtainEntryAndRemove(parameters, "method", "post");
			
			parameters["url"] = url;
			
			// reassign method so in case if there is no value the default is assigned.
			
			if (isMethodAssigned)
			{
				parameters["method"] = method;
			}

			String remoteFunc = new AjaxHelper().RemoteFunction(parameters);

			string formContent = String.Format("<form id='{1}' method='{2}' {3} onsubmit=\"{0}; return false;\" enctype=\"multipart/form-data\">", remoteFunc, currentFormId, method,GetAttributes(parameters));

			return formContent + afterFormTag;
		}

		/// <summary>
		/// Renders an end form element.
		/// </summary>
		/// <remarks>
		/// Should be used if you are using form validation. Some validation approaches
		/// uses the end form before or after appending a javascript snippet.
		/// </remarks>
		/// <returns></returns>
		public string EndFormTag()
		{
			string beforeEndTag = string.Empty;

			if (validationConfig != null)
			{
				beforeEndTag = IsValidationEnabled ?
					validationConfig.CreateBeforeFormClosed(currentFormId) :
					String.Empty;
			}
			
			return beforeEndTag + "</form>";
		}

		#endregion

		#region Object scope related

		/// <summary>
		/// Pushes the specified target. Experimental.
		/// </summary>
		/// <param name="target">The target.</param>
		public void Push(string target)
		{
			Push(target, null);
		}

		/// <summary>
		/// Pushes the specified target. Experimental.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="parameters">The parameters.</param>
		public void Push(string target, IDictionary parameters)
		{
			string disableValidation = CommonUtils.ObtainEntryAndRemove(parameters, "disablevalidation", "false");
			object value = ObtainValue(target);

			if (value != null)
			{
				objectStack.Push(new FormScopeInfo(target, disableValidation != "true"));
			}
			else
			{
				value = ObtainValue(target + "type");

				if (value != null)
				{
					objectStack.Push(new FormScopeInfo(target, disableValidation != "true"));
				}
				else
				{
					throw new ArgumentException("target could not be evaluated during Push operation. Target: " + target);
				}
			}
		}

		/// <summary>
		/// Pops this instance. Experimental.
		/// </summary>
		public void Pop()
		{
			objectStack.Pop();
		}

		#endregion

		#region Submit and Button related

		/// <summary>
		/// Generates an input submit element.
		/// </summary>
		/// <param name="value">The value/caption for the button.</param>
		/// <returns>The element tag</returns>
		public string Submit(string value)
		{
			return Submit(value, null);
		}

		/// <summary>
		/// Generates an input submit element.
		/// </summary>
		/// <param name="value">The value/caption for the button.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The element tag</returns>
		public string Submit(string value, IDictionary attributes)
		{
			return CreateInputElement("submit", value, attributes);
		}

		/// <summary>
		/// Generates an graphical submit element.
		/// </summary>
		/// <param name="imgsrc">The path the image file.</param>
		/// <param name="alttext">The alt text displayed by screenreaders, or when images are not enabled.</param>
		/// <returns>The element tag</returns>
		public string ImageSubmit(string imgsrc, string alttext)
		{
			return ImageSubmit(imgsrc, alttext, null);
		}

		/// <summary>
		/// Generates an input submit element.
		/// </summary>
		/// <param name="imgsrc">The path the image file.</param>
		/// <param name="alttext">The alt text displayed by screenreaders, or when images are not enabled.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The element tag</returns>
		public string ImageSubmit(string imgsrc, string alttext, IDictionary attributes)
		{
			attributes = attributes != null ? attributes : new Hashtable();
			attributes["src"] = imgsrc;
			attributes["alt"] = alttext;
			return CreateInputElement("image", alttext, attributes);
		}
		
		/// <summary>
		/// Generates an input button element.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The element tag</returns>
		public string Button(string value)
		{
			return Button(value, null);
		}

		/// <summary>
		/// Generates an input button element.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The element tag</returns>
		public string Button(string value, IDictionary attributes)
		{
			return CreateInputElement("button", value, attributes);
		}

		/// <summary>
		/// Creates a basic button element of type submit.
		/// </summary>
		/// <param name="innerText">The inner text of the button element.</param>
		/// <returns>The generated button element.</returns>
		public string ButtonElement(string innerText)
		{
			return ButtonElement(innerText, "submit", null);
		}

		/// <summary>
		/// Creates a basic button element of the specified type.
		/// </summary>
		/// <param name="innerText">The inner text of the button element.</param>
		/// <param name="type">The type of the button.</param>
		/// <returns>The generated button element.</returns>
		public string ButtonElement(string innerText, string type)
		{
			return ButtonElement(innerText, type, null);
		}

		/// <summary>
		/// Creates a basic button element of the specified type and with specified attributes.
		/// </summary>
		/// <param name="innerText">The inner text of the button element.</param>
		/// <param name="type">The type of the button.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated button element.</returns>
		public string ButtonElement(string innerText, string type, IDictionary attributes)
		{
			return String.Format("<button type=\"{0}\" {1}>{2}</button>", type, GetAttributes(attributes), innerText);
		}

		#endregion

		#region TextFieldValue

		/// <summary>
		/// Generates an input text form element
		/// with the supplied value
		/// </summary>
		/// <param name="target">The string to be used to create the element name.</param>
		/// <param name="value">Value to supply to the element (instead of querying the target)</param>
		/// <returns>The generated form element</returns>
		public string TextFieldValue(string target, object value)
		{
			return TextFieldValue(target, value, null);
		}

		/// <summary>
		/// Generates an input text form element
		/// with the supplied value
		/// </summary>
		/// <param name="target">The string to be used to create the element name.</param>
		/// <param name="value">Value to supply to the element (instead of querying the target)</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string TextFieldValue(string target, object value, IDictionary attributes)
		{
			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region TextField

		/// <summary>
		/// Generates an input text element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// 
		/// <example>
		/// The following example assumes that an entry <c>username</c> exists on the
		/// <see cref="Controller.PropertyBag"/> or <see cref="Controller.Flash"/> or <see cref="Controller.Session"/>
		/// 
		/// <code lang="none">
		/// $Form.TextField('username')
		/// </code>
		/// Outputs:
		/// <code lang="xml">
		/// &lt;input type='text' name='username' id='username' value='John Doe' /&gt;
		/// </code>
		/// 
		/// <para>
		/// The following example assumes that an entry <c>user</c> exists on the
		/// <see cref="Controller.PropertyBag"/> or <see cref="Controller.Flash"/> or <see cref="Controller.Session"/>
		/// </para>
		/// 
		/// <code lang="none">
		/// $Form.TextField('user.name')
		/// </code>
		/// Outputs:
		/// <code lang="xml">
		/// &lt;input type='text' name='user.name' id='user_name' value='John Doe' /&gt;
		/// </code>
		/// </example>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string TextField(string target)
		{
			return TextField(target, null);
		}

		/// <summary>
		/// Generates an input text element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// 
		/// <seealso cref="TextField(string)"/>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string TextField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region FilteredTextField

		/// <summary>
		/// Generates an input text element with a javascript that prevents the 
		/// chars listed in the forbid attribute from being entered.
		/// </summary>
		/// <para>
		/// You must pass an <c>forbid</c> value through the dictionary.
		/// It must be a comma separated list of chars that cannot be accepted on the field. 
		/// For example:
		/// </para>
		/// <code>
		/// FormHelper.FilteredTextField("product.price", {forbid='46'})
		/// </code>
		/// In this case the key code 46 (period) will not be accepted on the field.
		/// <para>
		/// The value is extracted from the target (if available).
		/// </para>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element.</returns>
		/// <remarks>
		/// You must invoke <see cref="FormHelper.InstallScripts"/> before using it.
		/// </remarks>
		public string FilteredTextField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			attributes = attributes != null ? attributes : new Hashtable();

			ApplyFilterOptions(attributes);
			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region NumberField

		/// <summary>
		/// Generates an input text element with a javascript that prevents
		/// chars other than numbers from being entered.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// 
		/// <seealso cref="InstallScripts"/>
		/// <seealso cref="NumberField(string,IDictionary)"/>
		/// 
		/// <remarks>
		/// You must include the formhelper javascript functions to use the NumberField. 
		/// See <see cref="InstallScripts"/>
		/// </remarks>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string NumberField(string target)
		{
			return NumberField(target, null);
		}

		/// <summary>
		/// Generates an input text element with a javascript that prevents
		/// chars other than numbers from being entered.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		/// 
		/// <remarks>
		/// You must include the formhelper javascript functions to use the NumberField. 
		/// See <see cref="InstallScripts"/>
		/// <para>
		/// You can optionally pass an <c>exceptions</c> value through the dictionary.
		/// It must be a comma separated list of chars that can be accepted on the field. 
		/// For example:
		/// </para>
		/// <code>
		/// FormHelper.NumberField("product.price", {exceptions='13,10,11'})
		/// </code>
		/// In this case the key codes 13, 10 and 11 will be accepted on the field.
		/// <para>
		/// You can aslo optionally pass an <c>forbid</c> value through the dictionary.
		/// It must be a comma separated list of chars that cannot be accepted on the field. 
		/// For example:
		/// </para>
		/// <code>
		/// FormHelper.NumberField("product.price", {forbid='46'})
		/// </code>
		/// In this case the key code 46 (period) will not be accepted on the field.
		/// </remarks>
		public string NumberField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			attributes = attributes != null ? attributes : new Hashtable();

			ApplyNumberOnlyOptions(attributes);
			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region NumberFieldValue

		/// <summary>
		/// Generates an input text element with a javascript that prevents
		/// chars other than numbers from being entered. The value is not gathered 
		/// from the context, instead you specify it on the second argument
		/// </summary>
		/// 
		/// <seealso cref="InstallScripts"/>
		/// <seealso cref="NumberField(string,IDictionary)"/>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value">The current value to output.</param>
		/// <returns>The generated form element</returns>
		/// 
		/// <remarks>
		/// You must include the formhelper javascript functions to use the NumberField. 
		/// See <see cref="InstallScripts"/>
		/// </remarks>
		public string NumberFieldValue(string target, object value)
		{
			return NumberFieldValue(target, value, null);
		}

		/// <summary>
		/// Generates an input text element with a javascript that prevents
		/// chars other than numbers from being entered. The value is not gathered 
		/// from the context, instead you specify it on the second argument
		/// </summary>
		/// 
		/// <seealso cref="InstallScripts"/>
		/// <seealso cref="NumberField(string,IDictionary)"/>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value">The current value to output.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		/// 
		/// <remarks>
		/// You must include the formhelper javascript functions to use the NumberField. 
		/// See <see cref="InstallScripts"/>
		/// </remarks>
		public string NumberFieldValue(string target, object value, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			attributes = attributes ?? new Hashtable();

			ApplyNumberOnlyOptions(attributes);
			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("text", target, value, attributes);
		}

		#endregion

		#region TextArea

		/// <summary>
		/// Generates a textarea element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string TextArea(string target)
		{
			return TextArea(target, null);
		}

		/// <summary>
		/// Generates a textarea element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string TextArea(string target, IDictionary attributes)
		{
			string targetForValue = RewriteTargetIfWithinObjectScope(target);
			object value = ObtainValue(targetForValue);
			return TextAreaValue(target, value, attributes);
		}

		/// <summary>
		/// Generates a textarea element with a specified value.
		/// </summary>
		/// <param name="target">The target to base the element name on.</param>
		/// <param name="value">The value to apply to the field.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string TextAreaValue(string target, object value, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			value = value == null ? "" : HtmlEncode(value.ToString());

			string id = CreateHtmlId(target);

			ApplyValidation(InputElementType.Text, target, ref attributes);

			return String.Format("<textarea id=\"{0}\" name=\"{1}\" {2}>{3}</textarea>",
				id, target, GetAttributes(attributes), FormatIfNecessary(value, attributes));
		}

		#endregion

		#region PasswordField

		/// <summary>
		/// Generates a password input field.
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string PasswordField(string target)
		{
			return PasswordField(target, null);
		}

		/// <summary>
		/// Generates a password input field.
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string PasswordField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("password", target, value, attributes);
		}

		#endregion

		#region PasswordNumberField

		/// <summary>
		/// Generates an input password element with a javascript that prevents
		/// chars other than numbers from being entered.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// <remarks>
		/// You must invoke <see cref="FormHelper.InstallScripts"/> before using it
		/// </remarks>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string PasswordNumberField(string target)
		{
			return PasswordNumberField(target, null);
		}

		/// <summary>
		/// Generates an input password element with a javascript that prevents
		/// chars other than numbers from being entered.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// <para>
		/// You can optionally pass an <c>exceptions</c> value through the dictionary.
		/// It must be a comma separated list of chars that can be accepted on the field. 
		/// For example:
		/// </para>
		/// <code>
		/// FormHelper.NumberField("product.price", {exceptions='13,10,11'})
		/// </code>
		/// In this case the key codes 13, 10 and 11 will be accepted on the field.
		/// <para>
		/// You can aslo optionally pass an <c>forbid</c> value through the dictionary.
		/// It must be a comma separated list of chars that cannot be accepted on the field. 
		/// For example:
		/// </para>
		/// <code>
		/// FormHelper.NumberField("product.price", {forbid='46'})
		/// </code>
		/// In this case the key code 46 (period) will not be accepted on the field.
		/// </summary>
		/// <remarks>
		/// You must invoke <see cref="FormHelper.InstallScripts"/> before using it
		/// </remarks>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string PasswordNumberField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			attributes = attributes != null ? attributes : new Hashtable();

			ApplyNumberOnlyOptions(attributes);
			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("password", target, value, attributes);
		}

		#endregion

		#region LabelFor

		/// <summary>
		/// Generates a label element.
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="label">Legend</param>
		/// <returns>The generated form element</returns>
		public string LabelFor(string target, string label)
		{
			return LabelFor(target, label, null);
		}
		
		/// <summary>
		/// Generates a label element.
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="label">Legend</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string LabelFor(string target, string label, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			string id = CreateHtmlId(attributes, target);

			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("label");
			writer.WriteAttribute("for", id);
			string strAttributes = GetAttributes(attributes);
			if (strAttributes != String.Empty) writer.Write(HtmlTextWriter.SpaceChar);

			writer.Write(strAttributes); 
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.Write(label);
			writer.WriteEndTag("label");

			return sbWriter.ToString();
		}

		#endregion

		#region HiddenField

		/// <summary>
		/// Generates a hidden form element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string HiddenField(string target)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			return CreateInputElement("hidden", target, value, null);
		}

		/// <summary>
		/// Generates a hidden form element with the specified value
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value">The value for the hidden field</param>
		/// <returns>The generated form element</returns>
		public string HiddenField(string target, object value)
		{
			return CreateInputElement("hidden", target, value, null);
		}
		
		/// <summary>
		/// Generates a hidden form element.
		/// <para>
		/// The value is extracted from the target (if available)
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string HiddenField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);
			
			string id = CreateHtmlId(attributes, target);
			
			value = value != null ? value : String.Empty;

			return CreateInputElement("hidden", id, target, value.ToString(), attributes);
		}
		
		/// <summary>
		/// Generates a hidden form element with the specified value
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value">The value for the hidden field</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string HiddenField(string target, object value, IDictionary attributes)
		{
			return CreateInputElement("hidden", target, value, attributes);
		}

		#endregion

		#region CheckboxList

		/// <summary>
		/// Creates a <see cref="CheckboxList"/> instance
		/// which is enumerable. For each interaction you can invoke
		/// <see cref="CheckboxList.Item()"/> which will correctly render
		/// a checkbox input element for the current element on the supplied set (<c>dataSource</c>).
		/// <para>
		/// The enumerable item will be an element of the <c>dataSource</c>.
		/// </para>
		/// If the <c>dataSource</c>
		/// elements are complex objects (ie not string or primitives), 
		/// supply the parameters <c>value</c> and <c>text</c> to the dictionary to make
		/// the helper use the specified properties to extract the <c>option</c> value and content respectively.
		/// <para>
		/// Usually both the <c>target</c> and obviously the <c>dataSource</c> are sets
		/// with multiple items. The element types tend to be the same. If 
		/// they are not, you might have to specify the <c>suffix</c> parameters on 
		/// the <c>attributes</c> as it would not be inferred.
		/// </para>
		/// </summary>
		/// 
		/// <example>
		/// Consider the following action code:
		/// <code>
		/// public void Index()
		/// {
		///     // data source
		///     PropertyBag["primenumbers"] = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23 };
		///     
		///     // initial selection
		///     PropertyBag["selectedPrimes"] = new int[] { 11, 19 };
		/// }
		/// </code>
		/// 
		/// And the respective view code
		/// 
		/// <code lang="none">
		/// #set($items = $FormHelper.CreateCheckboxList("selectedPrimes", $primenumbers))
		/// 
		/// #foreach($elem in $items)
		///   $items.Item()  $elem 
		/// #end
		/// </code>
		/// 
		/// That will generates the following html:
		/// 
		/// <code lang="none">
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_0_&quot; name=&quot;selectedPrimes[0]&quot; value=&quot;2&quot; /&gt;  2   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_1_&quot; name=&quot;selectedPrimes[1]&quot; value=&quot;3&quot; /&gt;  3   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_2_&quot; name=&quot;selectedPrimes[2]&quot; value=&quot;5&quot; /&gt;  5   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_3_&quot; name=&quot;selectedPrimes[3]&quot; value=&quot;7&quot; /&gt;  7   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_4_&quot; name=&quot;selectedPrimes[4]&quot; value=&quot;11&quot; checked=&quot;checked&quot; /&gt;  11  
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_5_&quot; name=&quot;selectedPrimes[5]&quot; value=&quot;13&quot; /&gt;  13   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_6_&quot; name=&quot;selectedPrimes[6]&quot; value=&quot;17&quot; /&gt;  17   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_7_&quot; name=&quot;selectedPrimes[7]&quot; value=&quot;19&quot; checked=&quot;checked&quot; /&gt;  19  
		///   &lt;input type=&quot;checkbox&quot; id=&quot;selectedPrimes_8_&quot; name=&quot;selectedPrimes[8]&quot; value=&quot;23&quot; /&gt;  23   
		/// </code>
		/// 
		/// <para>
		/// To customize the id, you can call the <see cref="CheckboxList.Item(string)"/> overload:
		/// </para>
		/// 
		/// <code lang="none">
		/// #set($items = $FormHelper.CreateCheckboxList("selectedPrimes", $primenumbers))
		/// 
		/// #foreach($elem in $items)
		///   $items.Item("myId${velocityCount}") $Form.LabelFor("myId${velocityCount}", $elem.ToString()) <br/>
		/// #end
		/// </code>
		/// </example>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="dataSource">The set of available elements</param>
		/// <returns>The generated form element</returns>
		public CheckboxList CreateCheckboxList(string target, IEnumerable dataSource)
		{
			return CreateCheckboxList(target, dataSource, null);
		}

		/// <summary>
		/// Creates a <see cref="CheckboxList"/> instance
		/// which is enumerable. For each interaction you can invoke
		/// <see cref="CheckboxList.Item()"/> which will correctly render
		/// a checkbox input element for the current element on the supplied set (<c>dataSource</c>).
		/// <para>
		/// The enumerable item will be an element of the <c>dataSource</c>.
		/// </para>
		/// If the <c>dataSource</c>
		/// elements are complex objects (ie not string or primitives), 
		/// supply the parameters <c>value</c> and <c>text</c> to the dictionary to make
		/// the helper use the specified properties to extract the <c>option</c> value and content respectively.
		/// <para>
		/// Usually both the <c>target</c> and obviously the <c>dataSource</c> are sets
		/// with multiple items. The element types tend to be the same. If 
		/// they are not, you might have to specify the <c>suffix</c> parameters on 
		/// the <c>attributes</c> as it would not be inferred.
		/// </para>
		/// </summary>
		/// 
		/// <seealso cref="CreateCheckboxList(string,IEnumerable)"/>
		///
		/// <example>
		/// Consider the following action code:
		/// <code>
		/// public void Index()
		/// {
		///		Category[] categories = new Category[] { new Category(1, "Music"), new Category(2, "Humor"), new Category(3, "Politics")  };
		///		PropertyBag["categories"] = categories; // datasource
		/// 
		///     Blog blog = new Blog();
		///     blog.Categories = new Category[] { new Category(2, "Humor") }; // initial selection
		///		PropertyBag["blog"] = blog;
		/// }
		/// </code>
		/// 
		/// And the respective view code
		/// 
		/// <code lang="none">
		/// #set($items = $Form.CreateCheckboxList("blog.categories", $categories, "%{value='Id'}"))
		/// 
		/// #foreach($elem in $items)
		///   $items.Item()  $elem  
		/// #end
		/// </code>
		/// 
		/// That will generates the following html:
		/// 
		/// <code lang="none">
		///   &lt;input type=&quot;checkbox&quot; id=&quot;blog_categories_0_&quot; name=&quot;blog.categories[0].Id&quot; value=&quot;1&quot; /&gt;  Music   
		///   &lt;input type=&quot;checkbox&quot; id=&quot;blog_categories_1_&quot; name=&quot;blog.categories[1].Id&quot; value=&quot;2&quot; checked=&quot;checked&quot; /&gt;  Humor  
		///   &lt;input type=&quot;checkbox&quot; id=&quot;blog_categories_2_&quot; name=&quot;blog.categories[2].Id&quot; value=&quot;3&quot; /&gt;  Politics  
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="dataSource">The set of available elements</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public CheckboxList CreateCheckboxList(string target, IEnumerable dataSource, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);
			
			return new CheckboxList(this, target, value, dataSource, attributes);
		}
		
		/// <summary>
		/// Outputs a checkbox element (for internal use)
		/// </summary>
		/// <param name="index"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="suffix"></param>
		/// <param name="item"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		internal string CheckboxItem(int index, string target, string suffix, SetItem item, IDictionary attributes)
		{
			if (item.IsSelected)
			{
				AddChecked(attributes);
			}
			else
			{
				RemoveChecked(attributes);
			}

			target = String.Format("{0}[{1}]", target, index);
			
			string elementId = CreateHtmlId(attributes, target, true);
			
			string computedTarget = target;
			
			if (suffix != null && suffix != String.Empty)
			{
				computedTarget += "." + suffix;
			}

			return CreateInputElement("checkbox", elementId, computedTarget, item.Value, attributes);
		}

		/// <summary>
		/// This class is an enumerable list of checkboxes. 
		/// It uses the <see cref="OperationState"/> to manage the sets
		/// and to control the check/uncheck state.
		/// </summary>
		public sealed class CheckboxList : IEnumerable, IEnumerator
		{
			private readonly FormHelper helper;
			private readonly string target;
			private readonly IDictionary attributes;
			private readonly OperationState operationState;
			private readonly IEnumerator enumerator;
			private bool hasMovedNext, hasItem;
			private int index = -1;

			/// <summary>
			/// Initializes a new instance of the <see cref="CheckboxList"/> class.
			/// </summary>
			/// <param name="helper">The helper.</param>
			/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
			/// <param name="initialSelectionSet">The initial selection set.</param>
			/// <param name="dataSource">The set of available elements</param>
			/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
			public CheckboxList(FormHelper helper, string target,
								object initialSelectionSet, IEnumerable dataSource, IDictionary attributes)
			{
				if (dataSource == null) throw new ArgumentNullException("dataSource");

				this.helper = helper;
				this.target = target;
				this.attributes = attributes ?? new HybridDictionary(true);
				
				operationState = SetOperation.IterateOnDataSource(initialSelectionSet, dataSource, attributes);
				enumerator = operationState.GetEnumerator();
			}
			
			/// <summary>
			/// Outputs the Checkbox in the correct state (checked/unchecked) based
			/// on the Set. 
			/// <seealso cref="FormHelper.CreateCheckboxList(string,IEnumerable,IDictionary)"/>
			/// </summary>
			/// <returns>The generated input element</returns>
			public string Item()
			{
				return Item(null);
			}

			/// <summary>
			/// Outputs the Checkbox in the correct state (checked/unchecked) based
			/// on the Set. 
			/// <seealso cref="FormHelper.CreateCheckboxList(string,IEnumerable,IDictionary)"/>
			/// </summary>
			/// <param name="id">The element id</param>
			/// <returns>The generated input element</returns>
			public string Item(string id)
			{
				if (!hasMovedNext)
				{
					throw new InvalidOperationException("Before rendering a checkbox item, you must use MoveNext");
				}

				if (!hasItem)
				{
					// Nothing to render
					return String.Empty;
				}

				if (id != null)
				{
					attributes["id"] = id;
				}

				return helper.CheckboxItem(index, target, operationState.TargetSuffix, CurrentSetItem, attributes);
			}

			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
			/// </returns>
			public IEnumerator GetEnumerator()
			{
				return this;
			}

			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
			/// </returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNext()
			{
				hasMovedNext = true;
				hasItem = enumerator.MoveNext();
				
				if (hasItem) index++;
				
				return hasItem;
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public void Reset()
			{
				index = -1;
				enumerator.Reset();
			}

			/// <summary>
			/// Gets the current element in the collection.
			/// </summary>
			/// <value></value>
			/// <returns>The current element in the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
			public object Current
			{
				get { return CurrentSetItem.Item; }
			}

			/// <summary>
			/// Gets the current set item.
			/// </summary>
			/// <value>The current set item.</value>
			public SetItem CurrentSetItem
			{
				get { return enumerator.Current as SetItem; }
			}
		}

		#endregion
		
		#region CheckboxField

		/// <summary>
		/// Generates a checkbox field. In fact it generates two as a
		/// way to send a value if the primary checkbox is not checked.
		/// This allow the process the be aware of the unchecked value
		/// and act accordingly.
		/// </summary>
		/// 
		/// <example>
		/// Consider the following view code:
		/// 
		/// <code lang="none">
		/// $Form.CheckboxField('user.disabled')
		/// </code>
		/// 
		/// That is going to output:
		/// 
		/// <code lang="none">
		///  &lt;input type=&quot;checkbox&quot; id=&quot;user_disabled&quot; name=&quot;user.disabled&quot; value=&quot;true&quot; /&gt;
		///  &lt;input type=&quot;hidden&quot; id=&quot;user_disabledH&quot; name=&quot;user.disabled&quot; value=&quot;false&quot; /&gt; 
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		public string CheckboxField(string target)
		{
			return CheckboxField(target, null);
		}

		/// <summary>
		/// Generates a checkbox field. In fact it generates two as a
		/// way to send a value if the primary checkbox is not checked.
		/// This allow the process the be aware of the unchecked value
		/// and act accordingly.
		/// 
		/// <para>
		/// The checked and unchecked values sent to the server defaults
		/// to true and false. You can override them using the 
		/// parameters <c>trueValue</c> and <c>falseValue</c>, but the DataBinder is prepared only
		/// to treat boolean arrays. 
		/// </para>
		/// 
		/// </summary>
		/// 
		/// <seealso cref="CheckboxField(string)"/>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string CheckboxField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			string trueValue = CommonUtils.ObtainEntryAndRemove(attributes, "trueValue", "true");
			
			bool isChecked;

			if (trueValue != "true")
			{
				isChecked = AreEqual(value, trueValue);
			}
			else
			{
				isChecked = ((value != null && value is bool && ((bool)value)) || 
							 (!(value is bool) && (value != null)));
			}

			if (isChecked)
			{
				if (attributes == null)
				{
					attributes = new HybridDictionary(true);
				}

				AddChecked(attributes);
			}

			ApplyValidation(InputElementType.Checkbox, target, ref attributes);

			string id = CreateHtmlId(attributes, target);
			string hiddenElementId = id + "H";
			string hiddenElementValue = CommonUtils.ObtainEntryAndRemove(attributes, "falseValue", "false");

			string result = CreateInputElement("checkbox", id, target, trueValue, attributes);
			
			result += CreateInputElement("hidden", hiddenElementId, target, hiddenElementValue, null);
			
			return result;
		}

		#endregion

		#region RadioField

		/// <summary>
		/// Generates a radio input type with the specified 
		/// value to send to the served in case the element in checked.
		/// It will automatically check the radio if the target 
		/// evaluated value is equal to the specified <c>valueToSend</c>.
		/// </summary>
		/// 
		/// <example>
		///	Consider the following action code:
		/// 
		/// <code>
		/// public void Index()
		/// {
		///		PropertyBag["mode"] = FileMode.Truncate;
		/// }
		/// </code>
		/// 
		/// And the following view code:
		/// 
		/// <code lang="none">
		///   $Form.RadioField("mode", "Append") FileMode.Append 
		///   $Form.RadioField("mode", "Create") FileMode.Create 
		///   $Form.RadioField("mode", "CreateNew") FileMode.CreateNew 
		///   $Form.RadioField("mode", "Open") FileMode.Open 
		///   $Form.RadioField("mode", "OpenOrCreate", "%{id='customhtmlid'}") FileMode.OpenOrCreate 
		///   $Form.RadioField("mode", "Truncate") FileMode.Truncate 
		/// </code>
		/// 
		/// That is going to output:
		/// 
		/// <code lang="none">
		///  &lt;input type=&quot;radio&quot; id=&quot;mode&quot; name=&quot;mode&quot; value=&quot;Append&quot; /&gt; FileMode.Append 
		///  &lt;input type=&quot;radio&quot; id=&quot;mode&quot; name=&quot;mode&quot; value=&quot;Create&quot; /&gt; FileMode.Create 
		///  &lt;input type=&quot;radio&quot; id=&quot;mode&quot; name=&quot;mode&quot; value=&quot;CreateNew&quot; /&gt; FileMode.CreateNew  
		///  &lt;input type=&quot;radio&quot; id=&quot;mode&quot; name=&quot;mode&quot; value=&quot;Open&quot; /&gt; FileMode.Open 
		///  &lt;input type=&quot;radio&quot; id=&quot;customhtmlid&quot; name=&quot;mode&quot; value=&quot;OpenOrCreate&quot; /&gt; FileMode.OpenOrCreate  
		///  &lt;input type=&quot;radio&quot; id=&quot;mode&quot; name=&quot;mode&quot; value=&quot;Truncate&quot; checked=&quot;checked&quot; /&gt; FileMode.Truncate 
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="valueToSend"></param>
		/// <returns>The generated form element</returns>
		public string RadioField(string target, object valueToSend)
		{
			return RadioField(target, valueToSend, null);
		}

		/// <summary>
		/// Generates a radio input type with the specified 
		/// value to send to the served in case the element in checked.
		/// It will automatically check the radio if the target 
		/// evaluated value is equal to the specified <c>valueToSend</c>.
		/// </summary>
		/// 
		/// <seealso cref="RadioField(string,object)"/>
		/// 
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="valueToSend"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string RadioField(string target, object valueToSend, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object value = ObtainValue(target);

			bool isChecked = AreEqual(valueToSend, value);

			if (isChecked)
			{
				if (attributes == null)
				{
					attributes = new HybridDictionary(true);
				}

				AddChecked(attributes);
			}

			return CreateInputElement("radio", target, valueToSend, attributes);
		}

		#endregion

		#region FileField

		/// <summary>
		/// Generates an input file element.
		/// <para>
		/// Dirrently than other operations exposed by this helper, 
		/// no value is extracted for this operation
		/// </para>
		/// </summary>
		/// <param name="target">The object to be based on when creating the element name.</param>
		/// <returns>The generated form element</returns>
		public string FileField(string target)
		{
			return FileField(target, null);
		}

		/// <summary>
		/// Generates an input file element.
		/// <para>
		/// Dirrently than other operations exposed by this helper, 
		/// no value is extracted for this operation
		/// </para>
		/// </summary>
		/// <param name="target">The object to be based on when creating the element name.</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string FileField(string target, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			ApplyValidation(InputElementType.Text, target, ref attributes);

			return CreateInputElement("file", target, string.Empty, attributes);
		}

		#endregion

		#region Select

		/// <summary>
		/// Creates a <c>select</c> element and its <c>option</c>s based on the <c>dataSource</c>.
		/// If the <c>dataSource</c>
		/// elements are complex objects (ie not string or primitives), 
		/// supply the parameters <c>value</c> and <c>text</c> to the dictionary to make
		/// the helper use the specified properties to extract the <c>option</c> value and content respectively.
		/// <para>
		/// You can also specify the attribute <c>firstoption</c> to force the first option be
		/// something like 'please select'. You can set the value of <c>firstoption</c> by specifying the attribute
		/// <c>firstoptionvalue</c>. The default value is '0'.
		/// </para>
		/// <para>
		/// Usually the <c>target</c> is a single value and the <c>dataSource</c> is obviously 
		/// a set with multiple items. The element types tend to be the same. If 
		/// they are not, you might have to specify the <c>suffix</c> parameters on 
		/// the <c>attributes</c> as it would not be inferred.
		/// </para>
		/// <para>
		/// The target can also be a set. In this case the intersection will be 
		/// the initially selected elements.
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="dataSource">The set of available elements</param>
		/// <returns>The generated form element</returns>
		public string Select(string target, IEnumerable dataSource)
		{
			return Select(target, dataSource, null);
		}

		/// <summary>
		/// Creates a <c>select</c> element and its <c>option</c>s based on the <c>dataSource</c>.
		/// If the <c>dataSource</c>
		/// elements are complex objects (ie not string or primitives), 
		/// supply the parameters <c>value</c> and <c>text</c> to the dictionary to make
		/// the helper use the specified properties to extract the <c>option</c> value and content respectively.
		/// <para>
		/// You can also specify the attribute <c>firstoption</c> to force the first option be
		/// something like 'please select'. You can set the value of <c>firstoption</c> by specifying the attribute
		/// <c>firstoptionvalue</c>. The default value is '0'.
		/// </para>
		/// <para>
		/// Usually the <c>target</c> is a single value and the <c>dataSource</c> is obviously 
		/// a set with multiple items. The element types tend to be the same. If 
		/// they are not, you might have to specify the <c>suffix</c> parameters on 
		/// the <c>attributes</c> as it would not be inferred.
		/// </para>
		/// <para>
		/// The target can also be a set. In this case the intersection will be 
		/// the initially selected elements.
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="dataSource">The set of available elements</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string Select(string target, IEnumerable dataSource, IDictionary attributes)
		{
			target = RewriteTargetIfWithinObjectScope(target);

			object selectedValue = ObtainValue(target);

			return Select(target, selectedValue, dataSource, attributes);
		}
		
		/// <summary>
		/// Creates a <c>select</c> element and its <c>option</c>s based on the <c>dataSource</c>.
		/// If the <c>dataSource</c>
		/// elements are complex objects (ie not string or primitives), 
		/// supply the parameters <c>value</c> and <c>text</c> to the dictionary to make
		/// the helper use the specified properties to extract the <c>option</c> value and content respectively.
		/// <para>
		/// You can also specify the attribute <c>firstoption</c> to force the first option be
		/// something like 'please select'. You can set the value of <c>firstoption</c> by specifying the attribute
		/// <c>firstoptionvalue</c>. The default value is '0'.
		/// </para>
		/// <para>
		/// Usually the <c>target</c> is a single value and the <c>dataSource</c> is obviously 
		/// a set with multiple items. The element types tend to be the same. If 
		/// they are not, you might have to specify the <c>suffix</c> parameters on 
		/// the <c>attributes</c> as it would not be inferred.
		/// </para>
		/// <para>
		/// The target can also be a set. In this case the intersection will be 
		/// the initially selected elements.
		/// </para>
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="selectedValue"></param>
		/// <param name="dataSource">The set of available elements</param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		public string Select(string target, object selectedValue, IEnumerable dataSource, IDictionary attributes)
		{
			return GenerateSelect(target, selectedValue, dataSource, attributes);
		}

		/// <summary>
		/// Generates the select.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="selectedValue">The selected value.</param>
		/// <param name="dataSource">The data source.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		protected virtual string GenerateSelect(string target, object selectedValue, IEnumerable dataSource, IDictionary attributes)
		{
			string id = CreateHtmlId(target);

			ApplyValidation(InputElementType.Select, target, ref attributes);

			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			string firstOption = null;
			string firstOptionValue = null;
			string name = target;

			if (attributes != null)
			{
				firstOption = CommonUtils.ObtainEntryAndRemove(attributes, "firstoption");
				firstOptionValue = CommonUtils.ObtainEntryAndRemove(attributes, "firstoptionvalue");

				if (attributes.Contains("name"))
				{
					name = (String) attributes["name"];
					attributes.Remove("name");
				}

				if (attributes.Contains("id"))
				{
					id = (String) attributes["id"];
					attributes.Remove("id");
				}
			}

			OperationState state = SetOperation.IterateOnDataSource(selectedValue, dataSource, attributes);

			writer.WriteBeginTag("select");
			writer.WriteAttribute("id", id);
			writer.WriteAttribute("name", name);
			writer.Write(" ");
			writer.Write(GetAttributes(attributes));
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			if (firstOption != null)
			{
				writer.WriteBeginTag("option");
				writer.WriteAttribute("value", (firstOptionValue == null) ? "0" : SafeHtmlEncode(firstOptionValue));
				writer.Write(HtmlTextWriter.TagRightChar);
				writer.Write(SafeHtmlEncode(firstOption));
				writer.WriteEndTag("option");
				writer.WriteLine();
			}

			foreach(SetItem item in state)
			{
				writer.WriteBeginTag("option");

				if (item.IsSelected)
				{
					writer.Write(" selected=\"selected\"");
				}

				writer.WriteAttribute("value", SafeHtmlEncode(item.Value));
				writer.Write(HtmlTextWriter.TagRightChar);
				writer.Write(SafeHtmlEncode(item.Text));
				writer.WriteEndTag("option");
				writer.WriteLine();
			}

			writer.WriteEndTag("select");

			return sbWriter.ToString();
		}

		#endregion

		#region Enum

		/// <summary>
		/// Creates a list of pairs for the enum type. 
		/// </summary>
		/// <param name="enumType">enum type.</param>
		/// <returns></returns>
		public static Pair<int, string>[] EnumToPairs(Type enumType)
		{
			if (enumType == null) throw new ArgumentNullException("enumType");
			if (!enumType.IsEnum) throw new ArgumentException("enumType must be an Enum", "enumType");

			Array values = Enum.GetValues(enumType);
			string[] names = Enum.GetNames(enumType);

			List<Pair<int, string>> listOfPairs = new List<Pair<int, string>>();
			int index = 0;

			foreach(string name in names)
			{
				int value = Convert.ToInt32(values.GetValue(index++));
				listOfPairs.Add(new Pair<int, string>(value, TextHelper.PascalCaseToWord(name)));
			}

			return listOfPairs.ToArray();
		}

		#endregion 

		#region Validation

		/// <summary>
		/// Configures this FormHelper instance to use the supplied
		/// web validator to generate field validation.
		/// </summary>
		/// <param name="provider">The validation provider.</param>
		public void UseWebValidatorProvider(IBrowserValidatorProvider provider)
		{
			if (provider == null) throw new ArgumentNullException("provider");

			validatorProvider = provider;
		}

		/// <summary>
		/// Configures this FormHelper instance to use Prototype for form fields validation
		/// </summary>
		public void UsePrototypeValidation()
		{
			UseWebValidatorProvider(new PrototypeWebValidator());
		}

		/// <summary>
		/// Configures this FormHelper instance to use fValidate for form fields validation
		/// </summary>
		public void UsefValidate()
		{
			UseWebValidatorProvider(new FValidateWebValidator());
		}

		/// <summary>
		/// Configures this FormHelper instance to use Zebda for form fields validation
		/// </summary>
		public void UseZebdaValidation()
		{
			UseWebValidatorProvider(new ZebdaWebValidator());
		}

		/// <summary>
		/// Disables the validation.
		/// </summary>
		public void DisableValidation()
		{
			isValidationDisabled = true;
		}

		/// <summary>
		/// Applies the validation.
		/// </summary>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="target">The target.</param>
		/// <param name="attributes">The attributes.</param>
		protected virtual void ApplyValidation(InputElementType inputType, string target, ref IDictionary attributes)
		{
			bool disableValidation = CommonUtils.ObtainEntryAndRemove(attributes, "disablevalidation", "false") == "true";

			if (!IsValidationEnabled && disableValidation)
			{
				return;
			}

			if (Controller.Validator == null || validationConfig == null)
			{
				return;
			}

			if (attributes == null)
			{
				attributes = new HybridDictionary(true);
			}

			IValidator[] validators = CollectValidators(RequestContext.All, target);

			IBrowserValidationGenerator generator = validatorProvider.CreateGenerator(validationConfig, inputType, attributes);

			foreach(IValidator validator in validators)
			{
				if (validator.SupportsBrowserValidation)
				{
					validator.ApplyBrowserValidation(validationConfig, inputType, generator, attributes, target);
				}
			}
		}

		private IValidator[] CollectValidators(RequestContext requestContext, string target)
		{
			List<IValidator> validators = new List<IValidator>();

			ObtainTargetProperty(requestContext, target, delegate(PropertyInfo property)
			{
				validators.AddRange(Controller.Validator.GetValidators(property.DeclaringType, property));
			});

			return validators.ToArray();
		}

		private bool IsValidationEnabled
		{
			get
			{
				if (isValidationDisabled) return false;

				if (objectStack.Count == 0) return true;

				return ((FormScopeInfo)objectStack.Peek()).IsValidationEnabled;
			}
		}

		#endregion

		#region protected members

		/// <summary>
		/// Rewrites the target if within object scope.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		protected string RewriteTargetIfWithinObjectScope(string target)
		{
			if (objectStack.Count == 0)
			{
				return target;
			}
			else
			{
				return ((FormScopeInfo) objectStack.Peek()).RootTarget + "." + target;
			}
		}

		/// <summary>
		/// Creates the specified input element 
		/// using the specified parameters to supply the name, value, id and others 
		/// html attributes.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		protected virtual string CreateInputElement(string type, string target, Object value, IDictionary attributes)
		{
			if (value == null)
			{
				value = CommonUtils.ObtainEntryAndRemove(attributes, "defaultValue");
			}

			string id = CreateHtmlId(attributes, target);

			return CreateInputElement(type, id, target, FormatIfNecessary(value, attributes), attributes);
		}

		/// <summary>
		/// Creates the specified input element 
		/// using the specified parameters to supply the name, value, id and others 
		/// html attributes.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <param name="value"></param>
		/// <param name="attributes">Attributes for the FormHelper method and for the html element it generates</param>
		/// <returns>The generated form element</returns>
		protected virtual string CreateInputElement(string type, string id, string target, string value, IDictionary attributes)
		{
			value = FormatIfNecessary(value, attributes);

			value = SafeHtmlEncode(value);

			if (attributes != null && attributes.Contains("mask"))
			{
				string mask = CommonUtils.ObtainEntryAndRemove(attributes, "mask");
				string maskSep = CommonUtils.ObtainEntryAndRemove(attributes, "mask_separator", "-");

				string onBlur = CommonUtils.ObtainEntryAndRemove(attributes, "onBlur", "void(0)");
				string onKeyUp = CommonUtils.ObtainEntryAndRemove(attributes, "onKeyUp", "void(0)");

				string js = "return monorail_formhelper_mask(event,this,'" + mask + "','" + maskSep + "');";

				attributes["onBlur"] = "javascript:" + onBlur + ";" + js;
				attributes["onKeyUp"] = "javascript:" + onKeyUp + ";" + js;
			}
			
			return String.Format("<input type=\"{0}\" id=\"{1}\" name=\"{2}\" value=\"{3}\" {4}/>", 
								 type, id, target, value, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates the input element.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		protected virtual string CreateInputElement(string type, string value, IDictionary attributes)
		{
			return String.Format("<input type=\"{0}\" value=\"{1}\" {2}/>",
								 type, FormatIfNecessary(value, attributes), GetAttributes(attributes));
		}

		/// <summary>
		/// Formats if necessary.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		protected static string FormatIfNecessary(object value, IDictionary attributes)
		{
			string formatString = CommonUtils.ObtainEntryAndRemove(attributes, "textformat");

			if (value != null && formatString != null)
			{
				IFormattable formattable = value as IFormattable;

				if (formattable != null)
				{
					value = formattable.ToString(formatString, null);
				}
			}
			else if (value == null)
			{
				value = String.Empty;
			}

			return value.ToString();
		}

		/// <summary>
		/// Obtains the target property.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		protected PropertyInfo ObtainTargetProperty(RequestContext context, string target, Action<PropertyInfo> action)
		{
			string[] pieces;

			Type root = ObtainRootType(context, target, out pieces);

			if (root != null && pieces.Length > 1)
			{
				return QueryPropertyInfoRecursive(root, pieces, 1, action);
			}

			return null;
		}

		/// <summary>
		/// Queries the context for the target value
		/// </summary>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainValue(string target)
		{
			return ObtainValue(RequestContext.All, target);
		}

		/// <summary>
		/// Queries the context for the target value
		/// </summary>
		/// <param name="context"></param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainValue(RequestContext context, string target)
		{
			string[] pieces;

			object rootInstance = ObtainRootInstance(context, target, out pieces);

			if (rootInstance != null && pieces.Length > 1)
			{
				return QueryPropertyRecursive(rootInstance, pieces, 1);
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the root instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The object to get the value from and to be based on to create the element name.</param>
		/// <returns>The generated form element</returns>
		protected object ObtainRootInstance(RequestContext context, string target)
		{
			object rootInstance = null;

			if (context == RequestContext.All || context == RequestContext.PropertyBag)
			{
				rootInstance = Controller.PropertyBag[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Flash))
			{
				rootInstance = Controller.Context.Flash[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Session))
			{
				rootInstance = Controller.Context.Session[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Params))
			{
				rootInstance = Controller.Params[target];
			}
			if (rootInstance == null && (context == RequestContext.All || context == RequestContext.Request))
			{
				rootInstance = Controller.Context.Items[target];
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the root instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="pieces">The pieces.</param>
		/// <returns></returns>
		protected object ObtainRootInstance(RequestContext context, string target, out string[] pieces)
		{
			pieces = target.Split(new char[] {'.'});

			string root = pieces[0];

			int index;

			bool isIndexed = CheckForExistenceAndExtractIndex(ref root, out index);

			object rootInstance = ObtainRootInstance(context, root);

			if (rootInstance == null)
			{
				return null;
			}

			if (isIndexed)
			{
				AssertIsValidArray(rootInstance, root, index);
			}
	
			if (!isIndexed && pieces.Length == 1)
			{
				return rootInstance;
			}
			else if (isIndexed)
			{
				rootInstance = GetArrayElement(rootInstance, index);
			}

			return rootInstance;
		}

		/// <summary>
		/// Obtains the type of the root.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <param name="pieces">The pieces.</param>
		/// <returns></returns>
		private Type ObtainRootType(RequestContext context, string target, out string[] pieces)
		{
			pieces = target.Split(new char[] { '.' });

			Type foundType = (Type) Controller.PropertyBag[pieces[0] + "type"];

			if (foundType == null)
			{
				object root = ObtainRootInstance(context, target, out pieces);

				if (root != null)
				{
					foundType = root.GetType();
				}
			}

			return foundType;
		}

		private static PropertyInfo QueryPropertyInfoRecursive(Type type, string[] propertyPath)
		{
			return QueryPropertyInfoRecursive(type, propertyPath, 0, null);
		}

		private static PropertyInfo QueryPropertyInfoRecursive(Type type, string[] propertyPath, int piece, Action<PropertyInfo> action)
		{
			string property = propertyPath[piece]; int index;

			bool isIndexed = CheckForExistenceAndExtractIndex(ref property, out index);

			PropertyInfo propertyInfo = type.GetProperty(property, ResolveFlagsToUse(type));

			if (propertyInfo == null)
			{
				if (logger.IsErrorEnabled)
				{
					logger.Error("No public property '{0}' found on type '{1}'", property, type.FullName);
				}

				return null;
			}

			if (!propertyInfo.CanRead)
			{
				throw new BindingException("Property '{0}' for type '{1}' can not be read",
					propertyInfo.Name, type.FullName);
			}

			if (propertyInfo.GetIndexParameters().Length != 0)
			{
				throw new BindingException("Property '{0}' for type '{1}' has indexes, which are not supported",
					propertyInfo.Name, type.FullName);
			}

			if (action != null)
			{
				action(propertyInfo);
			}

			type = propertyInfo.PropertyType;

			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return null;
			}

			if (isIndexed)
			{
				if (type.IsGenericType)
				{
					Type[] args = type.GetGenericArguments();
					if (args.Length != 1)
						throw new BindingException("Expected the generic indexed property '{0}' to be of 1 element", type.Name);
					type = args[0];
				}

				if (type.IsArray)
				{
					type = type.GetElementType();
				}
			}

			if (piece + 1 == propertyPath.Length)
			{
				return propertyInfo;
			}

			return QueryPropertyInfoRecursive(type, propertyPath, piece + 1, action);
		}

		/// <summary>
		/// Query property paths agains the rootInstance type
		/// </summary>
		/// <param name="rootInstance">the object to query</param>
		/// <param name="propertyPath">property path</param>
		/// <returns>The generated form element</returns>
		protected static object QueryPropertyRecursive(object rootInstance, string[] propertyPath)
		{
			return QueryPropertyRecursive(rootInstance, propertyPath, 0);
		}

		/// <summary>
		/// Query property paths agains the rootInstance type
		/// </summary>
		/// <param name="rootInstance">the object to query</param>
		/// <param name="propertyPath">property path</param>
		/// <param name="piece">start index</param>
		/// <returns>The generated form element</returns>
		protected static object QueryPropertyRecursive(object rootInstance, string[] propertyPath, int piece)
		{
			string property = propertyPath[piece]; int index;

			Type instanceType = rootInstance.GetType();

			bool isIndexed = CheckForExistenceAndExtractIndex(ref property, out index);

			PropertyInfo propertyInfo = instanceType.GetProperty(property, ResolveFlagsToUse(instanceType));

			object instance = null;

			if (propertyInfo == null)
			{
				FieldInfo fieldInfo = instanceType.GetField(property, FieldFlags);

				if (fieldInfo != null)
				{
					instance = fieldInfo.GetValue(rootInstance);
				}
			}
			else
			{
				if (!propertyInfo.CanRead)
				{
					throw new BindingException("Property '{0}' for type '{1}' can not be read", 
						propertyInfo.Name, instanceType.FullName);
				}
				
				if (propertyInfo.GetIndexParameters().Length != 0)
				{
					throw new BindingException("Property '{0}' for type '{1}' has indexes, which are not supported", 
						propertyInfo.Name, instanceType.FullName);
				}

				instance = propertyInfo.GetValue(rootInstance, null);
			}

			if (isIndexed && instance != null)
			{
				AssertIsValidArray(instance, property, index);

				instance = GetArrayElement(instance, index);
			}

			if (instance == null || piece + 1 == propertyPath.Length)
			{
				return instance;
			}

			return QueryPropertyRecursive(instance, propertyPath, piece + 1);
		}

		/// <summary>
		/// Creates the HTML id.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		/// <returns>The generated form element</returns>
		protected static string CreateHtmlId(IDictionary attributes, string target)
		{
			return CreateHtmlId(attributes, target, true);
		}
		
		/// <summary>
		/// Creates the HTML id.
		/// </summary>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		/// <param name="removeEntry">if set to <c>true</c> [remove entry].</param>
		/// <returns>The generated form element</returns>
		protected static string CreateHtmlId(IDictionary attributes, string target, bool removeEntry)
		{
			string id;
			
			if (removeEntry)
			{
				id = CommonUtils.ObtainEntryAndRemove(attributes, "id");
			}
			else
			{
				id = CommonUtils.ObtainEntry(attributes, "id");
			}

			if (id == null)
			{
				id = CreateHtmlId(target);
			}
			
			return id;
		}

		#endregion

		#region private helpers

		private static void ApplyNumberOnlyOptions(IDictionary attributes)
		{
			string list = CommonUtils.ObtainEntryAndRemove(attributes, "exceptions", String.Empty);
			string forbid = CommonUtils.ObtainEntryAndRemove(attributes, "forbid", String.Empty);

			attributes["onKeyPress"] = "return monorail_formhelper_numberonly(event, [" + list + "], [" + forbid + "]);";
		}

		private static void ApplyFilterOptions(IDictionary attributes)
		{
			string forbid = CommonUtils.ObtainEntryAndRemove(attributes, "forbid", String.Empty);

			attributes["onKeyPress"] = "return monorail_formhelper_inputfilter(event, [" + forbid + "]);";
		}

		private static void AssertIsValidArray(object instance, string property, int index)
		{
			Type instanceType = instance.GetType();

			IList list = instance as IList;

			bool validList = false;

			if (list == null && instanceType.IsGenericType)
			{
				Type[] genArgs = instanceType.GetGenericArguments();

				Type genList = typeof(System.Collections.Generic.IList<>).MakeGenericType(genArgs);
				Type genTypeDef = instanceType.GetGenericTypeDefinition().MakeGenericType(genArgs);

				validList = genList.IsAssignableFrom(genTypeDef);
			}
			
			if (!validList && list == null)
			{
				throw new RailsException("The property {0} is being accessed as " + 
					"an indexed property but does not seem to implement IList. " + 
					"In fact the type is {1}", property, instanceType.FullName);
			}

			if (index < 0)
			{
				throw new RailsException("The specified index '{0}' is outside the bounds " + 
					"of the array. Property {1}", index, property);
			}
		}

		private static object GetArrayElement(object instance, int index)
		{
			IList list = instance as IList;

			if (list == null && instance != null && instance.GetType().IsGenericType)
			{
				Type instanceType = instance.GetType();

				Type[] genArguments = instanceType.GetGenericArguments();

				Type genType = instanceType.GetGenericTypeDefinition().MakeGenericType(genArguments);
				
				// I'm not going to retest for IList implementation as 
				// if we got here, the AssertIsValidArray has run successfully

				PropertyInfo countPropInfo = genType.GetProperty("Count");

				int count = (int) countPropInfo.GetValue(instance, null);
				
				if (count == 0 || index + 1 > count)
				{
					return null;
				}

				PropertyInfo indexerPropInfo = genType.GetProperty("Item");

				return indexerPropInfo.GetValue(instance, new object[] { index });
			}
			
			if (list == null || list.Count == 0 || index + 1 > list.Count)
			{
				return null;
			}

			return list[index];
		}

		private static bool CheckForExistenceAndExtractIndex(ref string property, out int index)
		{
			bool isIndexed = property.IndexOf('[') != -1;

			index = -1;

			if (isIndexed)
			{
				int start = property.IndexOf('[') + 1;
				int len = property.IndexOf(']', start) - start;

				string indexStr = property.Substring(start, len);

				try
				{
					index = Convert.ToInt32(indexStr);
				}
				catch(Exception)
				{
					throw new RailsException("Could not convert (param {0}) index to Int32. Value is {1}", 
						property, indexStr);
				}

				property = property.Substring(0, start - 1);
			}

			return isIndexed;
		}

		private static bool AreEqual(object left, object right)
		{
			if (left == null || right == null) return false;

			if (left is string && right is String)
			{
				return String.Compare(left.ToString(), right.ToString()) == 0;
			}

			if (left.GetType() == right.GetType())
			{
				return right.Equals(left);
			}

			IConvertible convertible = left as IConvertible;

			if (convertible != null)
			{
				try
				{
					object newleft = convertible.ToType(right.GetType(), null);
					return (newleft.Equals(right));
				}
				catch(Exception)
				{
					// Do nothing
				}
			}

			return left.ToString().Equals(right.ToString());
		}

		private string SafeHtmlEncode(string content)
		{
			if (Controller.Context != null)
			{
				return HtmlEncode(content);
			}

			return content;
		}

		/// <summary>
		/// Determines whether the present value matches the value on 
		/// the initialSetValue (which can be a single value or a set)
		/// </summary>
		/// <param name="value">Value from the datasource</param>
		/// <param name="initialSetValue">Value from the initial selection set</param>
		/// <param name="propertyOnInitialSet">Optional. Property to obtain the value from</param>
		/// <param name="isMultiple"><c>true</c> if the initial selection is a set</param>
		/// <returns><c>true</c> if it's selected</returns>
		protected internal static bool IsPresent(object value, object initialSetValue, 
												 ValueGetter propertyOnInitialSet, bool isMultiple)
		{
			if (!isMultiple)
			{
				object valueToCompare = initialSetValue;
				
				if (propertyOnInitialSet != null)
				{
					// propertyOnInitialSet.GetValue(initialSetValue, null);
					valueToCompare = propertyOnInitialSet.GetValue(initialSetValue); 
				}
				
				return AreEqual(value, valueToCompare);
			}
			else
			{
				foreach(object item in (IEnumerable) initialSetValue)
				{
					object valueToCompare = item;

					if (propertyOnInitialSet != null)
					{
						// valueToCompare = propertyOnInitialSet.GetValue(item, null);
						valueToCompare = propertyOnInitialSet.GetValue(item); 
					}

					if (AreEqual(value, valueToCompare))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		private static void AddChecked(IDictionary attributes)
		{
			attributes["checked"] = "checked";
		}

		private static void RemoveChecked(IDictionary attributes)
		{
			attributes.Remove("checked");
		}

		private static string CreateHtmlId(string name)
		{
			StringBuilder sb = new StringBuilder(name.Length);

			bool canUseUnderline = false;

			foreach(char c in name.ToCharArray())
			{
				switch(c)
				{
					case '.':
					case '[':
					case ']':
						if (canUseUnderline)
						{
							sb.Append('_');
							canUseUnderline = false;
						}
						break;
					default:
						canUseUnderline = true;
						sb.Append(c);
						break;
				}
				
			}

			return sb.ToString();
		}

		/// <summary>
		/// Abstracts the approach to access values on objects. 
		/// </summary>
		public abstract class ValueGetter
		{
			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public abstract string Name { get; }

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public abstract object GetValue(object instance);
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// that uses reflection to access values
		/// </summary>
		public class ReflectionValueGetter : ValueGetter
		{
			private PropertyInfo propInfo;

			/// <summary>
			/// Initializes a new instance of the <see cref="ReflectionValueGetter"/> class.
			/// </summary>
			/// <param name="propInfo">The prop info.</param>
			public ReflectionValueGetter(PropertyInfo propInfo)
			{
				this.propInfo = propInfo;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return propInfo.Name; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				try
				{
					return propInfo.GetValue(instance, null);
				}
				catch(TargetException)
				{
					PropertyInfo tempProp = instance.GetType().GetProperty(Name);

					if (tempProp == null)
					{
						throw;
					}

					return tempProp.GetValue(instance, null);
				}
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// that uses reflection and recusion to access values
		/// </summary>
		public class RecursiveReflectionValueGetter : ValueGetter
		{
			private readonly string[] keyName;
			private readonly string name = string.Empty;

			/// <summary>
			/// Initializes a new instance of the <see cref="RecursiveReflectionValueGetter"/> class.
			/// </summary>
			/// <param name="targetType">The target type to query</param>
			/// <param name="keyName">the property path</param>
			public RecursiveReflectionValueGetter(Type targetType, string keyName)
			{
				this.keyName = keyName.Split('.');
				name = QueryPropertyInfoRecursive(targetType, this.keyName).Name;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return name; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				try
				{
					return QueryPropertyRecursive(instance, keyName);
				}
				catch (TargetException)
				{
					PropertyInfo tempProp = instance.GetType().GetProperty(Name);

					if (tempProp == null)
					{
						throw;
					}

					return tempProp.GetValue(instance, null);
				}
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access DataRow's value
		/// </summary>
		public class DataRowValueGetter : ValueGetter
		{
			private readonly string columnName;

			/// <summary>
			/// Initializes a new instance of the <see cref="DataRowValueGetter"/> class.
			/// </summary>
			/// <param name="columnName">Name of the column.</param>
			public DataRowValueGetter(string columnName)
			{
				this.columnName = columnName;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return columnName; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				DataRow row = (DataRow) instance;

				return row[columnName];
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access DataRowView's value
		/// </summary>
		public class DataRowViewValueGetter : ValueGetter
		{
			private readonly string columnName;

			/// <summary>
			/// Initializes a new instance of the <see cref="DataRowViewValueGetter"/> class.
			/// </summary>
			/// <param name="columnName">Name of the column.</param>
			public DataRowViewValueGetter(string columnName)
			{
				this.columnName = columnName;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return columnName; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				DataRowView row = (DataRowView)instance;

				return row[columnName];
			}
		}

		/// <summary>
		/// Empty implementation of a <see cref="ValueGetter"/>
		/// </summary>
		public class NoActionGetter : ValueGetter
		{
			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return string.Empty; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				return null;
			}
		}

		/// <summary>
		/// Implementation of <see cref="ValueGetter"/>
		/// to access enum fields
		/// </summary>
		public class EnumValueGetter : ValueGetter
		{
			private readonly Type enumType;

			/// <summary>
			/// Initializes a new instance of the <see cref="EnumValueGetter"/> class.
			/// </summary>
			/// <param name="enumType">Type of the enum.</param>
			public EnumValueGetter(Type enumType)
			{
				this.enumType = enumType;
			}

			/// <summary>
			/// Gets the name.
			/// </summary>
			/// <value>The name.</value>
			public override string Name
			{
				get { return string.Empty; }
			}

			/// <summary>
			/// Gets the value.
			/// </summary>
			/// <param name="instance">The instance.</param>
			/// <returns></returns>
			public override object GetValue(object instance)
			{
				return Enum.Format(enumType, Enum.Parse(enumType, Convert.ToString(instance)), "d");
			}
		}

		/// <summary>
		/// Abstract factory for <see cref="ValueGetter"/> implementations
		/// </summary>
		public class ValueGetterAbstractFactory
		{
			/// <summary>
			/// Creates the specified target type.
			/// </summary>
			/// <param name="targetType">Type of the target.</param>
			/// <param name="keyName">Name of the key.</param>
			/// <returns></returns>
			public static ValueGetter Create(Type targetType, string keyName)
			{
				if (targetType == null)
				{
					return new NoActionGetter();
				}
				else if (targetType == typeof(DataRow))
				{
					return new DataRowValueGetter(keyName);
				}
				else if (targetType == typeof(DataRowView))
				{
					return new DataRowViewValueGetter(keyName);
				}
				else if (targetType.IsEnum)
				{
					return new EnumValueGetter(targetType);
				}
				else
				{
					PropertyInfo info;

					// check for recusion
					if(keyName.Contains("."))
					{
						info = QueryPropertyInfoRecursive(targetType, keyName.Split('.'));
						
						if (info != null)
						{
							return new RecursiveReflectionValueGetter(targetType, keyName);
						}
					}
					else
					{
						info = targetType.GetProperty(keyName, ResolveFlagsToUse(targetType));
						if (info != null)
						{
							return new ReflectionValueGetter(info);
						}
					}

					return null;
				}
			}
		}

		#endregion

		#region FormScopeInfo

		class FormScopeInfo
		{
			private readonly string target;
			private readonly bool isValidationEnabled;

			/// <summary>
			/// Initializes a new instance of the <see cref="FormScopeInfo"/> class.
			/// </summary>
			/// <param name="target">The target.</param>
			/// <param name="isValidationEnabled">if set to <c>true</c> [is validation enabled].</param>
			public FormScopeInfo(string target, bool isValidationEnabled)
			{
				this.target = target;
				this.isValidationEnabled = isValidationEnabled;
			}

			/// <summary>
			/// Gets the root target.
			/// </summary>
			/// <value>The root target.</value>
			public string RootTarget
			{
				get { return target; }
			}

			/// <summary>
			/// Gets a value indicating whether this instance is validation enabled.
			/// </summary>
			/// <value>
			/// 	<c>true</c> if this instance is validation enabled; otherwise, <c>false</c>.
			/// </value>
			public bool IsValidationEnabled
			{
				get { return isValidationEnabled; }
			}
		}

		#endregion

		private static BindingFlags ResolveFlagsToUse(Type type)
		{
			if (type.Assembly.FullName.StartsWith("DynamicAssemblyProxyGen") || type.Assembly.FullName.StartsWith("DynamicProxyGenAssembly2"))
			{
				return PropertyFlags2;
			}

			return PropertyFlags;
		}
	}
} 
