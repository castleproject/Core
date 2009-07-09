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

namespace Castle.ActiveRecord.Queries
{
	using System;
	using NHibernate.Type;

	/// <summary>
	/// Represents a query parameter value and type. Can be used to override the 
	/// standard mechanism of determing parameter types.
	/// </summary>
	public class ValueAndTypeTuple
	{
		private readonly IType type;
		private readonly Object value;

		/// <summary>
		/// Creates a new instance of ValueAndTypeTuple with no specific Type
		/// </summary>
		/// <param name="value">The value of the parameter</param>
		public ValueAndTypeTuple(object value)
		{
			this.value = value;
		}

		/// <summary>
		/// Creates a new instance of ValueAndTypeTuple with a specific Type
		/// </summary>
		/// <param name="type">The type of the parameter</param>
		/// <param name="value">The value of the parameter</param>
		public ValueAndTypeTuple(IType type, object value) : this(value)
		{
			this.type = type;		
		}

		/// <summary>
		/// The parameter type
		/// </summary>
		public IType Type
		{
			get { return type; }
		}

		/// <summary>
		/// The parameter value
		/// </summary>
		public object Value
		{
			get { return value; }
		}

		/// <summary>
		/// Creates a new ValueAndTypeTuple using the argument as the value
		/// unless the argument is already a ValueAndTypeTuple, in which case
		/// that is returned unmodified.
		/// </summary>
		public static ValueAndTypeTuple Wrap(object obj)
		{
			return obj as ValueAndTypeTuple ?? new ValueAndTypeTuple(obj);
		}
	}
}
