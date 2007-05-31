namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Reflection;
	using NUnit.Framework;
#if DOTNET2
	public class GenericTestUtility
	{
		public static void CheckMethodInfoIsClosed(MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			Assert.IsFalse(method.ContainsGenericParameters);
			Assert.AreEqual(returnType, method.ReturnType);

			ParameterInfo[] parameters = method.GetParameters();
			Assert.AreEqual(parameterTypes.Length, parameters.Length);
			for(int i = 0; i < parameterTypes.Length; ++i)
			{
				Assert.AreEqual(parameterTypes[i], parameters[i].ParameterType);
			}
		}
	}
#endif
}