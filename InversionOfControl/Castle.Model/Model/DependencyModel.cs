// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Model
{
	using System;

	public enum DependencyType
	{
		Service,
		Parameter
	}

	/// <summary>
	/// Summary description for DependencyModel.
	/// </summary>
	public class DependencyModel
	{
		private String _dependencyKey;
		private Type _targetType;
		private bool _isOptional;
		private DependencyType _dependencyType;

		public DependencyModel( DependencyType type, String dependencyKey, 
			Type targetType, bool isOptional)
		{
			_dependencyType = type;
			_dependencyKey = dependencyKey;
			_targetType = targetType;
			_isOptional = isOptional;
		}

		public DependencyType DependencyType
		{
			get { return _dependencyType; }
		}

		public String DependencyKey
		{
			get { return _dependencyKey; }
		}

		public Type TargetType
		{
			get { return _targetType; }
		}

		public bool IsOptional
		{
			get { return _isOptional; }
		}
	}
}
