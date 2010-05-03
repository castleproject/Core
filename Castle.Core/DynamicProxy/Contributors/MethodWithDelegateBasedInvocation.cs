namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Xml.Serialization;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class MethodWithDelegateBasedInvocation : MethodGenerator
	{
		private readonly Reference interceptors;
		private readonly GetTargetExpressionDelegate getTargetExpression;
		private readonly Type invocation;
		private readonly Type @delegate;

		public MethodWithDelegateBasedInvocation(MetaMethod method, Reference interceptors, Type invocation, GetTargetExpressionDelegate getTargetExpression, OverrideMethodDelegate createMethod, Type @delegate)
			: base(method, createMethod)
		{
			this.invocation = invocation;
			this.getTargetExpression = getTargetExpression;
			this.interceptors = interceptors;
			this.@delegate = @delegate;
		}

		protected override MethodEmitter BuildProxiedMethodBody(MethodEmitter emitter, ClassEmitter @class, ProxyGenerationOptions options, INamingScope namingScope)
		{
			var invocationType = invocation;

			Trace.Assert(MethodToOverride.IsGenericMethod == invocationType.IsGenericTypeDefinition);
			var genericArguments = Type.EmptyTypes;

			var constructor = invocation.GetConstructors()[0];


			Expression proxiedMethodTokenExpression;
			if (MethodToOverride.IsGenericMethod)
			{
				// bind generic method arguments to invocation's type arguments
				genericArguments = emitter.MethodBuilder.GetGenericArguments();
				invocationType = invocationType.MakeGenericType(genericArguments);
				constructor = TypeBuilder.GetConstructor(invocationType, constructor);

				// Not in the cache: generic method
				proxiedMethodTokenExpression = new MethodTokenExpression(MethodToOverride.MakeGenericMethod(genericArguments));
			}
			else
			{
				var proxiedMethodToken = @class.CreateStaticField(namingScope.GetUniqueName("token_" + MethodToOverride.Name),
																  typeof(MethodInfo));
				@class.ClassConstructor.CodeBuilder.AddStatement(new AssignStatement(proxiedMethodToken,
				                                                                     new MethodTokenExpression(MethodToOverride)));

				proxiedMethodTokenExpression = proxiedMethodToken.ToExpression();
			}

			FieldReference delegateToken = null;
			if (@delegate.IsGenericType == false)
			{
				delegateToken = BuildDelegateToken(@class, new MethodTokenExpression(MethodOnTarget), namingScope);
			}
			var dereferencedArguments = IndirectReference.WrapIfByRef(emitter.Arguments);
			var ctorArguments = GetCtorArguments(@class, namingScope, proxiedMethodTokenExpression, dereferencedArguments,
			                                     delegateToken);

			var invocationLocal = emitter.CodeBuilder.DeclareLocal(invocationType);
			emitter.CodeBuilder.AddStatement(new AssignStatement(invocationLocal,
																 new NewInstanceExpression(constructor, ctorArguments)));

			if (MethodToOverride.ContainsGenericParameters)
			{
				EmitLoadGenricMethodArguments(emitter, MethodToOverride.MakeGenericMethod(genericArguments), invocationLocal);
			}

			var proceed = new ExpressionStatement(new MethodInvocationExpression(invocationLocal, InvocationMethods.Proceed));
			emitter.CodeBuilder.AddStatement(proceed);

			GeneratorUtil.CopyOutAndRefParameters(dereferencedArguments, invocationLocal, MethodToOverride, emitter);

			if (MethodToOverride.ReturnType != typeof(void))
			{
				// Emit code to return with cast from ReturnValue
				var getRetVal = new MethodInvocationExpression(invocationLocal, InvocationMethods.GetReturnValue);
				emitter.CodeBuilder.AddStatement(new ReturnStatement(new ConvertExpression(emitter.ReturnType, getRetVal)));
			}
			else
			{
				emitter.CodeBuilder.AddStatement(new ReturnStatement());
			}

			return emitter;
		}

		private FieldReference BuildDelegateToken(ClassEmitter @class, Expression method, INamingScope namingScope)
		{
			var callback = @class.CreateStaticField(namingScope.GetUniqueName("callback_" + MethodToOverride.Name), @delegate);
			var createDelegate = new MethodInvocationExpression(
				null,
				DelegateMethods.CreateDelegate,
				new TypeTokenExpression(@delegate),
				NullExpression.Instance,
				method);
			var bindDelegate = new AssignStatement(callback, new ConvertExpression(@delegate, createDelegate));

			@class.ClassConstructor.CodeBuilder.AddStatement(bindDelegate);
			return callback;
		}

		private Expression[] GetCtorArguments(ClassEmitter @class, INamingScope namingScope, Expression proxiedMethodTokenExpression, TypeReference[] dereferencedArguments, FieldReference delegateToken)
		{
			var selector = @class.GetField("__selector");
			if (selector != null)
			{
				if (delegateToken != null)
				{
					return new[]
					{
						delegateToken.ToExpression(),
						getTargetExpression(@class, MethodToOverride),
						SelfReference.Self.ToExpression(),
						interceptors.ToExpression(),
						proxiedMethodTokenExpression,
						new ReferencesToObjectArrayExpression(dereferencedArguments),
						selector.ToExpression(),
						new AddressOfReferenceExpression(BuildMethodInterceptorsField(@class, MethodToOverride, namingScope))
					};
				}
				return new[]
				{
					getTargetExpression(@class, MethodToOverride),
					SelfReference.Self.ToExpression(),
					interceptors.ToExpression(),
					proxiedMethodTokenExpression,
					new ReferencesToObjectArrayExpression(dereferencedArguments),
					selector.ToExpression(),
					new AddressOfReferenceExpression(BuildMethodInterceptorsField(@class, MethodToOverride, namingScope))
				};
			}
			if (delegateToken != null)
			{
				return new[]
				{
					delegateToken.ToExpression(),
					getTargetExpression(@class, MethodToOverride),
					SelfReference.Self.ToExpression(),
					interceptors.ToExpression(),
					proxiedMethodTokenExpression,
					new ReferencesToObjectArrayExpression(dereferencedArguments)
				};
			}
			return new[]
			{
				getTargetExpression(@class, MethodToOverride),
				SelfReference.Self.ToExpression(),
				interceptors.ToExpression(),
				proxiedMethodTokenExpression,
				new ReferencesToObjectArrayExpression(dereferencedArguments)
			};
		}

		protected FieldReference BuildMethodInterceptorsField(ClassEmitter @class, MethodInfo method, INamingScope namingScope)
		{
			var methodInterceptors = @class.CreateField(
				namingScope.GetUniqueName(string.Format("interceptors_{0}", method.Name)),
				typeof(IInterceptor[]),
				false);
#if !SILVERLIGHT
			@class.DefineCustomAttributeFor<XmlIgnoreAttribute>(methodInterceptors);
#endif
			return methodInterceptors;
		}

		private void EmitLoadGenricMethodArguments(MethodEmitter methodEmitter, MethodInfo method, Reference invocationLocal)
		{
#if SILVERLIGHT
			Type[] genericParameters =
				Castle.Core.Extensions.SilverlightExtensions.FindAll(method.GetGenericArguments(), t => t.IsGenericParameter);
#else
			Type[] genericParameters = Array.FindAll(method.GetGenericArguments(), t => t.IsGenericParameter);
#endif
			LocalReference genericParamsArrayLocal = methodEmitter.CodeBuilder.DeclareLocal(typeof(Type[]));
			methodEmitter.CodeBuilder.AddStatement(
				new AssignStatement(genericParamsArrayLocal, new NewArrayExpression(genericParameters.Length, typeof(Type))));

			for (int i = 0; i < genericParameters.Length; ++i)
			{
				methodEmitter.CodeBuilder.AddStatement(
					new AssignArrayStatement(genericParamsArrayLocal, i, new TypeTokenExpression(genericParameters[i])));
			}
			methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
													new MethodInvocationExpression(invocationLocal,
																				   InvocationMethods.SetGenericMethodArguments,
																				   new ReferenceExpression(
																					genericParamsArrayLocal))));
		}
	}
}