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

namespace Castle.MonoRail.Framework.Helpers.ValidationStrategy
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;

	using Castle.Components.Validator;
	using System.Collections;
	using Castle.MonoRail.Framework.Internal;

	#endregion Using Directives


	/// <summary>
	/// Implementation of a browser validator that uses the <c>JQuery.validate</c> 
	/// plugin for <c>JQuery</c>, which can be found here : 
	/// <c>http://plugins.jquery.com/project/validate</c>
	/// </summary>
	public class JQueryValidator : IBrowserValidatorProvider
	{

		#region IBrowserValidatorProvider Members

		/// <summary>
		/// Implementors should attempt to read their specific configuration
		/// from the <paramref name="parameters"/>, configure and return
		/// a class that extends from <see cref="BrowserValidationConfiguration"/>
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns>
		/// An instance that extends from <see cref="BrowserValidationConfiguration"/>
		/// </returns>
		public BrowserValidationConfiguration CreateConfiguration( IDictionary parameters )
		{
			JQueryConfiguration config = new JQueryConfiguration();
			config.Configure( parameters );
			return config;
		}

		/// <summary>
		/// Implementors should return their generator instance.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="inputType"></param>
		/// <param name="attributes"></param>
		/// <returns>A generator instance</returns>
		public IBrowserValidationGenerator CreateGenerator( BrowserValidationConfiguration configuration, InputElementType inputType, IDictionary attributes )
		{
			return new JQueryValidationGenerator( ( JQueryConfiguration )configuration, inputType, attributes );
		}

		#endregion IBrowserValidatorProvider Members

		#region Configuration

		/// <summary>
		/// The <see cref="BrowserValidationConfiguration"/> implementation for the JQuery validate plugin.
		/// </summary>
		public class JQueryConfiguration : BrowserValidationConfiguration
		{

			#region Instance Variables

			readonly Dictionary<String, CustomRule> _customRules = new Dictionary<String, CustomRule>();
			readonly Dictionary<string, string> _rules = new Dictionary<string, string>();
			readonly IDictionary _options = new Hashtable();
			readonly Dictionary<string, Group> _groups = new Dictionary<string, Group>();
			readonly IDictionary<string, IDictionary> _customClasses = new Dictionary<string, IDictionary>();

			#endregion Instance Variables

			#region Public Methods

			/// <summary>
			/// Adds a custom rule.
			/// </summary>
			/// <param name="name">The name of the rule.</param>
			/// <param name="violationMessage">The violation message.</param>
			/// <param name="rule">
			/// The rule: must be an anonymous method declaration or the validation 
			/// function reference (the function's name without parenthesis).
			/// </param>
			/// <example>
			/// This example adds a custom rule by passing an anonymous method:
			/// <code>
			/// AddCustomRule( 
			///		"maxWords", 
			///		"Please enter {0} words or less.", 
			///		"function(value, element, params) { return this.optional(element) || value.match(/\b\w+\b/g).length &lt; params; }" );
			/// </code>
			/// This example adds a custom rule by passing a method reference, assuming that a 
			/// referenced javascript file contains the <c>validateMaxWords</c> function.
			/// <code>
			/// AddCustomRule( 
			///		"maxWords", 
			///		"Please enter {0} words or less.", 
			///		"validateMaxWords" );
			/// </code>
			/// </example>
			public void AddCustomRule( string name, string violationMessage, string rule )
			{
				_customRules[ name ] = new CustomRule( name, rule, violationMessage );
			}

			/// <summary>
			/// Adds a custom class.
			/// </summary>
			/// <param name="name">The name of the class.</param>
			/// <param name="options">The class options</param>
			public void AddCustomClass( string name, IDictionary options )
			{
				_customClasses[ name ] = options;
			}

			/// <summary>
			/// Configures the JQuery Validate plugin based on the supplied parameters.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			public override void Configure( IDictionary parameters )
			{
				AddParameterToOptions( parameters, JQueryOptions.Debug, false );
				AddParameterToOptions( parameters, JQueryOptions.ErrorClass, true );
				AddParameterToOptions( parameters, JQueryOptions.ErrorContainer, true );
				AddParameterToOptions( parameters, JQueryOptions.ErrorElement, true );
				AddParameterToOptions( parameters, JQueryOptions.ErrorLabelContainer, true );
				AddParameterToOptions( parameters, JQueryOptions.ErrorPlacement, false );
				AddParameterToOptions( parameters, JQueryOptions.FocusCleanup, false );
				AddParameterToOptions( parameters, JQueryOptions.FocusInvalid, false );
				AddParameterToOptions( parameters, JQueryOptions.Highlight, false );
				AddParameterToOptions( parameters, JQueryOptions.Ignore, true );
				AddParameterToOptions( parameters, JQueryOptions.Messages, false );
				AddParameterToOptions( parameters, JQueryOptions.Meta, true );
				AddParameterToOptions( parameters, JQueryOptions.OnClick, false );
				AddParameterToOptions( parameters, JQueryOptions.OnFocusOut, false );
				AddParameterToOptions( parameters, JQueryOptions.OnKeyUp, false );
				AddParameterToOptions( parameters, JQueryOptions.OnSubmit, false );
				AddParameterToOptions( parameters, JQueryOptions.ShowErrors, false );
				AddParameterToOptions( parameters, JQueryOptions.SubmitHandler, false );
				AddParameterToOptions( parameters, JQueryOptions.Success, false );
				AddParameterToOptions( parameters, JQueryOptions.Unhighlight, false );
				AddParameterToOptions( parameters, JQueryOptions.Wrapper, true );
				AddParameterToOptions( parameters, JQueryOptions.IsAjax, false );

				AddCustomRules();
			}

			/// <summary>
			/// Implementors should return any tag/js content
			/// to be rendered after the form tag is closed.
			/// </summary>
			/// <param name="formId">The form id.</param>
			/// <returns></returns>
			public override string CreateBeforeFormClosed( string formId )
			{
				StringBuilder stringBuilder = new StringBuilder();

				if ( _rules.Count > 0 )
				{
					_options[ JQueryOptions.Rules ] = AbstractHelper.JavascriptOptions( _rules );
				}

				bool isAjax = false;
				bool.TryParse( CommonUtils.ObtainEntryAndRemove( _options, JQueryOptions.IsAjax, bool.FalseString ), out isAjax );

				if( isAjax )
				{
					string submitHandler = CommonUtils.ObtainEntryAndRemove( _options, JQueryOptions.SubmitHandler );

					if( submitHandler == null )
					{
						_options.Add( JQueryOptions.SubmitHandler, "function( form ) { jQuery( form ).ajaxSubmit(); }" );
					}
				}

				MergeGroupDefinitionsWithOptions();
				GenerateGroupNotEmptyValidatorCustomRule();
				GenerateGroupNotEmptyValidatorCustomClass();

				stringBuilder.AppendFormat("jQuery(\"#{0}\").validate( {1} );", formId, AbstractHelper.JavascriptOptions(_options));

				if ( _customRules.Count > 0 )
				{
					foreach ( CustomRule rule in _customRules.Values )
					{
						stringBuilder.Append( Environment.NewLine );
						stringBuilder.Append( "jQuery.validator.addMethod('" );
						stringBuilder.Append( rule.Name );
						stringBuilder.Append( "', " );
						stringBuilder.Append( rule.Rule );
						stringBuilder.Append( ", '" );
						stringBuilder.Append( rule.ViolationMessage );
						stringBuilder.Append( "' );" );
					}
				}

				if(_customClasses.Count > 0)
				{
					foreach(KeyValuePair<string, IDictionary> pair in _customClasses)
					{
						stringBuilder.Append(Environment.NewLine);
						stringBuilder.Append("jQuery.validator.addClassRules({");
						stringBuilder.Append(string.Format("required{0}",pair.Key));
						stringBuilder.Append(": ");
						stringBuilder.Append(AbstractHelper.JavascriptOptions(pair.Value));
						stringBuilder.Append("});");
					}
				}

				return AbstractHelper.ScriptBlock( stringBuilder.ToString() );
			}

			/// <summary>
			/// Generates the custom group not empty validator rule.
			/// </summary>
			private void GenerateGroupNotEmptyValidatorCustomRule()
			{
				foreach(KeyValuePair<string, Group> pair in _groups)
				{
					Group group = pair.Value;
					AddCustomRule(group.GroupName, group.ViolationMessage, group.GetCustomRuleFunction());
				}
			}

			/// <summary>
			/// Merges the group definitions with options.
			/// </summary>
			private void MergeGroupDefinitionsWithOptions()
			{
				string groupValues = string.Empty;
				foreach(KeyValuePair<string, Group> pair in _groups)
				{
					Group group = pair.Value;
					groupValues += string.Format("{0},{1}", group.GetFormattedGroup(), Environment.NewLine);
				}

				if(groupValues != string.Empty)
					_options.Add("groups", string.Format("{{{0}}}",groupValues));
			}

			/// <summary>
			/// Generates the custom group not empty validator class rule.
			/// </summary>
			private void GenerateGroupNotEmptyValidatorCustomClass()
			{
				foreach(KeyValuePair<string, Group> pair in _groups)
				{
					Group group = pair.Value;

					Hashtable options = new Hashtable();
					options.Add(group.GroupName,"true");

					AddCustomClass(group.GroupName, options);
				}
			}

			#endregion Public Methods

			#region Internal Methods

			internal void AddRule( string target, string rule )
			{
				if ( _rules.ContainsKey( target ) )
				{
					string originalRule = _rules[ target ];

					if ( originalRule.StartsWith( "{" ) )
						originalRule = originalRule.Substring( 1, originalRule.LastIndexOf( "}" ) );

					_rules[ target ] = string.Concat( "{ ", originalRule, ", ", rule, " }" );
				}
				else
				{
					_rules.Add( target, string.Concat( "{ ", rule, " }" ) );
				}
			}

			internal void AddValidateNotEmptyGroupItem(string groupName, string violationMessage, string target)
			{
				if (_groups.ContainsKey(groupName) == false)
				{
					Group group = new Group(groupName, violationMessage);
					_groups.Add(groupName, group);
				}
				_groups[groupName].GroupItems.Add(target);
			}

			#endregion Internal Methods

			#region Private Methods

			void AddCustomRules()
			{
				AddCustomRule( "notEqualTo", "Must not be equal to {0}.", "function(value, element, param) { return value != jQuery(param).val(); }" );
				AddCustomRule( "greaterThan", "Must be greater than {0}.", "function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value > jQuery(param).val() ); }" );
				AddCustomRule( "lesserThan", "Must be lesser than {0}.", "function(value, element, param) { return ( IsNaN( value ) && IsNaN( jQuery(param).val() ) ) || ( value < jQuery(param).val() ); }" );
			}

			void AddParameterToOptions( IDictionary parameters, string parameterName, bool quote )
			{
				string parameterValue = CommonUtils.ObtainEntryAndRemove( parameters, parameterName, null );

				if( parameterValue != null )
				{
					if( quote )
						if( !parameterValue.StartsWith( "'" ) && !parameterValue.StartsWith( "\"" ) )
							_options.Add( parameterName, AbstractHelper.SQuote( parameterValue ) );
						else
							_options.Add( parameterName, parameterValue );
					else
						_options.Add( parameterName, parameterValue );
				}
			}

			void AddParameterToOptions( IDictionary parameters, string parameterName, bool quote, string defaultValue )
			{
				string parameterValue = CommonUtils.ObtainEntryAndRemove( parameters, parameterName, defaultValue );

				if( parameterValue != null )
				{
					if( quote )
						if( !parameterValue.StartsWith( "'" ) && !parameterValue.StartsWith( "\"" ) )
							_options.Add( parameterName, AbstractHelper.SQuote( parameterValue ) );
						else
							_options.Add( parameterName, parameterValue );
					else
						_options.Add( parameterName, parameterValue );
				}
			}

			#endregion Private Methods

			#region Nested classes

			/// <summary>
			/// Group definition.
			/// </summary>
			class Group
			{
				private readonly string _groupName;
				private readonly string _violationMessage;
				private readonly List<string> _groupItems = new List<string>();

				/// <summary>
				/// Initializes a new instance of the <see cref="Group"/> class.
				/// </summary>
				/// <param name="groupName">Name of the group.</param>
				/// <param name="violationMessage">The violation message.</param>
				public Group(string groupName, string violationMessage)
				{
					_groupName = groupName;
					_violationMessage = violationMessage;
				}

				/// <summary>
				/// Gets the name of the group.
				/// </summary>
				/// <value>The name of the group.</value>
				public string GroupName
				{
					get { return _groupName; }
				}

				/// <summary>
				/// Gets the violation message.
				/// </summary>
				/// <value>The violation message.</value>
				public string ViolationMessage
				{
					get { return _violationMessage; }
				}

				/// <summary>
				/// Gets the group items.
				/// </summary>
				/// <value>The group items.</value>
				public List<string> GroupItems
				{
					get { return _groupItems; }
				}

				/// <summary>
				/// Gets the group items.
				/// </summary>
				/// <returns></returns>
				public string GetFormattedGroupItems()
				{
					StringBuilder build = new StringBuilder();
					foreach(string groupItem in _groupItems)
					{
						build.AppendFormat("{0} ", groupItem.Replace('_','.'));
					}
					return build.ToString();
				}

				/// <summary>
				/// Gets the group items.
				/// </summary>
				/// <returns></returns>
				public string GetFormattedGroup()
				{
					return string.Format("{0}: \"{1}\"", _groupName, GetFormattedGroupItems());
				}

				/// <summary>
				/// Gets the custom rule.
				/// </summary>
				/// <returns></returns>
				public string GetCustomRuleFunction()
				{
					StringBuilder builder = new StringBuilder();

					builder.Append(" function() { if(");
					for(int groupItem = 0; groupItem < _groupItems.Count; groupItem++)
					{
						string target = _groupItems[groupItem];

						builder.Append(string.Format("$(\"#{0}\").val()!=''", target));

						if(groupItem < _groupItems.Count - 1)
						{
							builder.Append(" || ");
						}
					}
					builder.Append(") { return true } else { return false; } }");

					return builder.ToString();
				}
			}

			class CustomRule
			{

				#region Instance Variables

				public readonly string Name;
				public readonly string Rule;
				public readonly string ViolationMessage;

				#endregion Instance Variables

				#region Constructors

				public CustomRule( string name, string rule, string violationMessage )
				{
					Name = name;
					Rule = rule;
					ViolationMessage = violationMessage;
				}

				#endregion Constructors

			}

			static class JQueryOptions
			{

				public const string Debug = "debug";
				public const string ErrorClass = "errorClass";
				public const string ErrorContainer = "errorContainer";
				public const string ErrorElement = "errorElement";
				public const string ErrorLabelContainer = "errorLabelContainer";
				public const string ErrorPlacement = "errorPlacement";
				public const string FocusCleanup = "focusCleanup";
				public const string FocusInvalid = "focusInvalid";
				public const string Highlight = "highlight";
				public const string Ignore = "ignore";
				public const string Messages = "messages";
				public const string Meta = "meta";
				public const string OnClick = "onclick";
				public const string OnFocusOut = "onfocusout";
				public const string OnKeyUp = "onkeyup";
				public const string OnSubmit = "onsubmit";
				public const string Rules = "rules";
				public const string ShowErrors = "showErrors";
				public const string SubmitHandler = "submitHandler";
				public const string Success = "success";
				public const string Unhighlight = "unhighlight";
				public const string Wrapper = "wrapper";
				public const string IsAjax = "isAjax";

			}

			#endregion Nested classes

		}

		#endregion Configuration

		#region Generator

		/// <summary>
		/// The <see cref="IBrowserValidationGenerator"/> implementation for the JQuery validate plugin.
		/// </summary>
		public class JQueryValidationGenerator : IBrowserValidationGenerator
		{
			#region Instance Variables

			readonly IDictionary _Attributes;
			readonly InputElementType _InputElementType;
			readonly JQueryConfiguration _configuration;

			#endregion Instance Variables

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="JQueryValidationGenerator"/> class.
			/// </summary>
			/// <param name="configuration">The configuration.</param>
			/// <param name="inputElementType">Type of the input element.</param>
			/// <param name="attributes">The attributes.</param>
			public JQueryValidationGenerator( JQueryConfiguration configuration, InputElementType inputElementType, IDictionary attributes )
			{
				_InputElementType = inputElementType;
				_Attributes = attributes;
				_configuration = configuration;
			}

			#endregion Constructors

			#region Private Methods

			static string GetPrefixedFieldld( string target, string field )
			{
				string[] parts = target.Split( '_' );

				return string.Join( "_", parts, 0, parts.Length - 1 ) + "_" + field;
			}			

			static string GetPrefixJQuerySelector(string target)
			{
				if (target.StartsWith("#")) return target;
				return string.Format("#{0}", target);
			}

			void AddClass( string className )
			{
				string existingClass = ( string )_Attributes[ "class" ];

				if ( existingClass != null )
				{
					_Attributes[ "class" ] = existingClass + " " + className;
				}
				else
				{
					_Attributes[ "class" ] = className;
				}
			}

			void AddTitle( string message )
			{
				if ( !string.IsNullOrEmpty( message ) )
				{
					string existingTitle = ( string )_Attributes[ "title" ];

					if ( !message.EndsWith( "." ) )
					{
						message += ".";
					}

					if ( existingTitle != null )
					{
						_Attributes[ "title" ] = existingTitle + " " + message;
					}
					else
					{
						_Attributes[ "title" ] = message;
					}
				}
			}

			void AddParameter( string parameterName, object parameterValue )
			{
				_Attributes[ parameterName ] = parameterValue;
			}

			#endregion Private Methods

			#region IBrowserValidationGenerator Members

			/// <summary>
			/// Set that a field should only accept digits.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetDigitsOnly( string target, string violationMessage )
			{
				AddClass( "digits" );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Set that a field should only accept numbers.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetNumberOnly( string target, string violationMessage )
			{
				AddClass( "number" );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that a field is required.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsRequired( string target, string violationMessage )
			{
				AddClass( "required" );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that a field value must match the specified regular expression.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="regExp">The reg exp.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetRegExp( string target, string regExp, string violationMessage )
			{
			}

			/// <summary>
			/// Sets that a field value must be a valid email address.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetEmail( string target, string violationMessage )
			{
				AddClass( "email" );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			public void SetExactLength( string target, int length )
			{
				_configuration.AddRule( target, string.Format( "rangelength: [{0}, {0}]", length ) );
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetExactLength( string target, int length, string violationMessage )
			{
				_configuration.AddRule( target, string.Format( "rangelength: [{0}, {0}]", length ) );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must have an minimum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			public void SetMinLength( string target, int minLength )
			{
				AddClass( "minlength" );
				AddParameter( "minlength", minLength );
			}

			/// <summary>
			/// Sets that field must have an minimum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetMinLength( string target, int minLength, string violationMessage )
			{
				AddClass( "minlength" );
				AddParameter( "minlength", minLength );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must have an maximum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="maxLength">The maximum length.</param>
			public void SetMaxLength( string target, int maxLength )
			{
				AddClass( "maxlength" );
				AddParameter( "maxlength", maxLength );
			}

			/// <summary>
			/// Sets that field must have an maximum lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="maxLength">The maximum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetMaxLength( string target, int maxLength, string violationMessage )
			{
				AddClass( "maxlength" );
				AddParameter( "maxlength", maxLength );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must be between a length range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="maxLength">The maximum length.</param>
			public void SetLengthRange( string target, int minLength, int maxLength )
			{
				_configuration.AddRule( target, string.Format( "rangelength: [{0}, {1}]", minLength, maxLength ) );
			}

			/// <summary>
			/// Sets that field must be between a length range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minLength">The minimum length.</param>
			/// <param name="maxLength">The maximum length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetLengthRange( string target, int minLength, int maxLength, string violationMessage )
			{
				_configuration.AddRule( target, string.Format( "rangelength: [{0}, {1}]", minLength, maxLength ) );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange( string target, int minValue, int maxValue, string violationMessage )
			{
				_configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange( string target, decimal minValue, decimal maxValue, string violationMessage )
			{
				_configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <remarks>This is not yet implemented in the JQuery validate plugin. It should be in next version: 1.2.3</remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value.</param>
			/// <param name="maxValue">Maximum value.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange( string target, DateTime minValue, DateTime maxValue, string violationMessage )
			{
			}

			/// <summary>
			/// Sets that field must be between a value range.
			/// </summary>
			/// <remarks>Note that the string values must only contains numbers.</remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="minValue">Minimum value: a number as a string.</param>
			/// <param name="maxValue">Maximum value: a number as a string.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetValueRange( string target, string minValue, string maxValue, string violationMessage )
			{
				_configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Set that a field value must be the same as another field's value.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsSameAs( string target, string comparisonFieldName, string violationMessage )
			{
				string prefixedComparisonFieldName = GetPrefixJQuerySelector(GetPrefixedFieldld(target, comparisonFieldName));
				AddClass( "equalTo" );
				AddParameter("equalTo", prefixedComparisonFieldName);
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Set that a field value must _not_ be the same as another field's value.
			/// </summary>
			/// <remarks>Not implemented by the JQuery validate plugin.</remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsNotSameAs( string target, string comparisonFieldName, string violationMessage )
			{
			}

			/// <summary>
			/// Set that a field value must be a valid date.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetDate( string target, string violationMessage )
			{
				AddClass( "date" );
				AddTitle( violationMessage );
			}

			/// <summary>
			/// Sets that a field's value must greater than another field's value.
			/// </summary>
			/// <remarks>
			/// Only numeric values can be compared for now. The JQuery validation plugin does not yet support dates comparison.
			/// </remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="validationType">The type of data to compare.</param>
			/// <param name="violationMessage">The violation message.</param>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			public void SetAsGreaterThan( string target, string comparisonFieldName, IsGreaterValidationType validationType, string violationMessage )
			{
				if( validationType == IsGreaterValidationType.Decimal || validationType == IsGreaterValidationType.Integer )
				{
					string prefixedComparisonFieldName = GetPrefixJQuerySelector(GetPrefixedFieldld(target, comparisonFieldName));

					AddClass( "greaterThan" );
					AddParameter("greaterThan", prefixedComparisonFieldName);
					AddTitle( violationMessage );
				}
			}

			/// <summary>
			/// Sets that a field's value must be lesser than another field's value.
			/// </summary>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="comparisonFieldName">The name of the field to compare with.</param>
			/// <param name="validationType">The type of data to compare.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsLesserThan( string target, string comparisonFieldName, IsGreaterValidationType validationType, string violationMessage )
			{
				if( validationType == IsGreaterValidationType.Decimal || validationType == IsGreaterValidationType.Integer )
				{
					string prefixedComparisonFieldName = GetPrefixJQuerySelector(GetPrefixedFieldld(target, comparisonFieldName));

					AddClass( "lesserThan" );
					AddParameter("lesserThan", prefixedComparisonFieldName);
					AddTitle( violationMessage );
				}
			}

			/// <summary>
			/// Sets that a flied is part of a group validation.
			/// </summary>
			/// <remarks>Not implemented by the JQuery validate plugin. Done via a custom rule.</remarks>
			/// <param name="target">The target.</param>
			/// <param name="groupName">Name of the group.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetAsGroupValidation(string target, string groupName, string violationMessage)
			{
				_configuration.AddValidateNotEmptyGroupItem(groupName, violationMessage, target);

				AddClass(string.Format("required{0}",groupName));
			}

			#endregion IBrowserValidationGenerator Members

		}

		#endregion Generator

	}

}
