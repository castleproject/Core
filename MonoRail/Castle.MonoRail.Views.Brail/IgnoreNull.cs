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

namespace Castle.MonoRail.Views.Brail
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using Boo.Lang;

	public class IgnoreNull : IQuackFu
	{
		private readonly object target;

		public IgnoreNull(object target)
		{
			this.target = target;
		}

		#region IQuackFu Members

		public object QuackGet(string name, object[] parameters)
		{
			if (name == "_IsIgnoreNullReferencingNotNullObject_")
			{
				return target != null;
			}

			if (target == null)
			{
				return this;
			}
			object value;
			if (IsNullOrEmpty(parameters))
			{
				value = ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.GetProperty(target, name);
			}
			else
			{
				value = ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.GetSlice(target, name, parameters);
			}
			return new IgnoreNull(value);
		}

		public object QuackSet(string name, object[] parameters, object obj)
		{
			if (target == null)
			{
				return this;
			}
			if (IsNullOrEmpty(parameters))
			{
				ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.SetProperty(target, name, obj);
			}
			else
			{
				ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.SetSlice(target, name,
				                                                                         GetParameterArray(parameters, obj));
			}
			return this;
		}

		public object QuackInvoke(string name, object[] args)
		{
			if (target == null)
			{
				return this;
			}
			object value = ExpandDuckTypedExpressions_WorkaroundForDuplicateVirtualMethods.Invoke(target, name, args);
			return new IgnoreNull(value);
		}

		#endregion

		private static bool IsNullOrEmpty(object[] parameters)
		{
			return parameters == null || parameters.Length == 0;
		}

		private static object[] GetParameterArray(object[] parameters, object obj)
		{
			List<object> args = new List<object>(parameters);
			args.Add(obj);
			return args.ToArray();
		}

		public override string ToString()
		{
			if (target == null)
			{
				return string.Empty;
			}
			return target.ToString();
		}

		public static bool AreEqual(object left, object right)
		{
			IgnoreNull temp = left as IgnoreNull;
			if (temp != null)
			{
				left = temp.target;
			}
			temp = right as IgnoreNull;
			if (temp != null)
			{
				right = temp.target;
			}
			return Equals(left, right);
		}

		/// <summary>
		/// Gets the underlying target object of the IgnorNull
		/// </summary>
		/// <param name="ignored"></param>
		/// <returns>target</returns>
		public static Object ExtractTarget(IgnoreNull ignored)
		{
			return ignored.target;
		}

		/// <summary>
		/// Returns a new array copied from args that has replaced IgnoreNull instances
		/// with the underlying target object.
		/// </summary>
		/// <param name="args">source array</param>
		/// <returns>new array</returns>
		public static Object[] ReplaceIgnoreNullsWithTargets(Object[] args)
		{
			Object[] replaced = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] is IgnoreNull)
				{
					replaced[i] = ExtractTarget((IgnoreNull) args[i]);
				}
				else
				{
					replaced[i] = args[i];
				}
			}
			return replaced;
		}

		/// <summary>
		/// Replace all the values in the Hashtable that are marked as 
		/// replaced values of IgnoreNull with the underlying target object.
		/// </summary>
		/// <param name="dict"></param>
		/// <returns>The original hashtable, with all the IgnoreNull values stripped to their targets</returns>
		public static IDictionary ReplaceIgnoreNullsWithTargets(IDictionary dict)
		{
			Hashtable modificationSafeCopy = new Hashtable(dict);
			foreach (DictionaryEntry de in modificationSafeCopy)
			{
				if (de.Value is IgnoreNull)
				{
					dict[de.Key] = ExtractTarget((IgnoreNull)de.Value);
				}
			}
			return dict;
		}
	}
}