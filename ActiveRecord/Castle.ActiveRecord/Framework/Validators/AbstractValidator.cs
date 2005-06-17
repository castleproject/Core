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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	using System.Reflection;


	[Serializable]
	public abstract class AbstractValidator : IValidator
	{
		private PropertyInfo _property;
		private String _errorMessage;

		public PropertyInfo Property
		{
			get { return _property; }
		}

		public void Initialize(PropertyInfo property)
		{
			_property = property;

			if (_errorMessage == null)
			{
				_errorMessage = BuildErrorMessage();
			}
		}

		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { _errorMessage = value; }
		}

		public bool Perform(object instance)
		{
			return this.Perform( instance, Property.GetValue(instance, null) );
		}

		public abstract bool Perform(object instance, object fieldValue);

		protected abstract string BuildErrorMessage();
	}
}
