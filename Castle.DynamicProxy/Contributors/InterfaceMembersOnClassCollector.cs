namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;
	using Generators;

	public class InterfaceMembersOnClassCollector : MembersCollector
	{
		private readonly bool onlyProxyVirtual;
		private readonly InterfaceMapping map;

		public InterfaceMembersOnClassCollector(Type type, bool onlyProxyVirtual, InterfaceMapping map) : base(type)
		{
			this.onlyProxyVirtual = onlyProxyVirtual;
			this.map = map;
		}

		protected override MetaMethod GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			if (!IsAccessible(method))
			{
				return null;
			}

			if (onlyProxyVirtual && IsVirtuallyImplementedInterfaceMethod(method))
			{
				return null;
			}

			var methodOnTarget = GetMethodOnTarget(method);

			var proxyable = AcceptMethod(method, onlyProxyVirtual, hook);
			return new MetaMethod(method, methodOnTarget, isStandalone, proxyable, methodOnTarget.IsPrivate == false);
		}

		private MethodInfo GetMethodOnTarget(MethodInfo method)
		{
			int index = Array.IndexOf(map.InterfaceMethods, method);
			if (index == -1)
			{
				return null;
			}

			return map.TargetMethods[index];
		}

		private bool IsVirtuallyImplementedInterfaceMethod(MethodInfo method)
		{
			var info = GetMethodOnTarget(method);
			return info != null && info.IsFinal == false;
		}
	}
}