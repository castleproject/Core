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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
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
		private IDictionary property2Messages = new HybridDictionary(true);

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

			ArrayList list = (ArrayList) property2Messages[name];

			if (list == null) return new string[0];

			return (string[]) list.ToArray(typeof(string));
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

			if (!property2Messages.Contains(property.Name))
			{
				property2Messages[property.Name] = new ArrayList();
			}

			IList list = (IList) property2Messages[property.Name];

			list.Add(message);

			errorsCount++;
			invalidPropertiesCount = property2Messages.Count;
		}
	}
}
