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

	using Castle.MonoRail.Framework;

	/// <summary>
	/// Extends <see cref="DataBindAttribute"/> with 
	/// ActiveRecord specific functionallity
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter), Serializable]
	public class ARDataBindAttribute : DataBindAttribute, IParameterBinder
	{
		private static readonly object NullWhenPrimaryKeyEmpty = new object();

		private bool autoLoad;
        
		private object nullWhenPrimaryKey = NullWhenPrimaryKeyEmpty;

		public ARDataBindAttribute(String prefix) : base (prefix)
		{
		}

		public bool AutoLoad
		{
			get { return autoLoad; }
			set { autoLoad = value; }
		}

        public object NullWhenPrimaryKey
        {
            get { return nullWhenPrimaryKey; }
            set { nullWhenPrimaryKey = value; }
        }

        public override object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			ARDataBinder binder = new ARDataBinder();

			ConfigureBinder(binder, controller);

			binder.AutoLoad = AutoLoad;
    
			if (IsNullWhenPrimaryKeySet)
            {
                binder.NullWhenPrimaryKey = NullWhenPrimaryKey;
            }

			return binder.BindObject(parameterInfo.ParameterType, Prefix, Exclude, Allow, ResolveParams(controller));
		}

		protected internal bool IsNullWhenPrimaryKeySet
		{
			get { return NullWhenPrimaryKey != NullWhenPrimaryKeyEmpty; }
		}
	}
}
