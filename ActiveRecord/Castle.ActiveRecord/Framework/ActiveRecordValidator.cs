// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Validator;

	/// <summary>
	/// Provides the implementation of <see cref="IValidationProvider"/>. 
	/// An AR clas that wants to implement <see cref="IValidationProvider"/> 
	/// can use an instance of this class to delegate the validation methods and properties
	/// </summary>
	/// <remarks>
	/// This class contains the validation logic that was previously duplicated in 
	/// ActiveRecordValidationBase and ActiveRecordValidationBase&lt;T&gt;
	/// </remarks>
	public class ActiveRecordValidator : IValidationProvider
	{
		private ValidatorRunner _runner;

		private Dictionary<PropertyInfo, ArrayList> failedProperties;

		private readonly object _arObjectInstance;

		/// <summary>
		/// Constructs an ActiveRecordValidator
		/// </summary>
		public ActiveRecordValidator(object arObjectInstance)
		{
			_arObjectInstance = arObjectInstance;
		}

		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsValid()
		{
			return IsValid(RunWhen.Everytime);
		}

		/// <summary>
		/// Performs the fields validation for the specified action.
		/// </summary>
		/// <param name="runWhen">Use validators appropriate to the action being performed.</param>
		/// <returns>True if no validation error was found</returns>
		public virtual bool IsValid(RunWhen runWhen)
		{
			failedProperties = new Dictionary<PropertyInfo, ArrayList>();

			// first check if the object itself is valid
			bool returnValue = Runner.IsValid(ARObjectInstance, runWhen);

			// then check the components that are properties if the object
			foreach (PropertyInfo propinfo in GetNestedPropertiesToValidate(ARObjectInstance))
			{
				object propval = propinfo.GetValue(ARObjectInstance, null);

				if (propval != null)
				{
					bool propValid = Runner.IsValid(propval, runWhen);
					if (!propValid)
					{
						ErrorSummary propSummary = Runner.GetErrorSummary(propval);
						string[] propErrors = propSummary.GetErrorsForProperty(propinfo.Name);
						failedProperties.Add(propinfo, new ArrayList(propErrors));
					}
					returnValue &= propValid;
				}
			}

			if (!returnValue)
			{
				Type type = ARObjectInstance.GetType();
				ErrorSummary summary = Runner.GetErrorSummary(ARObjectInstance);

				foreach (string property in summary.InvalidProperties)
				{
					string[] errors = summary.GetErrorsForProperty(property);
					failedProperties.Add(type.GetProperty(property), new ArrayList(errors));
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Returns a list of current validation errors messages.
		/// </summary>
		public virtual String[] ValidationErrorMessages
		{
			get
			{
				if (Runner.GetErrorSummary(ARObjectInstance) == null)
				{
					IsValid();
				}

				List<string> errorMessages = new List<string>(Runner.GetErrorSummary(ARObjectInstance).ErrorMessages);

				AddNestedPropertyValidationErrorMessages(errorMessages, ARObjectInstance, Runner);

				return errorMessages.ToArray();
			}
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of
		/// error messages. Useful for frameworks.
		/// </summary>
		public virtual IDictionary PropertiesValidationErrorMessages
		{
			get { return failedProperties; }
		}

		/// <summary>
		/// Gets the <see cref="ValidatorRunner"/> to actually perform the validation.
		/// </summary>
		/// <value>The runner.</value>
		protected virtual ValidatorRunner Runner
		{
			get
			{
				if (_runner == null)
				{
					_runner = new ValidatorRunner(ActiveRecordModelBuilder.ValidatorRegistry);
				}
				return _runner;
			}
		}

		/// <summary>
		/// Gets the AR object instance that 
		/// uses this <see cref="ActiveRecordValidator"/> to do the validation.
		/// </summary>
		/// <value>The AR object instance.</value>
		protected virtual object ARObjectInstance
		{
			get { return _arObjectInstance; }
		}

		/// <summary>
		/// Throws an exception explaining why the save or update
		/// cannot be executed when fields are not ok to pass.
		/// </summary>
		/// <remarks>
		/// You can override this method to declare a better behavior.
		/// </remarks>
		public static void ThrowNotValidException(string[] validationErrorMessages, IDictionary propertiesValidationErrorMessages)
		{
			throw new ActiveRecordValidationException(
				"Can't save or update as there is one (or more) field that has not passed the validation test",
				validationErrorMessages, propertiesValidationErrorMessages);
		}

		internal static IEnumerable GetNestedPropertiesToValidate(object instance)
		{
			Type type = instance.GetType();
			ActiveRecordModel me = ActiveRecordModel.GetModel(type);

			foreach (NestedModel component in me.Components)
			{
				yield return component.Property;
			}
		}

		internal static void AddNestedPropertyValidationErrorMessages(List<string> errorMessages, object instance, ValidatorRunner runner)
		{
			foreach (PropertyInfo propinfo in GetNestedPropertiesToValidate(instance))
			{
				object propval = propinfo.GetValue(instance, null);
				if (propval != null)
				{
					ErrorSummary summary = runner.GetErrorSummary(propval);
					
					if (summary != null)
					{
						errorMessages.AddRange(summary.ErrorMessages);
					}
				}
			}
		}
	}
}
