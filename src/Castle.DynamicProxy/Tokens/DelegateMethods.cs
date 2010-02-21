namespace Castle.DynamicProxy.Tokens
{
	using System;
	using System.Reflection;

	public static class DelegateMethods
	{
		public static readonly MethodInfo CreateDelegate =
			typeof(Delegate).GetMethod("CreateDelegate", new[] { typeof(Type), typeof(object), typeof(MethodInfo) });
	}
}