// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.GenInterfaces
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	public interface GenInterfaceWithGenericTypes
	{
		IList Find(string[,] query);

		IList<T> Find<T>(string query);

		IList<String> FindStrings(string query);

		void Populate<T>(IList<T> list);
	}

	public class GenInterfaceWithGenericTypesImpl : GenInterfaceWithGenericTypes
	{
		public IList Find(string[,] query)
		{
			return new String[0];
		}

		public IList Find(string query)
		{
			return new String[0];
		}

		public IList<T> Find<T>(string query)
		{
			return new List<T>();
		}

		public IList<string> FindStrings(string query)
		{
			return new List<String>();
		}

		public void Populate<T>(IList<T> list)
		{
		}
	}

	public class Proxy : GenInterfaceWithGenericTypes
	{
		private readonly IInterceptor[] interceptors;
		private readonly GenInterfaceWithGenericTypesImpl target;

		public Proxy(IInterceptor[] interceptors, GenInterfaceWithGenericTypesImpl target)
		{
			this.interceptors = interceptors;
			this.target = target;
		}

		public void Populate<T>(IList<T> list)
		{
			Find3Invo<T> inv = new Find3Invo<T>(target, interceptors, typeof(Proxy),
				null, null, new object[] { list });
			inv.Proceed();
		}

		public IList Find(string[,] query)
		{
			Find1Invo inv = new Find1Invo(target, interceptors, typeof(Proxy),
				null, null, new object[] { query });
			inv.Proceed();

			return (IList)inv.ReturnValue;
		}

		public IList Find(string query)
		{
			Find1InvoA inv = new Find1InvoA(target, interceptors, typeof(Proxy),
				null, null, new object[] { query });
			inv.Proceed();

			return (IList)inv.ReturnValue;
		}

		public IList<T> Find<T>(string query)
		{
			Find2Invo<T> inv = new Find2Invo<T>(target, interceptors, typeof(Proxy),
				null, null, new object[] { query });
			inv.Proceed();

			return (IList<T>)inv.ReturnValue;
		}

		public IList<string> FindStrings(string query)
		{
			throw new NotImplementedException();
		}

		public class Find2Invo<T> : AbstractInvocation
		{
			private GenInterfaceWithGenericTypesImpl target;

			public Find2Invo(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = (target as GenInterfaceWithGenericTypesImpl).Find<T>((String)GetArgumentValue(0));
			}
		}

		public class Find1Invo : AbstractInvocation
		{
			private GenInterfaceWithGenericTypesImpl target;

			public Find1Invo(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments) :
					base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = (target as GenInterfaceWithGenericTypesImpl).Find((String[,])GetArgumentValue(0));
			}
		}

		public class Find1InvoA : AbstractInvocation
		{
			private GenInterfaceWithGenericTypesImpl target;

			public Find1InvoA(GenInterfaceWithGenericTypesImpl target, IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				ReturnValue = (target as GenInterfaceWithGenericTypesImpl).Find((String)GetArgumentValue(0));
			}
		}

		public class Find3Invo<T> : AbstractInvocation
		{
			private GenInterfaceWithGenericTypesImpl target;

			public Find3Invo(GenInterfaceWithGenericTypesImpl target,
				IInterceptor[] interceptors, Type targetType,
				MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
				: base(target, interceptors, interfMethod, arguments)
			{
				this.target = target;
			}

			public override object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public override Type TargetType
			{
				get { throw new NotImplementedException(); }
			}

			public override MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			protected override void InvokeMethodOnTarget()
			{
				(target as GenInterfaceWithGenericTypesImpl).Populate((List<T>)GetArgumentValue(0));
			}
		}
	}
}