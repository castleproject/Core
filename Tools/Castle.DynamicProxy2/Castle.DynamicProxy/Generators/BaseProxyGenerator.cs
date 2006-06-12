// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Reflection;
	using System.Threading;
	using System.Runtime.CompilerServices;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// Base class that exposes the common functionalities
	/// to proxy generation.
	/// </summary>
	public abstract class BaseProxyGenerator
	{
		private readonly ModuleScope scope;

		protected Type baseType = typeof(Object);
		protected ClassEmitter emitter;
		protected FieldReference interceptorsField;
//		protected FieldReference interceptorSelectorField;
//		protected FieldReference useSelectorField;
		protected int nestedCounter;
		protected bool canOnlyProxyVirtuals = true;

#if DOTNET2
		private ReaderWriterLock internalsToDynProxyLock = new ReaderWriterLock();
		private System.Collections.Generic.IDictionary<Assembly, bool> internalsToDynProxy = new System.Collections.Generic.Dictionary<Assembly, bool>();
#endif
		
		protected BaseProxyGenerator(ModuleScope scope)
		{
			this.scope = scope;
		}

		protected ModuleScope Scope
		{
			get { return scope; }
		}

		protected virtual ClassEmitter BuildClassEmitter(String typeName, Type parentType, Type[] interfaces)
		{
			if (interfaces == null)
			{
				interfaces = new Type[0];
			}

			return new ClassEmitter(Scope, typeName, parentType, interfaces, true);
		}

		protected void GenerateIProxyTargetAccessor()
		{
			emitter.TypeBuilder.AddInterfaceImplementation(typeof(IProxyTargetAccessor));

			MethodEmitter methodEmitter = emitter.CreateMethod("GetTarget",
				MethodAttributes.Public | MethodAttributes.Virtual,
				new ReturnReferenceExpression(typeof(object)));

			methodEmitter.CodeBuilder.AddStatement(
				new ReturnStatement(GetProxyTargetReference()));
		}

		protected bool IsInternalToDynamicProxy(Assembly asm)
		{
#if DOTNET2
			internalsToDynProxyLock.AcquireReaderLock(-1);

			try
			{
				if (!internalsToDynProxy.ContainsKey(asm))
				{
					internalsToDynProxyLock.UpgradeToWriterLock(-1);
					InternalsVisibleToAttribute[] atts = (InternalsVisibleToAttribute[])
						asm.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);

					bool found = false;

					foreach (InternalsVisibleToAttribute internals in atts)
					{
						if (internals.AssemblyName.Contains(ModuleScope.ASSEMBLY_NAME))
						{
							found = true;
							break;
						}
					}
					internalsToDynProxy.Add(asm, found);
				}
				return internalsToDynProxy[asm];

			}
			finally
			{
				internalsToDynProxyLock.ReleaseLock();
			}
#else
			return false;
#endif
		}

		protected virtual Type CreateType()
		{
			Type newType = emitter.BuildType();

//			RegisterInCache(newType);

			return newType;
		}

		protected virtual void GenerateFields()
		{
			interceptorsField = emitter.CreateField("__interceptors", typeof(IInterceptor[]));
//			useSelectorField = emitter.CreateField("__useSelector", typeof(bool));
//			interceptorSelectorField = emitter.CreateField("__interceptorSelector", typeof(IInterceptorSelector));
		}

		/// <summary>
		/// Used by dinamically implement <see cref="IProxyTargetAccessor"/>
		/// </summary>
		/// <returns></returns>
		protected abstract Reference GetProxyTargetReference();

		// TODO: Implement this
		protected Type GetFromCache(CacheKey key)
		{
			return null;
		}

		// TODO: Implement this
		protected void AddToCache(CacheKey key, Type type)
		{
			
		}

		protected void GenerateMethods(Type targetType, Reference targetRef, IProxyGenerationHook hook, bool useSelector)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			MethodInfo[] methods = targetType.GetMethods(flags);

			foreach (MethodInfo method in methods)
			{
				bool shouldIntercept = hook.ShouldInterceptMethod(targetType, method);

				if (!shouldIntercept)
				{
					continue;
				}

				if (shouldIntercept && canOnlyProxyVirtuals && !method.IsVirtual)
				{
					hook.NonVirtualMemberNotification(baseType, method);
					continue;
				}
				
				ParameterInfo[] parametersInfo = method.GetParameters();

				Type[] parameters = new Type[parametersInfo.Length];

				for (int i = 0; i < parametersInfo.Length; i++)
				{
					parameters[i] = parametersInfo[i].ParameterType;
				}

				MethodAttributes atts = ObtainMethodAttributes(method);

				NestedClassEmitter iinvocationImpl = CreateIInvocationImplementation(targetType, method);
				// NestedClassEmitter iinvocationImpl = CreateIInvocationImplementation(baseType, method);

				MethodEmitter methodEmitter = emitter.CreateMethod(method.Name,
					atts, new ReturnReferenceExpression(method.ReturnType), parameters);

				methodEmitter.DefineParameters(parametersInfo);

				TypeReference[] dereferencedArguments = IndirectReference.WrapIfByRef(methodEmitter.Arguments);

				LocalReference invocationImplLocal =
					methodEmitter.CodeBuilder.DeclareLocal(iinvocationImpl.TypeBuilder);

				// TODO: Initialize iinvocation instance 
				// with ordinary arguments and in and out arguments

				Expression interceptors = null;

				if (useSelector)
				{
					// TODO: Generate code that checks the return of selector
					// if no interceptors is returned, should we invoke the base.Method directly?
				}
				else
				{
					interceptors = interceptorsField.ToExpression();
				}

				TypeTokenExpression typeExp = new TypeTokenExpression(baseType);
				MethodTokenExpression methodTokenExp = new MethodTokenExpression(method, targetType);

				NewInstanceExpression newInvocImpl = new NewInstanceExpression(
					iinvocationImpl.Constructors[0].Builder,
					targetRef.ToExpression(),
					interceptors,
					typeExp,
					methodTokenExp,
					new ReferencesToObjectArrayExpression(dereferencedArguments));

				methodEmitter.CodeBuilder.AddStatement(new AssignStatement(invocationImplLocal, newInvocImpl));

				methodEmitter.CodeBuilder.AddStatement(new ExpressionStatement(
					new MethodInvocationExpression(invocationImplLocal,
					Constants.AbstractInvocationProceed)));

				if (method.ReturnType != typeof(void))
				{
					// Emit code to return with cast from ReturnValue
					MethodInvocationExpression getRetVal =
						new MethodInvocationExpression(invocationImplLocal,
							typeof(AbstractInvocation).GetMethod("get_ReturnValue"));

					methodEmitter.CodeBuilder.AddStatement(new ReturnStatement(
						new ConvertExpression(method.ReturnType, getRetVal)));
				}

				methodEmitter.CodeBuilder.AddStatement(new ReturnStatement());
			}
		}

		protected NestedClassEmitter CreateIInvocationImplementation(Type targetType, MethodInfo methodInfo)
		{
			nestedCounter++;

			NestedClassEmitter nested = new NestedClassEmitter(emitter,
				"Invocation" + nestedCounter.ToString(), typeof(AbstractInvocation), new Type[0]);

#if DOTNET2
			nested.CreateGenericParameters(targetType);
#endif
			// Create the fields

			FieldReference targetField = nested.CreateField("target", targetType);

			// Create constructor

			ArgumentReference cArg0 = new ArgumentReference(targetType);
			ArgumentReference cArg1 = new ArgumentReference(typeof(IInterceptor[]));
			ArgumentReference cArg2 = new ArgumentReference(typeof(Type));
			ArgumentReference cArg3 = new ArgumentReference(typeof(MethodInfo));
			ArgumentReference cArg4 = new ArgumentReference(typeof(object[]));

			ConstructorEmitter constructor = nested.CreateConstructor(cArg0, cArg1, cArg2, cArg3, cArg4);

			constructor.CodeBuilder.AddStatement(new AssignStatement(targetField, cArg0.ToExpression()));
			constructor.CodeBuilder.InvokeBaseConstructor(Constants.AbstractInvocationConstructor,
				cArg1, cArg2, cArg3, cArg4);
			constructor.CodeBuilder.AddStatement(new ReturnStatement());

			// InvokeMethodOnTarget implementation

			MethodEmitter method = nested.CreateMethod("InvokeMethodOnTarget",
				new ReturnReferenceExpression(typeof(void)),
					MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);

			ParameterInfo[] parameters = methodInfo.GetParameters();

			Expression[] args = new Expression[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];

				if (!param.IsOut && !param.IsRetval)
				{
					args[i] = new ConvertExpression(param.ParameterType,
							new MethodInvocationExpression(SelfReference.Self,
								typeof(AbstractInvocation).GetMethod("GetArgumentValue"),
									new LiteralIntExpression(i)));
				}
				else
				{
					throw new NotImplementedException("Int/Ref parameters are not supported yet");
				}
			}

			LocalReference ret_local = null;
			MethodInvocationExpression baseMethodInvExp =
				new MethodInvocationExpression(targetField, methodInfo, args);

			// TODO: Process out/ref arguments

			if (methodInfo.ReturnType != typeof(void))
			{
				ret_local = method.CodeBuilder.DeclareLocal(typeof(object));
				method.CodeBuilder.AddStatement(
					new AssignStatement(ret_local,
						new ConvertExpression(typeof(object), methodInfo.ReturnType, baseMethodInvExp)));
			}
			else
			{
				method.CodeBuilder.AddStatement(new ExpressionStatement(baseMethodInvExp));
			}

			if (methodInfo.ReturnType != typeof(void))
			{
				MethodInvocationExpression setRetVal =
					new MethodInvocationExpression(SelfReference.Self,
						typeof(AbstractInvocation).GetMethod("set_ReturnValue"), ret_local.ToExpression());

				method.CodeBuilder.AddStatement(new ExpressionStatement(setRetVal));
			}

			method.CodeBuilder.AddStatement(new ReturnStatement());

			return nested;
		}

		protected MethodAttributes ObtainMethodAttributes(MethodInfo method)
		{
			MethodAttributes atts;

			//			if (context.ShouldCreateNewSlot(method))
			//			{
			//				atts = MethodAttributes.NewSlot;
			//			}
			//			else
			{
				atts = MethodAttributes.Virtual;
			}

			if (method.IsPublic)
			{
				atts |= MethodAttributes.Public;
			}
			if (IsInternalToDynamicProxy(method.DeclaringType.Assembly) && method.IsAssembly)
			{
				atts |= MethodAttributes.Assembly;
			}
			if (method.IsHideBySig)
			{
				atts |= MethodAttributes.HideBySig;
			}

			if (method.IsFamilyAndAssembly)
			{
				atts |= MethodAttributes.FamANDAssem;
			}
			else if (method.IsFamilyOrAssembly)
			{
				atts |= MethodAttributes.FamORAssem;
			}
			else if (method.IsFamily)
			{
				atts |= MethodAttributes.Family;
			}

			if (method.Name.StartsWith("set_") || method.Name.StartsWith("get_"))
			{
				atts |= MethodAttributes.SpecialName;
			}
			return atts;
		}
	}
}
