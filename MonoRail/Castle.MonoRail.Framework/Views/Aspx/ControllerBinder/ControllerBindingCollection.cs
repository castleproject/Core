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
	using System.Web.UI;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ControllerBindingCollection : TypedCollection<ControllerBinding>
	{
		private readonly ControllerBinder binder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBindingCollection"/> class.
		/// </summary>
		/// <param name="binder">The binder.</param>
		public ControllerBindingCollection(ControllerBinder binder)
		{
			this.binder = binder;
		}

		/// <summary>
		/// Gets the <see cref="Castle.MonoRail.Framework.Views.Aspx.ControllerBinding"/> with the specified control.
		/// </summary>
		/// <value></value>
		public ControllerBinding this[Control control]
		{
			get
			{
				if (control == null)
				{
					throw new ArgumentNullException("control");
				}
				return this[control.ID];
			}
		}

		/// <summary>
		/// Gets the <see cref="Castle.MonoRail.Framework.Views.Aspx.ControllerBinding"/> with the specified control id.
		/// </summary>
		/// <value></value>
		public ControllerBinding this[string controlId]
		{
			get
			{
				if (controlId == null)
				{
					throw new ArgumentNullException("controlId");
				}

				foreach(ControllerBinding binding in List)
				{
					if (binding.ControlID == controlId)
					{
						return binding;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Removes the specified control.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns></returns>
		public bool Remove(Control control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}

			return Remove(control.ID);
		}

		/// <summary>
		/// Removes the specified control id.
		/// </summary>
		/// <param name="controlId">The control id.</param>
		/// <returns></returns>
		public bool Remove(string controlId)
		{
			if (controlId == null)
			{
				throw new ArgumentNullException("controlId");
			}

			for(int i = 0; i < List.Count; ++i)
			{
				if (this[i].ControlID == controlId)
				{
					RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			ControllerBinding oldBinding = (ControllerBinding) oldValue;
			oldBinding.Binder = null;

			ControllerBinding newBinding = (ControllerBinding) newValue;
			newBinding.Binder = binder;

			EnsureActionBindingExists(newBinding);

			base.OnSetComplete(index, oldValue, newValue);
		}

		/// <summary>
		/// The VS Designer adds new items as soon as their accessed,
		/// but items may not be valid so we have to clean up.
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// </summary>
		protected override void OnInsertComplete(int index, object value)
		{
			ControllerBinding binding = (ControllerBinding) value;
			binding.Binder = binder;

			EnsureActionBindingExists(binding);
			PruneEmptyBindingsInDesignMode();

			base.OnInsertComplete(index, value);
		}

		/// <summary>
		/// Performs additional custom processes after removing an element from the <see cref="T:System.Collections.CollectionBase"></see> instance.
		/// </summary>
		/// <param name="index">The zero-based index at which value can be found.</param>
		/// <param name="value">The value of the element to remove from index.</param>
		protected override void OnRemoveComplete(int index, object value)
		{
			ControllerBinding binding = (ControllerBinding) value;
			binding.Binder = null;
		}

		private void EnsureActionBindingExists(ControllerBinding binding)
		{
			// Populate the default event in design mode.

			if (binder.DesignMode && binding.ActionBindings.Count == 0)
			{
				binding.ActionBindings.Add(new ActionBinding());
			}
		}

		private void PruneEmptyBindingsInDesignMode()
		{
			if (!binder.DesignMode || binder == null) return;

			for(int i = 0; i < Count;)
			{
				ControllerBinding binding = this[i];

				if (binding.ActionBindings.Count == 0 ||
				    (binding.ControlInstance != null &&
				     binding.ControlInstance.ID != binding.ControlID))
				{
					RemoveAt(i);
				}
				else
				{
					++i;
				}
			}
		}
	}
}

#endif
