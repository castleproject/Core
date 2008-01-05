// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Extends the <see cref="SmartDispatcherController"/> 
	/// with ActiveRecord specific functionality
	/// </summary>
	public class ARSmartDispatcherController : SmartDispatcherController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ARSmartDispatcherController"/> class.
		/// </summary>
		public ARSmartDispatcherController() : base(new ARDataBinder())
		{
		}

		/// <summary>
		/// Binds the object using the posted values.
		/// </summary>
		/// <param name="from">Defines where the parameters should be obtained from.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="excludedProperties">The excluded properties.</param>
		/// <param name="allowedProperties">The allowed properties.</param>
		/// <param name="autoLoad">The auto load behavior.</param>
		/// <returns></returns>
		protected object BindObject(ParamStore from, Type targetType, String prefix, String excludedProperties, String allowedProperties, AutoLoadBehavior autoLoad)
		{
			SetAutoLoadBehavior(autoLoad);

			return BindObject(from, targetType, prefix, excludedProperties, allowedProperties);
		}

		/// <summary>
		/// Binds the object.
		/// </summary>
		/// <param name="from">Defines where the parameters should be obtained from.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="autoLoad">The auto load behavior.</param>
		/// <returns></returns>
		protected object BindObject(ParamStore from, Type targetType, String prefix, AutoLoadBehavior autoLoad)
		{
			SetAutoLoadBehavior(autoLoad);
			return BindObject(from, targetType, prefix);
		}

		protected void BindObjectInstance(object instance, ParamStore from, String prefix, AutoLoadBehavior autoLoad)
		{
			SetAutoLoadBehavior(autoLoad);
			BindObjectInstance(instance, from, prefix);
		}

		protected void BindObjectInstance(object instance, String prefix, AutoLoadBehavior autoLoad)
		{
			SetAutoLoadBehavior(autoLoad);
			BindObjectInstance(instance, ParamStore.Params, prefix);
		}

		protected void SetAutoLoadBehavior(AutoLoadBehavior autoLoad)
		{
			ARDataBinder binder = (ARDataBinder) Binder;
			binder.AutoLoad = autoLoad;
		}
	}
}
