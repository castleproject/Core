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

namespace Castle.Applications.PestControl.Services.SourceControl
{
	using System;

	public enum ParameterType
	{
		Single,
		Enum
	}

	/// <summary>
	/// Summary description for ParameterDefinition.
	/// </summary>
	public class ParameterDefinition
	{
		ParameterType _parameterType;
		String _name;
		String[] _possibleValues;

		public ParameterDefinition(String name)
		{
			_parameterType = ParameterType.Single;
			_name = name;
		}

		public ParameterDefinition(String name, String[] possibleValues) : this(name)
		{
			_parameterType = ParameterType.Enum;
			_possibleValues = possibleValues;
		}

		public ParameterType ParameterType
		{
			get { return _parameterType; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string[] PossibleValues
		{
			get { return _possibleValues; }
		}
	}
}
