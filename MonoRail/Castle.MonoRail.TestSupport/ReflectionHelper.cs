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

namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Reflection;

	public static class ReflectionHelper
	{
		public static object RunInstanceMethod<ObjectType>(ObjectType objectInstance, string methodName)
		{
			return RunInstanceMethod(typeof(ObjectType), objectInstance, methodName);
		}

		public static object RunInstanceMethod(Type objectType, object objectInstance, string methodName)
		{
			object[] methodParameters = new object[0];
			return RunInstanceMethod(objectType, objectInstance, methodName, ref methodParameters, BindingFlags.Default);
		}

		public static object RunInstanceMethod<ObjectType>(ObjectType objectInstance, string methodName, ref object[] methodParameters)
		{
			return RunInstanceMethod(typeof(ObjectType), objectInstance, methodName, ref methodParameters);
		}

		public static object RunInstanceMethod(Type objectType, object objectInstance, string methodName, ref object[] methodParameters)
		{
			return RunInstanceMethod(objectType, objectInstance, methodName, ref methodParameters, BindingFlags.Default);
		}

		public static object RunInstanceMethod<ObjectType>(ObjectType objectInstance, string methodName, ref object[] methodParameters, BindingFlags extraFlags)
		{
			return RunInstanceMethod(typeof(ObjectType), objectInstance, methodName, ref methodParameters, extraFlags);
		}

		public static object RunInstanceMethod(Type objectType, object objectInstance, string methodName, ref object[] methodParameters, BindingFlags extraFlags)
		{
			BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | extraFlags;
			return RunMethod(objectType, objectInstance, methodName, ref methodParameters, eFlags);
		}

		private static object RunMethod(Type objectType, object objectInstance, string methodName, ref object[] methodParameters, BindingFlags bindingFlags)
		{
			try
			{
				MethodInfo methodInfo = objectType.GetMethod(methodName, bindingFlags);

				if (methodInfo == null)
					throw new ArgumentException("There is no method '" + methodName + "' for type '" + objectType + "'.");

				object returnValue = methodInfo.Invoke(objectInstance, methodParameters);

				return returnValue;
			}
			catch (TargetInvocationException ex)
			{
				throw (ex.InnerException != null) ? ex.InnerException : ex;
			}
		}

		public static object RunStaticMethod(Type objectType, string methodName)
		{
			object[] methodParameters = new object[0];
			return RunStaticMethod(objectType, methodName, ref methodParameters, BindingFlags.Default);
		}

		public static object RunStaticMethod(Type objectType, string methodName, ref object[] methodParameters)
		{
			return RunStaticMethod(objectType, methodName, ref methodParameters, BindingFlags.Default);
		}

		public static object RunStaticMethod(Type objectType, string methodName, ref object[] methodParameters, BindingFlags extraFlags)
		{
			BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | extraFlags;
			return RunMethod(objectType, null, methodName, ref methodParameters, bindingFlags);
		}

		public static ReturnType GetField<ReturnType, ObjectType>(ObjectType objectInstance, string fieldName)
		{
			FieldInfo fieldInfo = typeof(ObjectType).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return (ReturnType)fieldInfo.GetValue(objectInstance);
		}

		public static void SetField<ObjectType>(ObjectType objectInstance, string fieldName, object fieldValue)
		{
			SetField(typeof(ObjectType), objectInstance, fieldName, fieldValue);
		}

		public static void SetField(Type objectType, object objectInstance, string fieldName, object fieldValue)
		{
			FieldInfo fieldInfo = objectType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			fieldInfo.SetValue(objectInstance, fieldValue);
		}
	}
}
