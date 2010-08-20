// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Represents a validation report for an object instance
	/// which is a snapshot since the last validation check.
	/// </summary>
	[Serializable]
	public class ErrorSummary
	{
		private int errorsCount;
		private int invalidPropertiesCount;
		private IDictionary<string, string[]> property2Messages = 
			new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Indicates whether some error was registered on this summary instance.
		/// </summary>
		/// <value><c>true</c> if this instance has any error registered; otherwise, <c>false</c>.</value>
		public bool HasError
		{
			get { return errorsCount != 0 || invalidPropertiesCount != 0; }
		}

		/// <summary>
		/// Gets the total of validation errors since the last validation check.
		/// <para>
		/// That includes all errors for all properties.
		/// </para>
		/// </summary>
		/// <value>The error count.</value>
		public int ErrorsCount
		{
			get { return errorsCount; }
		}

		/// <summary>
		/// Gets the total of properties that have failed validation checks.
		/// </summary>
		public int InvalidPropertiesCount
		{
			get { return invalidPropertiesCount; }
		}

		/// <summary>
		/// Gets the invalid properties' name.
		/// </summary>
		/// <value>The invalid properties.</value>
		public string[] InvalidProperties
		{
			get
			{
				string[] names = new string[property2Messages.Count];
				property2Messages.Keys.CopyTo(names, 0);
				return names;
			}
		}

		/// <summary>
		/// Gets the error messages.
		/// </summary>
		/// <value>The error messages.</value>
		public string[] ErrorMessages
		{
			get
			{
				string[] messages = new string[errorsCount];

				int index = 0;

				foreach(IList list in property2Messages.Values)
				{
					list.CopyTo(messages, index);
					index += list.Count;
				}

				return messages;
			}
		}

		/// <summary>
		/// Gets the errors for a property.
		/// </summary>
		/// <param name="name">The property name.</param>
		/// <returns>Array of error messages</returns>
		public string[] GetErrorsForProperty(string name)
		{
			if (name == null) throw new ArgumentNullException("name");

			if (!property2Messages.ContainsKey(name))
			{
				return new string[0];
			}

			string[] messages = property2Messages[name];

			return messages;
		}

		/// <summary>
		/// Registers the error message per <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="message">The message.</param>
		public void RegisterErrorMessage(PropertyInfo property, string message)
		{
			if (property == null) throw new ArgumentNullException("property");
			if (message == null) throw new ArgumentNullException("message");

			RegisterErrorMessage(property.Name, message);
		}

		/// <summary>
		/// Registers the error message per <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="message">The message.</param>
		public void RegisterErrorMessage(string property, string message)
		{
			if (property == null) throw new ArgumentNullException("property");
			if (message == null) throw new ArgumentNullException("message");

			if (!property2Messages.ContainsKey(property))
			{
				property2Messages[property] = new string[] { message };
			}
			else
			{
				string[] messages = property2Messages[property];
				Array.Resize(ref messages, messages.Length + 1);
				messages[messages.Length - 1] = message;
				property2Messages[property] = messages;
			}

			errorsCount++;
			invalidPropertiesCount = property2Messages.Count;
		}

		/// <summary>
		/// Registers the errors from another error summary instance.
		/// </summary>
		/// <param name="errorSummary">The error summary.</param>
		public void RegisterErrorsFrom(ErrorSummary errorSummary)
		{
			foreach (string property in errorSummary.InvalidProperties)
			{
				foreach (string errorMessage in errorSummary.GetErrorsForProperty(property))
				{
					RegisterErrorMessage(property, errorMessage);
				}
			}
		}
	}
}
