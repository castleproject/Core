namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class InheritanceInvocationWithDelegateTypeGenerator : InvocationTypeGenerator
	{
		private readonly Type delegateType;
		public static readonly Type BaseType = typeof(InheritanceInvocation);

		public InheritanceInvocationWithDelegateTypeGenerator(Type targetType, MetaMethod method, Type delegateType)
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

		protected override void CustomizeCtor(ConstructorEmitter constructor, ArgumentReference[] arguments, AbstractTypeEmitter invocation)
		{
			if(delegateType.IsGenericType)
			{
				// we don't get the delegate from outside, since we can't cache it anyway.
				// we'll bind to it lazily in InvokeMethodOnTarget method
				return;
			}
			var delegateField = invocation.CreateField("delegate", delegateType);
			constructor.CodeBuilder.AddStatement(new AssignStatement(delegateField, new ReferenceExpression(arguments[0])));
		}

		protected override ArgumentReference[] GetArgumentsForBaseCtor(ArgumentReference[] arguments)
		{
			if (delegateType.IsGenericType)
			{
				// we don't get the delegate from outside, since we can't cache it anyway.
				// we'll bind to it lazily in InvokeMethodOnTarget method
				return base.GetArgumentsForBaseCtor(arguments);
			}
			Debug.Assert(arguments.Length > 1, "arguments.Length > 1");

			var baseArguments = new ArgumentReference[arguments.Length - 1];
			Array.Copy(arguments, 1, baseArguments, 0, arguments.Length - 1);
			return baseArguments;
		}

		protected override MethodInfo GetCallbackMethod(MethodInfo callbackMethod, AbstractTypeEmitter @class)
		{
			var callback = delegateType.GetMethod("Invoke");
			return callback;
		}

		protected override MethodInvocationExpression GetCallbackMethodInvocation(AbstractTypeEmitter invocation, Expression[] args, MethodInfo callbackMethod, Reference targetField, MethodEmitter invokeMethodOnTarget)
		{
			var allArgs = GetAllArgs(args, targetField);
			var @delegate = GetDelegate(invocation, invokeMethodOnTarget);
			return new MethodInvocationExpression(@delegate,
			                                      callbackMethod,
			                                      allArgs);
		}

		private Expression[] GetAllArgs(Expression[] args, Reference targetField)
		{
			if(delegateType.IsGenericType)
			{
				return args;
			}
			var allArgs = new Expression[args.Length + 1];
			args.CopyTo(allArgs, 1);
			allArgs[0] = new ConvertExpression(targetType, targetField.ToExpression());
			return allArgs;
		}

		private Reference GetDelegate(AbstractTypeEmitter invocation, MethodEmitter invokeMethodOnTarget)
		{
			if(delegateType.IsGenericType == false)
			{
				return invocation.GetField("delegate");
			}
			var closedDelegateType = delegateType.MakeGenericType(invocation.GenericTypeParams);
			var localReference = invokeMethodOnTarget.CodeBuilder.DeclareLocal(closedDelegateType);
			var closedMethodOnTarget = method.MethodOnTarget.MakeGenericMethod(invocation.GenericTypeParams);
			var localTarget = new ReferenceExpression(GetTargetReference());
			invokeMethodOnTarget.CodeBuilder.AddStatement(
				SetDelegate(localReference, localTarget, closedDelegateType, closedMethodOnTarget));
			return localReference;
		}

		private AssignStatement SetDelegate(LocalReference localDelegate, ReferenceExpression localTarget, Type closedDelegateType, MethodInfo closedMethodOnTarget)
		{
			var delegateCreateDelegate = new MethodInvocationExpression(
				null,
				DelegateMethods.CreateDelegate,
				new TypeTokenExpression(closedDelegateType),
				localTarget,
				new MethodTokenExpression(closedMethodOnTarget));
			return new AssignStatement(localDelegate, new ConvertExpression(closedDelegateType, delegateCreateDelegate));
		}

		protected override ArgumentReference[] GetCtorArgumentsAndBaseCtorToCall(Type targetFieldType, ProxyGenerationOptions proxyGenerationOptions, out ConstructorInfo baseConstructor)
		{
			if (proxyGenerationOptions.Selector == null)
			{
				baseConstructor = InvocationMethods.InheritanceInvocationConstructorNoSelector;
				if (delegateType.IsGenericType)
				{
					return new[]
					{
						new ArgumentReference(typeof(Type)),
						new ArgumentReference(typeof(object)),
						new ArgumentReference(typeof(IInterceptor[])),
						new ArgumentReference(typeof(MethodInfo)),
						new ArgumentReference(typeof(object[]))
					};
				}
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
			if (delegateType.IsGenericType)
			{
				return new[]
				{
					new ArgumentReference(typeof(Type)),
					new ArgumentReference(typeof(object)),
					new ArgumentReference(typeof(IInterceptor[])),
					new ArgumentReference(typeof(MethodInfo)),
					new ArgumentReference(typeof(object[])),
					new ArgumentReference(typeof(IInterceptorSelector)),
					new ArgumentReference(typeof(IInterceptor[]).MakeByRefType())
				};
			}
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