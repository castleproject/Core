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

namespace Castle.Monorail.JSONSupport
{
	using System;
	using System.Reflection;
	using Castle.MonoRail.Framework;
	using Newtonsoft.Json;

	/// <summary>
	/// Extends <see cref="DataBindAttribute"/> with 
	/// the <see cref="JavaScriptConvert"/> functionality.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class JSONBinderAttribute : Attribute, IParameterBinder
	{
		private readonly string entryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSONBinderAttribute"/> class.
		/// </summary>
		/// <param name="entryKey">The entry key, which is the form or 
		/// querystring key that identifies the JSON persisted content</param>
		public JSONBinderAttribute(string entryKey)
		{
			if (entryKey == null) throw new ArgumentNullException("entryKey");
			
			this.entryKey = entryKey;
		}

		public int CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			return controller.Params[entryKey] != null ? 1 : 0; 
		}

		public object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			string entryValue = controller.Params[entryKey];

			return Bind(entryValue, parameterInfo.ParameterType);
		}

		public static object Bind(string entryValue, Type parameterType)
		{
			return JavaScriptConvert.DeserializeObject(entryValue, parameterType);
		}
	}
}
