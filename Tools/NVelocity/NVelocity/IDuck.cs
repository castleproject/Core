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
// limitations under the License.

namespace NVelocity
{
	using System;

	/// <summary>
	/// 
	/// </summary>
	public interface IDuck
	{
		/// <summary>
		/// Defines the behavior when a property is read
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <returns>value back to the template</returns>
		object GetInvoke(String propName);

		/// <summary>
		/// Defines the behavior when a property is written
		/// </summary>
		/// <param name="propName">Property name.</param>
		/// <param name="value">The value to assign.</param>
		void SetInvoke(String propName, object value);

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method name.</param>
		/// <param name="args">The method arguments.</param>
		/// <returns>value back to the template</returns>
		object Invoke(String method, params object[] args);
	}
}