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

			Dictionary<String, CustomRule> _CustomRules = new Dictionary<String, CustomRule>();
			Dictionary<string, string> _Rules = new Dictionary<string, string>();
			IDictionary _Options = new Hashtable();

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
				_CustomRules[ name ] = new CustomRule( name, rule, violationMessage );
			}

			/// <summary>
			/// Configures the JQuery Validate plugin based on the supplied parameters.
			/// </summary>
			/// <param name="parameters">The parameters.</param>
			public override void Configure( IDictionary parameters )
			{
				AddParameterToOptions( parameters, JQueryOptions.Debug );
				AddParameterToOptions( parameters, JQueryOptions.ErrorClass );
				AddParameterToOptions( parameters, JQueryOptions.ErrorContainer );
				AddParameterToOptions( parameters, JQueryOptions.ErrorElement );
				AddParameterToOptions( parameters, JQueryOptions.ErrorLabelContainer );
				AddParameterToOptions( parameters, JQueryOptions.ErrorPlacement );
				AddParameterToOptions( parameters, JQueryOptions.FocusCleanup );
				AddParameterToOptions( parameters, JQueryOptions.FocusInvalid );
				AddParameterToOptions( parameters, JQueryOptions.Highlight );
				AddParameterToOptions( parameters, JQueryOptions.Ignore );
				AddParameterToOptions( parameters, JQueryOptions.Messages );
				AddParameterToOptions( parameters, JQueryOptions.Meta );
				AddParameterToOptions( parameters, JQueryOptions.OnClick );
				AddParameterToOptions( parameters, JQueryOptions.OnFocusOut );
				AddParameterToOptions( parameters, JQueryOptions.OnKeyUp );
				AddParameterToOptions( parameters, JQueryOptions.OnSubmit );
				AddParameterToOptions( parameters, JQueryOptions.ShowErrors );
				AddParameterToOptions( parameters, JQueryOptions.SubmitHandler );
				AddParameterToOptions( parameters, JQueryOptions.Success );
				AddParameterToOptions( parameters, JQueryOptions.Unhighlight );
				AddParameterToOptions( parameters, JQueryOptions.Wrapper );
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

				if ( _Rules.Count > 0 )
				{
					_Options[ JQueryOptions.Rules ] = AjaxHelper.JavascriptOptions( _Rules );
				}

				stringBuilder.AppendFormat( "$(\"#{0}\").validate( {1} );", formId, AjaxHelper.JavascriptOptions( _Options ) );

				if ( _CustomRules.Count > 0 )
				{
					foreach ( CustomRule rule in _CustomRules.Values )
					{
						stringBuilder.Append( Environment.NewLine );
						stringBuilder.Append( "jQuery.validator.addMethod('" );
						stringBuilder.Append( rule.Name );
						stringBuilder.Append( "', " );
						stringBuilder.Append( rule.Rule );
						stringBuilder.Append( "', '" );
						stringBuilder.Append( rule.ViolationMessage );
						stringBuilder.Append( "' );" );
					}
				}

				return AbstractHelper.ScriptBlock( stringBuilder.ToString() );
			}

			#endregion Public Methods

			#region Internal Methods

			internal void AddRule( string target, string rule )
			{
				if ( _Rules.ContainsKey( target ) )
				{
					string originalRule = _Rules[ target ];

					if ( originalRule.StartsWith( "{" ) )
						originalRule = originalRule.Substring( 1, originalRule.LastIndexOf( "}" ) );

					_Rules[ target ] = string.Concat( "{ ", originalRule, ", ", rule, " }" );
				}
				else
				{
					_Rules.Add( target, string.Concat( "{ ", rule, " }" ) );
				}
			}

			#endregion Internal Methods

			#region Private Methods

			void AddParameterToOptions( IDictionary parameters, string parameterName )
			{
				string parameterValue = CommonUtils.ObtainEntryAndRemove( parameters, parameterName, null );

				if ( parameterValue != null )
					_Options.Add( parameterName, parameterValue );
			}

			void AddParameterToOptions( IDictionary parameters, string parameterName, string defaultValue )
			{
				string parameterValue = CommonUtils.ObtainEntryAndRemove( parameters, parameterName, defaultValue );

				if ( parameterValue != null )
					_Options.Add( parameterName, parameterValue );
			}

			#endregion Private Methods

			#region Nested classes

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
			readonly JQueryConfiguration _Configuration;

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
				_Configuration = configuration;
			}

			#endregion Constructors

			#region Private Methods

			static string GetPrefixedFieldld( string target, string field )
			{
				string[] parts = target.Split( '.' );

				return string.Join( "_", parts, 0, parts.Length - 1 ) + "_" + field;
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
				_Configuration.AddRule( target, string.Format( "rangelength: [{0}, {0}]", length ) );
			}

			/// <summary>
			/// Sets that field must have an exact lenght.
			/// </summary>
			/// <param name="target">The target name (ie, a hint about the controller being validated)</param>
			/// <param name="length">The length.</param>
			/// <param name="violationMessage">The violation message.</param>
			public void SetExactLength( string target, int length, string violationMessage )
			{
				_Configuration.AddRule( target, string.Format( "rangelength: [{0}, {0}]", length ) );
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
				_Configuration.AddRule( target, string.Format( "rangelength: [{0}, {1}]", minLength, maxLength ) );
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
				_Configuration.AddRule( target, string.Format( "rangelength: [{0}, {1}]", minLength, maxLength ) );
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
				_Configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
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
				_Configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
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
				_Configuration.AddRule( target, string.Format( "range: [{0}, {1}]", minValue, maxValue ) );
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
				AddClass( "equalTo" );
				AddParameter( "equalTo", comparisonFieldName );
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

			#endregion IBrowserValidationGenerator Members

		}

		#endregion Generator

	}

}
