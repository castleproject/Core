// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	/// <summary>
	/// Represents a constructor of the component 
	/// that the container can use to initialize it properly.
	/// </summary>
	public class ConstructorCandidate
	{
		private ConstructorInfo _constructorInfo;
		private DependencyModel[] _dependencies;
		private int _points;

		public ConstructorCandidate( ConstructorInfo constructorInfo, params DependencyModel[] dependencies )
		{
			_constructorInfo = constructorInfo;
			_dependencies = dependencies;
		}

		public ConstructorInfo Constructor
		{
			get { return _constructorInfo; }
		}

		public DependencyModel[] Dependencies
		{
			get { return _dependencies; }
		}

		public int Points
		{
			get { return _points; }
			set { _points = value; }
		}
	}
}
