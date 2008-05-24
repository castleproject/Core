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

namespace Castle.MonoRail.Framework.Helpers
{
	/// <summary>
	/// Defines whether the <see cref="FormHelper"/> should extract the value
	/// from the target and assign it to the <c>value</c> attribute of the generated
	/// HTML elemen.
	/// </summary>
	public enum ValueBehaviour
	{
		/// <summary>
		/// The form helper methods extracts the value from the target and assigns
		/// it to the <c>value</c> attribute of the generated HTML element.
		/// This is the default behaviour.
		/// </summary>
		Set,
		/// <summary>
		/// The form helper does not use the target to set the <c>value</c> attribute. This
		/// behaviour can be used in cases where it is a potential security risk to have the <c>value</c>
		/// attribute filled in with the target value.
		/// </summary>
		DoNotSet
	}
}
