namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;
	using Generators;

	public class InterfaceMembersOnClassCollector : MembersCollector
	{
		public InterfaceMembersOnClassCollector(Type type, ITypeContributor contributor, bool onlyProxyVirtual, InterfaceMapping map) : base(type, contributor, onlyProxyVirtual, map)
		{
		}

		protected override MethodToGenerate GetMethodToGenerate(MethodInfo method, IProxyGenerationHook hook, bool isStandalone)
		{
			// This is here, so that we don't add multiple times property getters/setters and event add/remove methods
			if (!IsAccessible(method))
			{
				return null;
			}

			if (onlyProxyVirtual && IsVirtuallyImplementedInterfaceMethod(method))
			{
				return null;
			}

			var methodOnTarget = GetMethodOnTarget(method);
			ITypeContributor target;
			if (methodOnTarget.IsPrivate)//explicitly implemented
			{
				target = null;
			}
			else
			{
				target = contributor;
			}

			var proxyable = AcceptMethod(method, onlyProxyVirtual, hook);
			return new MethodToGenerate(method, isStandalone, target, methodOnTarget, proxyable);
		}

		private bool IsVirtuallyImplementedInterfaceMethod(MethodInfo method)
		{
			var index = Array.IndexOf(map.InterfaceMethods, method);
			return index != -1 && map.TargetMethods[index].IsFinal == false;
		}
	}
}