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

namespace Castle.MicroKernel.Registration
{
	using System;
	using Castle.Core.Configuration;

	/// <summary>
	/// Represents a configuration parameter.
	/// </summary>
	public class Parameter
	{
		private readonly String key;
		private readonly String value;
		private readonly IConfiguration configNode;

		internal Parameter(String key, String value)
		{
			this.key = key;
			this.value = value;
		}

		internal Parameter(String key, IConfiguration configNode)
		{
			this.key = key;
			this.configNode = configNode;
		}

		/// <summary>
		/// Gets the parameter key.
		/// </summary>
		public string Key
		{
			get { return key; }
		}

		/// <summary>
		/// Gets the parameter value.
		/// </summary>
		public String Value
		{
			get { return value; }
		}

		/// <summary>
		/// Gets the parameter configuration.
		/// </summary>
		public IConfiguration ConfigNode
		{
			get { return configNode; }
		}

		/// <summary>
		/// Create a <see cref="ParameterKey"/> with key.
		/// </summary>
		/// <param name="key">The parameter key.</param>
		/// <returns>The new <see cref="ParameterKey"/></returns>
		public static ParameterKey ForKey(String key)
		{
			return new ParameterKey(key);
		}
	}

	#region ParameterKey
	
	/// <summary>
	/// Represents a parameter key.
	/// </summary>
	public class ParameterKey
	{
		private readonly String name;

		internal ParameterKey(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// The parameter key name.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Builds the <see cref="Parameter"/> with key/value.
		/// </summary>
		/// <param name="value">The parameter value.</param>
		/// <returns>The new <see cref="Parameter"/></returns>
		public Parameter Eq(String value)
		{
			return new Parameter(name, value);
		}

		/// <summary>
		/// Builds the <see cref="Parameter"/> with key/config.
		/// </summary>
		/// <param name="configNode">The parameter configuration.</param>
		/// <returns>The new <see cref="Parameter"/></returns>
		public Parameter Eq(IConfiguration configNode)
		{
			return new Parameter(name, configNode);
		}
		
		#endregion
	}
}