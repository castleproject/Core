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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;

	/// <summary>
	/// Implements a conversion logic to a type of a
	/// set of types. 
	/// </summary>
	public interface ITypeConverter
	{
		/// <summary>
		/// Returns true if this instance of <code>ITypeConverter</code>
		/// is able to handle the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool CanHandleType( Type type );

		/// <summary>
		/// Should perform the conversion from the
		/// string representation specified to the type
		/// specified.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object PerformConversion( String value, Type targetType );
	}
}
