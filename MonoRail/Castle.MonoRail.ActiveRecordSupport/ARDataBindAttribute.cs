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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using System.Reflection;
	using Castle.Components.Binder;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Defines the behavior of 
	/// Autoload feature on <see cref="ARDataBinder"/>
	/// </summary>
	public enum AutoLoadBehavior
	{
		/// <summary>
		/// Means that no autoload should be perform on the target
		/// type or on nested types.
		/// </summary>
		Never,
		
		/// <summary>
		/// Means that autoload should be used for the target type
		/// and the nested types (if present). This demands that
		/// the primary key be present on the http request
		/// </summary>
		Always,
		
		/// <summary>
		/// Does not load the root type, but loads nested types
		/// if the primary key is present. If not present, sets null on nested type.
		/// </summary>
		OnlyNested,

		/// <summary>
		/// Means that we should autoload, but if the key is 
		/// invalid, like <c>null</c>, 0 or an empty string, then just
		/// create a new instance of the target type.
		/// </summary>
		NewInstanceIfInvalidKey,
		
		/// <summary>
		/// Means that we should autoload, but if the key is 
		/// invalid, like <c>null</c>, 0 or an empty string, then just
		/// return null
		/// </summary>
		NullIfInvalidKey
	}

	/// <summary>
	/// Extends <see cref="DataBindAttribute"/> with 
	/// ActiveRecord specific functionallity
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter), Serializable]
	public class ARDataBindAttribute : DataBindAttribute, IParameterBinder
	{
		private AutoLoadBehavior autoLoad = AutoLoadBehavior.Never;

		/// <summary>
		/// Defines a binder for the parameter
		/// using the <see cref="ARDataBinder"/> and the 
		/// specified <c>prefix</c>.
		/// </summary>
		/// <remarks>
		/// This uses the default <see cref="AutoLoadBehavior"/>
		/// whic is <see cref="AutoLoadBehavior.Never"/>
		/// </remarks>
		/// <param name="prefix">A name that prefixes the entries on the http request</param>
		public ARDataBindAttribute(String prefix) : base(prefix)
		{
		}

		/// <summary>
		/// Defines a binder for the parameter
		/// using the <see cref="ARDataBinder"/> and the 
		/// specified <c>prefix</c>.
		/// </summary>
		/// <param name="prefix">A name that prefixes the entries on the http request</param>
		/// <param name="autoLoadBehavior">The predefined behavior the autoload feature should use</param>
		public ARDataBindAttribute(String prefix, AutoLoadBehavior autoLoadBehavior) : base(prefix)
		{
			autoLoad = autoLoadBehavior;
		}

		/// <summary>
		/// Defines the behavior the autoload feature 
		/// should use
		/// </summary>
		public AutoLoadBehavior AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

		public override object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			ARDataBinder binder = controller.Binder as ARDataBinder;

			if (binder == null)
			{
				binder = new ARDataBinder();
			}

			binder.AutoLoad = autoLoad;
			
			CompositeNode node = controller.ObtainParamsNode(From);

			object instance = binder.BindObject(parameterInfo.ParameterType, Prefix, Exclude, Allow, node);

			if (instance != null)
			{
				controller.BoundInstanceErrors[instance] = binder.ErrorList;
			}

			return instance;
		}
	}
}
