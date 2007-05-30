#if DOTNET2
namespace Castle.MonoRail.TestSupport
{
	using System;
	using System.Reflection;

	public static class ReflectionHelper
	{
		public static object RunInstanceMethod(Type objectType, object objectInstance, string methodName)
		{
			object[] methodParameters = new object[0];
			return RunInstanceMethod(objectType, objectInstance, methodName, ref methodParameters, BindingFlags.Default);
		}

		public static object RunInstanceMethod(Type objectType, object objectInstance, string methodName, ref object[] methodParameters)
		{
			return RunInstanceMethod(objectType, objectInstance, methodName, ref methodParameters, BindingFlags.Default);
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

		public static void SetField(Type objectType, object objectInstance, string fieldName, object fieldValue)
		{
			FieldInfo fieldInfo = objectType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			fieldInfo.SetValue(objectInstance, fieldValue);
		}
	}
}
#endif