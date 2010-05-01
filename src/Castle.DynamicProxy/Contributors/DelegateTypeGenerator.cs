namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class DelegateTypeGenerator : IGenerator<AbstractTypeEmitter>
	{
		private readonly Type targetType;
		private readonly MetaMethod method;

		private const TypeAttributes DelegateFlags = TypeAttributes.Class |
		                                             TypeAttributes.Public |
		                                             TypeAttributes.Sealed |
		                                             TypeAttributes.AnsiClass |
		                                             TypeAttributes.AutoClass;

		public DelegateTypeGenerator(Type targetType, MetaMethod method)
		{
			this.targetType = targetType;
			this.method = method;
		}

		public AbstractTypeEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var emitter = GetEmitter(@class, namingScope);
			BuildConstructor(emitter);
			BuildInvokeMethod(emitter);
			return emitter;
		}

		private void BuildInvokeMethod(AbstractTypeEmitter emitter)
		{
			var paramTypes = GetParamTypes();
			var invoke = emitter.CreateMethod("Invoke",
			                                  MethodAttributes.Public |
			                                  MethodAttributes.HideBySig |
			                                  MethodAttributes.NewSlot |
			                                  MethodAttributes.Virtual,
			                                  method.MethodOnTarget.ReturnType,
			                                  paramTypes);
			invoke.MethodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
		}

		private ArgumentReference[] GetParamTypes()
		{
			var parameters = method.MethodOnTarget.GetParameters();
			var paramTypes = new ArgumentReference[parameters.Length + 1];
			paramTypes[0] = new ArgumentReference(targetType);
			for (var i = 0; i < parameters.Length; i++)
			{
				paramTypes[i + 1] = new ArgumentReference(parameters[i].ParameterType);
			}
			return paramTypes;
		}

		private void BuildConstructor(AbstractTypeEmitter emitter)
		{
			var constructor = emitter.CreateConstructor(new ArgumentReference(typeof(object)), new ArgumentReference(typeof(IntPtr)));
			constructor.ConstructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
		}

		private AbstractTypeEmitter GetEmitter(ClassEmitter @class, INamingScope namingScope)
		{
			var methodInfo = method.MethodOnTarget;
			var suggestedName = string.Format("Castle.Proxies.Delegates.{0}_{1}",
			                                  methodInfo.DeclaringType.Name,
			                                  method.Method.Name);
			var uniqueName = namingScope.ParentScope.GetUniqueName(suggestedName);

			return new ClassEmitter(@class.ModuleScope, uniqueName, typeof(MulticastDelegate), Type.EmptyTypes, DelegateFlags, false);
		}
	}
}