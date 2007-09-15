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

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionBindingCollection : TypedCollection<ActionBinding>
	{
		private readonly ControllerBinding parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionBindingCollection"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public ActionBindingCollection(ControllerBinding parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			
			this.parent = parent;
		}

		/// <summary>
		/// Gets the <see cref="Castle.MonoRail.Framework.Views.Aspx.ActionBinding"/> with the specified event name.
		/// </summary>
		/// <value></value>
		public ActionBinding this[string eventName]
		{
			get
			{
				foreach (ActionBinding binding in List)
				{
					if (binding.EventName == eventName)
					{
						return binding;
					}
				}
				
				return null;
			}
		}

		/// <summary>
		/// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			ActionBinding oldBinding = (ActionBinding) oldValue;
			oldBinding.Parent = null;
			
			ActionBinding newBinding = (ActionBinding) newValue;
			newBinding.Parent = parent;

			base.OnSetComplete(index, oldValue, newValue);
		}

		/// <summary>
		/// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsertComplete(int index, object value)
		{
			ActionBinding binding = (ActionBinding) value;
			binding.Parent = parent;

			base.OnInsertComplete(index, value);
		}
	}
}

#endif
