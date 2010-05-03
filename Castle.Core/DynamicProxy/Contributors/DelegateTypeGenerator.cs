namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class DelegateTypeGenerator : IGenerator<AbstractTypeEmitter>
	{
		private readonly MetaMethod method;
		private readonly Type targetType;

		private const TypeAttributes DelegateFlags = TypeAttributes.Class |
		                                             TypeAttributes.Public |
		                                             TypeAttributes.Sealed |
		                                             TypeAttributes.AnsiClass |
		                                             TypeAttributes.AutoClass;

		public DelegateTypeGenerator(MetaMethod method, Type targetType)
		{
			this.method = method;
			this.targetType = targetType;
		}

		public AbstractTypeEmitter Generate(ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var emitter = GetEmitter(@class, namingScope);
			BuildConstructor(emitter);
			BuildInvokeMethod(emitter);
			return emitter;
		}

		private void BuildInvokeMethod(AbstractTypeEmitter @delegate)
		{
			var paramTypes = GetParamTypes(@delegate);
			var invoke = @delegate.CreateMethod("Invoke",
			                                    MethodAttributes.Public |
			                                    MethodAttributes.HideBySig |
			                                    MethodAttributes.NewSlot |
			                                    MethodAttributes.Virtual,
			                                    TypeUtil.GetClosedParameterType(@delegate, method.MethodOnTarget.ReturnType),
			                                    paramTypes);
			invoke.MethodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
		}

		private Type[] GetParamTypes(AbstractTypeEmitter @delegate)
		{
			var parameters = method.MethodOnTarget.GetParameters();
			if(@delegate.TypeBuilder.IsGenericType)
			{
				var types = new Type[parameters.Length];
				
				for (var i = 0; i < parameters.Length; i++)
				{
					types[i] = TypeUtil.GetClosedParameterType(@delegate, parameters[i].ParameterType);
				}
				return types;
				
			}
			var paramTypes = new Type[parameters.Length + 1];
			paramTypes[0] = targetType;
			for (var i = 0; i < parameters.Length; i++)
			{
				paramTypes[i + 1] = TypeUtil.GetClosedParameterType(@delegate, parameters[i].ParameterType);
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

			var @delegate = new ClassEmitter(@class.ModuleScope,
			                                    uniqueName,
			                                    typeof(MulticastDelegate),
			                                    Type.EmptyTypes,
			                                    DelegateFlags,
			                                    false);
			@delegate.CopyGenericParametersFromMethod(method.Method);
			return @delegate;
		}
	}
}