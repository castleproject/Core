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

namespace Castle.DynamicProxy.Builder
{
	using System;
	using Castle.DynamicProxy.Builder.CodeGenerators;

	/// <summary>
	/// Summary description for DefaultProxyBuilder.
	/// </summary>
	[CLSCompliant(false)]
	public class DefaultProxyBuilder : IProxyBuilder
	{
		ModuleScope _scope = new ModuleScope();

		public DefaultProxyBuilder()
		{
		}

		protected ModuleScope ModuleScope
		{
			get { return _scope; }
		}

		#region IProxyBuilder Members

		public virtual Type CreateInterfaceProxy(Type[] interfaces, Type type)
		{
			InterfaceProxyGenerator generator = new InterfaceProxyGenerator(_scope);
			return generator.GenerateCode(interfaces, type);
		}

		public virtual Type CreateClassProxy(Type theClass)
		{
			ClassProxyGenerator generator = new ClassProxyGenerator(_scope);
			return generator.GenerateCode(theClass);
		}

		public virtual Type CreateCustomInterfaceProxy(Type[] interfaces, Type type, GeneratorContext context)
		{
			InterfaceProxyGenerator generator = new InterfaceProxyGenerator(_scope, context);
			return generator.GenerateCode(interfaces, type);
		}

		public virtual Type CreateCustomClassProxy(Type theClass, GeneratorContext context)
		{
			ClassProxyGenerator generator = new ClassProxyGenerator(_scope, context);
			return generator.GenerateCustomCode(theClass, null);
		}

		#endregion
	}
}