// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Configuration
{
	using System;
	using System.Globalization;

	/// <summary>
	///   This is an abstract <see cref = "IConfiguration" /> implementation
	///   that deals with methods that can be abstracted away
	///   from underlying implementations.
	/// </summary>
	/// <remarks>
	///   <para><b>AbstractConfiguration</b> makes easier to implementers 
	///     to create a new version of <see cref = "IConfiguration" /></para>
	/// </remarks>
#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public abstract class AbstractConfiguration : IConfiguration
	{
		private readonly ConfigurationAttributeCollection attributes = new ConfigurationAttributeCollection();
		private readonly ConfigurationCollection children = new ConfigurationCollection();

		/// <summary>
		///   Gets node attributes.
		/// </summary>
		/// <value>
		///   All attributes of the node.
		/// </value>
		public virtual ConfigurationAttributeCollection Attributes
		{
			get { return attributes; }
		}

		/// <summary>
		///   Gets all child nodes.
		/// </summary>
		/// <value>The <see cref = "ConfigurationCollection" /> of child nodes.</value>
		public virtual ConfigurationCollection Children
		{
			get { return children; }
		}

		/// <summary>
		///   Gets the name of the <see cref = "IConfiguration" />.
		/// </summary>
		/// <value>
		///   The Name of the <see cref = "IConfiguration" />.
		/// </value>
		public string Name { get; protected set; }

		/// <summary>
		///   Gets the value of <see cref = "IConfiguration" />.
		/// </summary>
		/// <value>
		///   The Value of the <see cref = "IConfiguration" />.
		/// </value>
		public string Value { get; protected set; }

		/// <summary>
		///   Gets the value of the node and converts it
		///   into specified <see cref = "Type" />.
		/// </summary>
		/// <param name = "type">The <see cref = "Type" /></param>
		/// <param name = "defaultValue">
		///   The Default value returned if the conversion fails.
		/// </param>
		/// <returns>The Value converted into the specified type.</returns>
		public virtual object GetValue(Type type, object defaultValue)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			try
			{
				return Convert.ChangeType(Value, type, CultureInfo.CurrentCulture);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}
	}
}