namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;
	using Contributors;

	public abstract class ProxyMethod : IProxyMethod
	{
		private readonly MethodInfo method;
		private readonly ITypeContributor target;

		protected ProxyMethod(MethodInfo method, ITypeContributor target)
		{
			this.method = method;
			this.target = target;
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public bool HasTarget
		{
			get
			{
				return target != null;
			}
		}

		public abstract MethodInfo MethodOnTarget { get; }
	}
}