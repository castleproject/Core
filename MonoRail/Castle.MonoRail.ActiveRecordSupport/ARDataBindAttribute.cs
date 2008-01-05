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
		/// Means that no autoload should be performed on the target
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
		/// Means that we should autoload target and nested types when the key is valid.
		/// If the key is invalid, like <c>null</c>, 0 or an empty string, and the
		/// instance is the root instance, then create a new instance of the target type.
		/// If the key is invalid, and it's a nested instance, then set null on the nested type.
		/// </summary>
		NewRootInstanceIfInvalidKey,

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
	public class ARDataBindAttribute : DataBindAttribute
	{
		private AutoLoadBehavior autoLoad = AutoLoadBehavior.Never;
		private string expect = null;

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

		/// <summary>
		/// Gets or sets the names of the collection that are expected to be binded.
		/// If the binder does not find any value to an expected collection, it will clear to collection.
		/// </summary>
		/// <value>The expect collections names, in a csv fashion.</value>
		public string Expect
		{
			get { return expect; }
			set { expect = value; }
		}

		/// <summary>
		/// Implementation of <see cref="IParameterBinder.Bind"/>
		/// and it is used to read the data available and construct the
		/// parameter type accordingly.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info</param>
		/// <returns>The bound instance</returns>
		public override object Bind(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo)
		{
			ARDataBinder binder = (ARDataBinder) CreateBinder();
			IValidatorAccessor validatorAccessor = controller as IValidatorAccessor;

			ConfigureValidator(validatorAccessor, binder);

			binder.AutoLoad = autoLoad;

			CompositeNode node = context.Request.ObtainParamsNode(From);

			object instance = binder.BindObject(parameterInfo.ParameterType, Prefix, Exclude, Allow, Expect, node);

			BindInstanceErrors(validatorAccessor, binder, instance);
			PopulateValidatorErrorSummary(validatorAccessor, binder, instance);

			return instance;
		}

		protected override IDataBinder CreateBinder()
		{
			return new ARDataBinder();
		}
	}
}
