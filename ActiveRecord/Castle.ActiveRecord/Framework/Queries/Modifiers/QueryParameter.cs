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

namespace Castle.ActiveRecord.Queries.Modifiers
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Type;

	/// <summary>
	/// Represents a query parameter.
	/// </summary>
	public class QueryParameter : IQueryModifier
	{
		private readonly String name;
		private readonly int position = -1;
		private readonly Object value;
		private readonly IType type;
		private readonly ParameterFlags flags;

		#region Constructors for Named Parameters
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public QueryParameter(string name, object value)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.flags = ParameterFlags.Named;
			this.name = name;
			this.value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public QueryParameter(String name, Object value, IType type)
			: this(name, value)
		{
			this.type = type;
		}
		#endregion

		#region Constructors for Positional Parameters
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="value">The value.</param>
		public QueryParameter(int position, object value)
		{
			if (position < 0)
				throw new ArgumentException("Position must be equal or greater than 0", "position");

			this.flags = ParameterFlags.Positional;
			this.position = position;
			this.value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="position">The position.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public QueryParameter(int position, object value, IType type)
			: this(position, value)
		{
			this.type = type;
		}
		#endregion

		#region Constructors for Named List Parameters
		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <param name="type">The type.</param>
		public QueryParameter(String name, ICollection value, IType type)
			: this(name, (object) value, type)
		{
			this.flags |= ParameterFlags.List;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryParameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public QueryParameter(String name, ICollection value)
			: this(name, (object) value)
		{
			this.flags |= ParameterFlags.List;
		}
		#endregion

		#region Constructors for Positional List Parameters (throws exceptions)
		/// <remarks>
		/// It is important to keep this constructor as is, to avoid
		/// confusion with the <see cref="QueryParameter(int, object, IType)"/>
		/// overload.
		/// </remarks>
		public QueryParameter(int position, ICollection value, IType type)
			: this(position, (object) value, type)
		{
			throw new InvalidOperationException("Parameter lists can not be positional");
		}

		/// <remarks>
		/// It is important to keep this constructor as is, to avoid
		/// confusion with the <see cref="QueryParameter(int, object)"/>
		/// overload.
		/// </remarks>
		public QueryParameter(int position, ICollection value)
			: this(position, (object) value)
		{
			throw new InvalidOperationException("Parameter lists can not be positional");
		}
		#endregion

		#region Public Read-Only Properties
		/// <summary>
		/// The position of the positional parameter, or <c>-1</c>
		/// if this is a named parameter.
		/// </summary>
		public int Position
		{
			get { return position; }
		}

		/// <summary>
		/// The name of the named parameter, or <c>null</c>
		/// if this is a positional parameter.
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// The parameter value.
		/// </summary>
		public Object Value
		{
			get { return value; }
		}

		/// <summary>
		/// The NHibernate type.
		/// </summary>
		public IType Type
		{
			get { return type; }
		}
		#endregion

		#region "Apply" method
		/// <summary>
		/// Add this parameter to the <paramref name="query"/>.
		/// </summary>
		/// <param name="query">The query</param>
		/// <remarks>
		/// Is there a cleaner way to do this, without reflection or complex
		/// hierarchies?
		/// </remarks>
		public void Apply(IQuery query)
		{
			if (IsFlagged(ParameterFlags.Named))
			{
				if (IsFlagged(ParameterFlags.List))
				{
					if (Type != null)
						query.SetParameterList(Name, (ICollection) Value, Type);
					else
						query.SetParameterList(Name, (ICollection) Value);
				}
				else
				{
					if (Type != null)
						query.SetParameter(Name, Value, Type);
					else
						query.SetParameter(Name, Value);
				}
			}
			else if (IsFlagged(ParameterFlags.Positional))
			{
				if (IsFlagged(ParameterFlags.List))
				{
					throw new InvalidOperationException("Parameter lists can not be positional");
				}
				else
				{
					if (Type != null)
						query.SetParameter(Position, Value, Type);
					else
						query.SetParameter(Position, Value);
				}
			}
		}
		#endregion

		#region Flags
		private bool IsFlagged(ParameterFlags flag)
		{
			return ((flags & flag) == flag);
		}

		[Flags]
		private enum ParameterFlags
		{
			Positional = 0x0001,
			Named = 0x0002,

			List = 0x0010,
		}
		#endregion
	}
}