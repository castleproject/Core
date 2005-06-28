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

namespace Castle.MonoRail.Framework
{
	using System;

	public interface IPropertyError
	{
		/// <summary>
		/// Key identifying this error.
		/// </summary>
		String Key { get; }

		/// <summary>
		/// Name of the property this error occured on.
		/// </summary>
		String Property { get; }
		
		/// <summary>
		/// Name of the parent class this property belongs to.
		/// </summary>
		String Parent { get; }

		/// <summary>
		/// Exception that was raised.
		/// </summary>
		Exception Exception { get; }

		/// <summary>
		/// ToString combines key with interface implementation.
		/// </summary>
		String ToString();
	}
}
