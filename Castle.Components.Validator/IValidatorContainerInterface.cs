// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using Castle.Components.Validator.Attributes;

	/// <summary>
	/// This is a base interface that should be implemented by 
	/// interfaces that have members with validation attributes.
	/// </summary>
	/// <remarks>
	/// Note that that the interface should be explicitely flagged 
	/// with the <see cref="ValidatorContainerInterfaceFlagAttribute"/>
	/// attribute
	/// </remarks>
	/// <example>
	/// [ValidatorContainerInterfaceFlag]
	/// public interface IMyModelValidationDeclaration
	///		: IValidatorContainerInterface
	/// {
	///		[ValidateNonEmpty]
	///		string MyProperty { get; }
	/// }
	/// </example>
	public interface IValidatorContainerInterface
	{ 
		
	}
}
