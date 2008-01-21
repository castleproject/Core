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

namespace Castle.Core
{
	using System;

	/// <summary>
	/// This attribute is usefull only when you want to register all components
	/// on an assembly as a batch process. 
	/// By doing so, the batch register will look 
	/// for this attribute to distinguish components from other classes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public class CastleComponentAttribute : LifestyleAttribute
	{
		private readonly Type service;
		private readonly string key;

		/// <summary>
		/// Initializes a new instance of the <see cref="CastleComponentAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public CastleComponentAttribute(String key) : this(key, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CastleComponentAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="service">The service.</param>
		public CastleComponentAttribute(String key, Type service) : this(key, service, LifestyleType.Undefined)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CastleComponentAttribute"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="service">The service.</param>
		/// <param name="lifestyle">The lifestyle.</param>
		public CastleComponentAttribute(String key, Type service, LifestyleType lifestyle) : base(lifestyle)
		{
			this.key = key;
			this.service = service;
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <value>The service.</value>
		public Type Service
		{
			get { return service; }
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public String Key
		{
			get { return key; }
		}
	}
}