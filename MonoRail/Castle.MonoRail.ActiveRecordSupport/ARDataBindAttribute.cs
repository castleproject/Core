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
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.Components.Binder;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Extends <see cref="DataBindAttribute"/> with 
	/// ActiveRecord specific functionallity
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter), Serializable]
	public class ARDataBindAttribute : DataBindAttribute, IParameterBinder
	{
		private bool validate;
		private bool autoPersist;
		private bool autoLoad;
		
		public ARDataBindAttribute(String prefix) : base (prefix)
		{
		}

		public bool AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

		/// <summary>
		/// When true performs automatic validation of any class
		/// that inherit from <see cref="Castle.ActiveRecord.ActiveRecordValidationBase"/>
		/// </summary>
		public bool Validate
		{
			get { return validate; }
			set { validate = value; }
		}	
		
		/// <summary>
		/// When true automatically saves any record
		/// that inherit from <see cref="Castle.ActiveRecord.ActiveRecordBase"/>
		/// </summary>
		public bool AutoPersist
		{
			get { return autoPersist; }
			set { autoPersist = value; }
		}

		public override object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			ARDataBinder binder = new ARDataBinder();

			NameValueCollection coll = ResolveParams(controller);

			ConfigureBinder(binder, controller);

			binder.AutoLoad = AutoLoad;
			binder.PersistChanges = autoPersist;
			binder.Validate = validate;

			return binder.BindObject(parameterInfo.ParameterType, new NameValueCollectionAdapter(coll));
		}
	}
}
