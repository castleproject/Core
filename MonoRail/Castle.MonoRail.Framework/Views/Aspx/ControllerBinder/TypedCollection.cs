// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections;
	using System.ComponentModel;

	/// <summary>
	/// Pendent
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TypedCollection<T> : CollectionBase
		where T : AbstractBindingComponent
	{
		/// <summary>
		/// Gets or sets the <typeparamref name="T"/> at the specified index.
		/// </summary>
		/// <value></value>
		public T this[int index]
		{
			get { return (T) List[index]; }
			set { List[index] = value; }
		}

		/// <summary>
		/// Adds the specified member.
		/// </summary>
		/// <param name="member">The member.</param>
		public void Add(T member)
		{
			List.Add(member);
		}

		/// <summary>
		/// Adds at.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="member">The member.</param>
		public void AddAt(int index, T member)
		{
			List.Insert(index, member);
		}

		/// <summary>
		/// Removes the specified member.
		/// </summary>
		/// <param name="member">The member.</param>
		public void Remove(T member)
		{
			List.Remove(member);
		}

		/// <summary>
		/// Determines whether [contains] [the specified member].
		/// </summary>
		/// <param name="member">The member.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified member]; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(T member)
		{
			return List.Contains(member);
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(object value)
		{
			base.OnValidate(value);

			if (!(value is T))
			{
				throw new ArgumentException(string.Format(
				                            	"Only {0} instances are supported by this collection.", typeof(T).Name));
			}

			AbstractBindingComponent properties = (AbstractBindingComponent) value;

			// We cannot throw the exception in Design Mode since the 
			// Set<Property> is never called to commit the update and so
			// the collection may contain invalid values.

			if (!properties.IsValid() && !Design.DesignUtil.IsInDesignMode)
			{
				throw new ArgumentException(properties.Error);
			}
		}

		#region Hide Properties for the Designer

		/// <summary>
		/// Gets or sets the number of elements that the <see cref="T:System.Collections.CollectionBase"></see> can contain.
		/// </summary>
		/// <value></value>
		/// <returns>The number of elements that the <see cref="T:System.Collections.CollectionBase"></see> can contain.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="P:System.Collections.CollectionBase.Capacity"></see> is set to a value that is less than <see cref="P:System.Collections.CollectionBase.Count"></see>.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Capacity
		{
			get { return base.Capacity; }
			set { base.Capacity = value; }
		}

		#endregion
	}
}

#endif
