// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration.Util
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Utility classes for NHibernate. Contains methods to get properties of an entity etc.
	/// </summary>
	public class ReflectionUtility
	{
		/// <summary>
		/// Gets the readable (non indexed) properties names and values.
		/// The keys holds the names of the properties.
		/// The values are the values of the properties
		/// </summary>
		public static IDictionary<string,object> GetPropertiesDictionary(object obj)
		{
			IDictionary<string,object> ht =new Dictionary<string, object>();
			foreach (PropertyInfo property in obj.GetType().
				GetProperties(BindingFlags.Instance |
				              BindingFlags.GetProperty | BindingFlags.Public |
				              BindingFlags.NonPublic))
			{
				if (property.CanRead && property.GetIndexParameters().Length == 0)
				{
					ht[property.Name] = property.GetValue(obj, null);
				}
			}
			return ht;
		}


		/// <summary>
		/// Determines whether type is simple enough to need just ToString()
		/// to show its state.
		/// (string,int, bool, enums are simple.
		/// Anything else is false.
		/// </summary>
		public static bool IsSimpleType(Type type)
		{
			if (type.IsEnum || type.IsPrimitive || type == typeof(string)
			    || type == typeof(DateTime))
				return true;
			else
				return false;
		}

	}
}