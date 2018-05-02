// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using Castle.DynamicProxy.Tests.GenInterfaces;

	using NUnit.Framework;

	[TestFixture]
	public class GenericConstraintsTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Non_generic_type_generic_method_with_class_struct_and_new_constraints()
		{
			CreateProxyFor<IHaveGenericMethodWithNewClassStructConstraints>();
		}

		[Test]
		public void Generic_type_generic_method_with_struct_base_Method_base_Type_constraints()
		{
			CreateProxyFor<IConstraint_Method1IsTypeStructAndMethod2<object>>();
		}


		private T CreateProxyFor<T>(params IInterceptor[] interceptors) where T : class
		{
			return generator.CreateInterfaceProxyWithoutTarget<T>(interceptors);
		}
	}
}