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

#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Web.UI;

	public class ControllerBindingCollection : TypedCollection<ControllerBinding>
	{
		private readonly ControllerBinder binder;

		public ControllerBindingCollection(ControllerBinder binder)
		{
			this.binder = binder;
		}

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

		public bool Remove(Control control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}

			return Remove(control.ID);
		}

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
