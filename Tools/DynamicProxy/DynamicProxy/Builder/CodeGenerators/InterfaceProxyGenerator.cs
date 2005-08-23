// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeGenerators
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Runtime.Serialization;

	using Castle.DynamicProxy.Builder.CodeBuilder;
	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;

	/// <summary>
	/// Summary description for InterfaceProxyGenerator.
	/// </summary>
	[CLSCompliant(false)]
	public class InterfaceProxyGenerator : BaseCodeGenerator
	{
		protected Type _targetType;
		protected FieldReference _targetField;

		public InterfaceProxyGenerator(ModuleScope scope) : base(scope)
		{
		}

		public InterfaceProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
		{
		}

		protected override Type InvocationType
		{
			get { return Context.InterfaceInvocation; }
		}

		protected override String GenerateTypeName(Type type, Type[] interfaces)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Type inter in interfaces)
			{
				sb.Append('_');
				sb.Append(inter.Name);
			}
			/// Naive implementation
			return String.Format("ProxyInterface{2}{0}{1}", type.Name, sb.ToString(), type.Namespace.Replace('.', '_'));
		}

		protected override void GenerateFields()
		{
			base.GenerateFields ();
			_targetField = MainTypeBuilder.CreateField("__target", typeof (object));
		}

		protected override MethodInfo GenerateCallbackMethodIfNecessary(MethodInfo method, Reference invocationTarget)
		{
			if (Context.HasMixins && _interface2mixinIndex.Contains(method.DeclaringType))
			{
				return method;
			}

			if (method.IsAbstract)
			{
				method = GetCorrectMethod(method);
			}

			return base.GenerateCallbackMethodIfNecessary(method, _targetField);
		}

		/// <summary>
		/// From an interface method (abstract) look up 
		/// for a matching method on the target
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		protected override MethodInfo GetCorrectMethod(MethodInfo method)
		{
			if (Context.HasMixins && _interface2mixinIndex.Contains(method.DeclaringType))
			{
				return method;
			}

			ParameterInfo[] paramsInfo = method.GetParameters();
			Type[] argTypes = new Type[paramsInfo.Length];

			for(int i=0;i < argTypes.Length; i++)
			{
				argTypes[i] = paramsInfo[i].ParameterType;
			}

			MethodInfo newMethod = _targetType.GetMethod(method.Name, argTypes);

			if (newMethod == null)
			{
				System.Diagnostics.Trace.Write("Target class does not offer the method " + method.Name);
				newMethod = method;
			}

			return newMethod;
		}

		/// <summary>
		/// Generates one public constructor receiving 
		/// the <see cref="IInterceptor"/> instance and instantiating a HybridCollection
		/// </summary>
		protected override EasyConstructor GenerateConstructor()
		{
			ArgumentReference arg1 = new ArgumentReference( Context.Interceptor );
			ArgumentReference arg2 = new ArgumentReference( typeof(object) );
			ArgumentReference arg3 = new ArgumentReference( typeof(object[]) );

			EasyConstructor constructor;

			if (Context.HasMixins)
			{
				constructor = MainTypeBuilder.CreateConstructor( arg1, arg2, arg3 );
			}
			else
			{
				constructor = MainTypeBuilder.CreateConstructor( arg1, arg2 );
			}
			
			GenerateConstructorCode(constructor.CodeBuilder, arg1, SelfReference.Self, arg3);

			constructor.CodeBuilder.InvokeBaseConstructor();

			constructor.CodeBuilder.AddStatement( new AssignStatement(
				_targetField, arg2.ToExpression()) );

			constructor.CodeBuilder.AddStatement( new ReturnStatement() );

			return constructor;
		}

		protected Type[] Join(Type[] interfaces, Type[] mixinInterfaces)
		{
			Type[] union = new Type[ interfaces.Length + mixinInterfaces.Length ];
			Array.Copy( interfaces, 0, union, 0, interfaces.Length );
			Array.Copy( mixinInterfaces, 0, union, interfaces.Length, mixinInterfaces.Length );
			return union;
		}

		protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1, ArgumentReference arg2)
		{
			Type[] key_and_object = new Type[] {typeof (String), typeof (Object)};
			MethodInfo addValueMethod = typeof (SerializationInfo).GetMethod("AddValue", key_and_object);

			codebuilder.AddStatement( new ExpressionStatement(
				new VirtualMethodInvocationExpression(arg1, addValueMethod, 
				new FixedReference("__target").ToExpression(), 
				_targetField.ToExpression() ) ) );
		}

		protected override Expression GetPseudoInvocationTarget(MethodInfo method)
		{
			if (Context.HasMixins && _interface2mixinIndex.Contains(method.DeclaringType))
			{
				return base.GetPseudoInvocationTarget(method);
			}

			return _targetField.ToExpression();
		}

		public virtual Type GenerateCode(Type[] interfaces, Type targetType)
		{
			if (Context.HasMixins)
			{
				_mixins = Context.MixinsAsArray();
				Type[] mixinInterfaces = InspectAndRegisterInterfaces( _mixins );
				interfaces = Join(interfaces, mixinInterfaces);
			}

			interfaces = AddISerializable(interfaces);

			Type cacheType = GetFromCache(targetType, interfaces);
			
			if (cacheType != null)
			{
				return cacheType;
			}

			_targetType = targetType;

			CreateTypeBuilder( GenerateTypeName(targetType, interfaces), typeof(Object), interfaces );
			GenerateFields();
			ImplementGetObjectData( interfaces );
			ImplementCacheInvocationCache();
			GenerateInterfaceImplementation( interfaces );
			GenerateConstructor();
			return CreateType();
		}
	}
}
