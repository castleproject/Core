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

namespace Castle.CastleOnRails.Framework
{
	using System;

	/// <summary>
	/// Decorates a method, indicating that 
	/// no filter should be applied when the method
	/// is invoked.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class SkipFilter : Attribute
	{
	}

	/// <summary>
	/// Decorates a controller associating a <see cref="IFilter"/>
	/// implementation with it. More than one can be associated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class FilterAttribute : Attribute
	{
		private Type _filterType;
		private ExecuteEnum _when;

		/// <summary>
		/// Constructs a FilterAttribute associating 
		/// the filter type and when the filter should be invoked.
		/// </summary>
		/// <param name="when"></param>
		/// <param name="filterType"></param>
		public FilterAttribute( ExecuteEnum when, Type filterType )
		{
			if (!typeof(IFilter).IsAssignableFrom(filterType))
			{
				throw new ArgumentException("The specified filter does not implement IFilter");
			}

			_filterType = filterType;
			_when = when;
		}

		public Type FilterType
		{
			get { return _filterType; }
		}

		public ExecuteEnum When
		{
			get { return _when; }
		}
	}
}
