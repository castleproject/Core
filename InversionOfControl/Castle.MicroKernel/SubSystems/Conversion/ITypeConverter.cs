// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Configuration;

	/// <summary>
	/// Implements a conversion logic to a type of a
	/// set of types. 
	/// </summary>
	public interface ITypeConverter
	{
		ITypeConverterContext Context { get; set; }

		/// <summary>
		/// Returns true if this instance of <c>ITypeConverter</c>
		/// is able to handle the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool CanHandleType( Type type );

        /// <summary>
        /// Returns true if this instance of <c>ITypeConverter</c>
        /// is able to handle the specified type with the specified 
        /// configuration
        /// </summary>
        /// <param name="type"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
        bool CanHandleType(Type type, IConfiguration configuration);

		/// <summary>
		/// Should perform the conversion from the
		/// string representation specified to the type
		/// specified.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object PerformConversion( String value, Type targetType );

		/// <summary>
		/// Should perform the conversion from the
		/// configuration node specified to the type
		/// specified.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object PerformConversion( IConfiguration configuration, Type targetType );
	}
}
