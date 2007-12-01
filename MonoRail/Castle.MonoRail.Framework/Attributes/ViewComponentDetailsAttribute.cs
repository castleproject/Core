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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Identifies the cache strategy associated with a view component
	/// </summary>
	public enum ViewComponentCache
	{
		/// <summary>
		/// No cache 
		/// </summary>
		Disabled,
		/// <summary>
		/// Always cache the view component output, no varying.
		/// </summary>
		Always,
		/// <summary>
		/// Uses a custom key generator that should implement the vary algorithm. 
		/// </summary>
		UseCustomCacheKeyGenerator
	}

	/// <summary>
	/// Decorates a <see cref="ViewComponent"/> to associate a custom name with it.
	/// </summary>
    /// <remarks>
    /// Decorates a <see cref="ViewComponent"/> to associate a custom name with it.
    /// <para>
    /// Optionally you can associate the section names supported by the 
    /// <see cref="ViewComponent"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// In the code below, the class MyHeaderViewConponent will be referenced as just <c>Header</c>,
    /// and it will support the subsections <c>header</c> and <c>footer</c>.
    /// <code><![CDATA[
    /// [ViewComponentDetails("Header", Sections="header,footer")
    /// public class MyHeaderViewComponent : ViewComponent
    /// {
    ///    // :
    ///    // :
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="ViewComponent"/>
    /// <seealso cref="ViewComponentParamAttribute"/>
	[AttributeUsage(AttributeTargets.Class), Serializable]
	public class ViewComponentDetailsAttribute : Attribute
	{
		private readonly string name;
		private string sections;
		private ViewComponentCache cache = ViewComponentCache.Disabled;
		private Type cacheKeyFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentDetailsAttribute"/> class.
		/// </summary>
		/// <param name="name">The specified ViewComponent's Name</param>
		public ViewComponentDetailsAttribute(String name)
		{
			this.name = name;
		}

		/// <summary>
		/// The component's name
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Sets the nested sections that this <see cref="ViewComponent"/> supports.
		/// </summary>
		/// <value>The nested sections names, comma separated.</value>
		public string Sections
		{
			get { return sections; }
			set { sections = value; }
		}

		/// <summary>
		/// Sets the cache strategy.
		/// </summary>
		/// <value>The cache.</value>
		public ViewComponentCache Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary>
		/// Sets the cache key factory.
		/// </summary>
		/// <value>The cache key factory.</value>
		public Type CacheKeyFactory
		{
			get { return cacheKeyFactory; }
			set
			{
				if (value != null)
				{
					cache = ViewComponentCache.UseCustomCacheKeyGenerator;
				}
				cacheKeyFactory = value;
			}
		}
	}
}
