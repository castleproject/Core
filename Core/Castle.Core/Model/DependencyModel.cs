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
	using System.Globalization;

	public enum DependencyType
	{
		Service,
		Parameter,
		ServiceOverride
	}

	/// <summary>
	/// Represents a dependency (other component or a 
	/// fixed value available through external configuration).
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class DependencyModel
	{
		private String dependencyKey;
		private Type targetType;
		private bool isOptional;
		private DependencyType dependencyType;

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyModel"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="dependencyKey">The dependency key.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="isOptional">if set to <c>true</c> [is optional].</param>
		public DependencyModel(DependencyType type, String dependencyKey,
		                       Type targetType, bool isOptional)
		{
			dependencyType = type;
			this.dependencyKey = dependencyKey;
			this.targetType = targetType;
			this.isOptional = isOptional;
		}

		/// <summary>
		/// Gets or sets the type of the dependency.
		/// </summary>
		/// <value>The type of the dependency.</value>
		public DependencyType DependencyType
		{
			get { return dependencyType; }
			set { dependencyType = value; }
		}

		/// <summary>
		/// Gets or sets the dependency key.
		/// </summary>
		/// <value>The dependency key.</value>
		public String DependencyKey
		{
			get { return dependencyKey; }
			set { dependencyKey = value; }
		}

		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		/// <value>The type of the target.</value>
		public Type TargetType
		{
			get { return targetType; }
		}

		/// <summary>
		/// Gets or sets whether this dependency is optional.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this dependency is optional; otherwise, <c>false</c>.
		/// </value>
		public bool IsOptional
		{
			get { return isOptional; }
			set { isOptional = value; }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} dependency '{1}' type '{2}'",
			                     DependencyType, dependencyKey, TargetType);
		}

		/// <summary>
		/// Serves as a hash function for a particular type, suitable
		/// for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			int result = dependencyKey.GetHashCode();
			result += 37 ^ targetType.GetHashCode();
			result += 37 ^ isOptional.GetHashCode();
			result += 37 ^ dependencyType.GetHashCode();
			return result;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified <see cref="T:System.Object"/> is equal to the
		/// current <see cref="T:System.Object"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			DependencyModel dependencyModel = obj as DependencyModel;
			if (dependencyModel == null) return false;
			if (!Equals(dependencyKey, dependencyModel.dependencyKey)) return false;
			if (!Equals(targetType, dependencyModel.targetType)) return false;
			if (!Equals(isOptional, dependencyModel.isOptional)) return false;
			if (!Equals(dependencyType, dependencyModel.dependencyType)) return false;
			return true;
		}
	}
}