// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	[Serializable]
	public class DependencyModel
	{
		private String dependencyKey;
		private Type targetType;
		private bool isOptional;
		private DependencyType dependencyType;

		public DependencyModel(DependencyType type, String dependencyKey, 
		                       Type targetType, bool isOptional)
		{
			this.dependencyType = type;
			this.dependencyKey = dependencyKey;
			this.targetType = targetType;
			this.isOptional = isOptional;
		}

		public DependencyType DependencyType
		{
			get { return dependencyType; }
			set { dependencyType = value; }
		}

		public String DependencyKey
		{
			get { return dependencyKey; }
			set { dependencyKey = value; }
		}

		public Type TargetType
		{
			get { return targetType; }
		}

		public bool IsOptional
		{
			get { return isOptional; }
		}

        public override string ToString()
        {
            return string.Format("{0} dependency '{1}' type '{2}'", 
                                 DependencyType, dependencyKey, TargetType);
        }

		public override int GetHashCode()
		{
			int result = dependencyKey.GetHashCode();
			result += 37 ^ targetType.GetHashCode();
			result += 37 ^ isOptional.GetHashCode();
			result += 37 ^ dependencyType.GetHashCode();
			return result;
		}

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