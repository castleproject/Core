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
// limitations under the License.using System;

namespace Castle.MicroKernel.Tests.Configuration.Components
{
	using Castle.MicroKernel.SubSystems.Conversion;

	public class ClassWithComplexParameter
	{
		private ComplexParameterType param1;

		[Convertible]
		public class ComplexParameterType
		{
			private string mandatoryValue;
			private string optionalValue;

			public ComplexParameterType()
			{
				// sets default values
				mandatoryValue = "default1";
				optionalValue = "default2";
			}

			public ComplexParameterType(string mandatoryValue)
			{
				this.mandatoryValue = mandatoryValue;
			}

			public string MandatoryValue
			{
				get { return mandatoryValue; }
			}

			public string OptionalValue
			{
				get { return optionalValue; }
				set { optionalValue = value; }
			}
		}

		public ComplexParameterType ComplexParam
		{
			get { return param1; }
			set { param1 = value; }
		}
	}
}