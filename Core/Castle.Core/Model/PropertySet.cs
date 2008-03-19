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
	using System.Reflection;

	/// <summary>
	/// Represents a property and the respective dependency.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class PropertySet
	{
		private readonly PropertyInfo propertyInfo;
		private readonly DependencyModel dependency;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertySet"/> class.
		/// </summary>
		/// <param name="propertyInfo">The property info.</param>
		/// <param name="dependency">The dependency.</param>
		public PropertySet(PropertyInfo propertyInfo, DependencyModel dependency)
		{
			this.propertyInfo = propertyInfo;
			this.dependency = dependency;
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return propertyInfo; }
		}

		/// <summary>
		/// Gets the dependency.
		/// </summary>
		/// <value>The dependency.</value>
		public DependencyModel Dependency
		{
			get { return dependency; }
		}
	}
}