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

namespace Castle.Core.Configuration
{
	using System;
	using System.Collections.Specialized;

	/// <summary>
	/// This is an abstract <see cref="IConfiguration"/> implementation
	/// that deals with methods that can be abstracted away
	/// from underlying implementations.
	/// </summary>
	/// <remarks>
	/// <para><b>AbstractConfiguration</b> makes easier to implementers 
	/// to create a new version of <see cref="IConfiguration"/></para>
	/// </remarks>
#if !SILVERLIGHT
	[Serializable]
#endif
	public abstract class AbstractConfiguration : IConfiguration
	{
		private String internalName;
		private String internalValue;
		private readonly NameValueCollection attributes = new NameValueCollection();
		private readonly ConfigurationCollection children = new ConfigurationCollection();

		/// <summary>
		/// Gets the name of the <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The Name of the <see cref="IConfiguration"/>.
		/// </value>
		public virtual String Name
		{
			get { return internalName; }
			protected set { internalName = value; }
		}

		/// <summary>
		/// Gets the value of <see cref="IConfiguration"/>.
		/// </summary>
		/// <value>
		/// The Value of the <see cref="IConfiguration"/>.
		/// </value>
		public virtual String Value
		{
			get { return internalValue; }
			protected set { internalValue = value; }
		}

		/// <summary>
		/// Gets all child nodes.
		/// </summary>
		/// <value>The <see cref="ConfigurationCollection"/> of child nodes.</value>
		public virtual ConfigurationCollection Children
		{
			get { return children; }
		}

		/// <summary>
		/// Gets node attributes.
		/// </summary>
		/// <value>
		/// All attributes of the node.
		/// </value>
		public virtual NameValueCollection Attributes
		{
			get { return attributes; }
		}

		/// <summary>
		/// Gets the value of the node and converts it
		/// into specified <see cref="System.Type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/></param>
		/// <param name="defaultValue">
		/// The Default value returned if the convertion fails.
		/// </param>
		/// <returns>The Value converted into the specified type.</returns>
		public virtual object GetValue(Type type, object defaultValue)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			try
			{
				return Convert.ChangeType(Value, type, System.Threading.Thread.CurrentThread.CurrentCulture);
			}
			catch(InvalidCastException)
			{
				return defaultValue;
			}
		}
	}
}
