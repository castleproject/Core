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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;
	using System.Reflection;

	public class PropertyBasedRegistry : IValidatorRegistry
	{
		#region IValidatorRegistry Members

		public IValidator[] GetValidators(Type targeType)
		{
			PropertyInfo[] properties = targeType.GetProperties();

			ArrayList list = new ArrayList();

			foreach(PropertyInfo prop in properties)
			{
				list.AddRange(GetValidators(targeType, prop));
			}

			return (IValidator[]) list.ToArray(typeof(IValidator));
		}

		public IValidator[] GetValidators(Type targeType, PropertyInfo property)
		{
			object[] builders = property.GetCustomAttributes(typeof(IValidatorBuilder), true);

			IValidator[] validators = new IValidator[builders.Length];
			int index = 0;

			foreach(IValidatorBuilder builder in builders)
			{
				IValidator validator = builder.Build();

				validator.Initialize(property);

				validators[index++] = validator;
			}

			return validators;
		}

		#endregion
	}
}
