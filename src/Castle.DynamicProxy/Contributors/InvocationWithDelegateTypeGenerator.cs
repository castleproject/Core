namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Diagnostics;
	using System.Reflection;

	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class InvocationWithDelegateTypeGenerator : InvocationTypeGenerator
	{
		private readonly Type delegateType;
		public static readonly Type BaseType = typeof(InheritanceInvocation);
		public InvocationWithDelegateTypeGenerator(Type targetType, MetaMethod method, Type delegateType)
			: base(targetType, method, null, false)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			this.delegateType = delegateType;
		}

		protected override FieldReference GetTargetReference()
		{
			return new FieldReference(InvocationMethods.ProxyObject);
		}

		protected override Type GetBaseType()
		{
			return BaseType;
		}

		protected override void CustomizeCtor(ConstructorEmitter constructor, ArgumentReference[] arguments, AbstractTypeEmitter @class)
		{
			var delegateField = @class.CreateField("delegate", delegateType);
			constructor.CodeBuilder.AddStatement(new AssignStatement(delegateField, new ReferenceExpression(arguments[0])));
		}

		protected override ArgumentReference[] GetArgumentsForBaseCtor(ArgumentReference[] arguments)
		{
			Debug.Assert(arguments.Length > 1, "arguments.Length > 1");

			var baseArguments = new ArgumentReference[arguments.Length - 1];
			Array.Copy(arguments, 1, baseArguments, 0, arguments.Length - 1);
			return baseArguments;
		}

		protected override MethodInfo GetCallbackMethod(MethodInfo callbackMethod, AbstractTypeEmitter @class)
		{
			var callback = delegateType.GetMethod("Invoke");
			if (callback.IsGenericMethod)
			{
				throw new NotSupportedException("Generic delegates are not supported yet.");
			}
			return callback;
		}

		protected override MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter @class, Expression[] args, MethodInfo callbackMethod, Reference targetField)
		{
			var allArgs = new Expression[args.Length + 1];
			args.CopyTo(allArgs, 1);
			allArgs[0] = new ConvertExpression(targetType, targetField.ToExpression());

			var @delegate = @class.GetField("delegate");
			var methodOnTargetInvocationExpression = new MethodInvocationExpression(
				@delegate,
				callbackMethod,
				allArgs) { VirtualCall = true };
			return methodOnTargetInvocationExpression;
		}

		protected override ArgumentReference[] GetCtorArgumentsAndBaseCtorToCall(Type targetFieldType, ProxyGenerationOptions proxyGenerationOptions, out ConstructorInfo baseConstructor)
		{
			if (proxyGenerationOptions.Selector == null)
			{
				baseConstructor = InvocationMethods.InheritanceInvocationConstructorNoSelector;
				return new[]
				{
					new ArgumentReference(delegateType),
					new ArgumentReference(typeof(Type)),
					new ArgumentReference(typeof(object)),
					new ArgumentReference(typeof(IInterceptor[])),
					new ArgumentReference(typeof(MethodInfo)),
					new ArgumentReference(typeof(object[]))
				};
			}

			baseConstructor = InvocationMethods.InheritanceInvocationConstructorWithSelector;
			return new[]
			{
				new ArgumentReference(delegateType),
				new ArgumentReference(typeof(Type)),
				new ArgumentReference(typeof(object)),
				new ArgumentReference(typeof(IInterceptor[])),
				new ArgumentReference(typeof(MethodInfo)),
				new ArgumentReference(typeof(object[])),
				new ArgumentReference(typeof(IInterceptorSelector)),
				new ArgumentReference(typeof(IInterceptor[]).MakeByRefType())
			};
		}
	}
}