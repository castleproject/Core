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

namespace AspectSharp.Core.Dispatcher
{
	using System;
	using System.Reflection;
	using AopAlliance.Intercept;

	/// <summary>
	/// Summary description for DefaultMethodInvocation.
	/// </summary>
	public class InvocationComposite : IMutableMethodInvocation
	{
		private IMethodInterceptor[] _interceptors;
		private Castle.DynamicProxy.IInvocation _innerInvocation;
		private object[] _arguments;
		private int _currentIndex;

		public InvocationComposite(IMethodInterceptor[] interceptors,
		                           Castle.DynamicProxy.IInvocation innerInvocation, object[] arguments)
		{
			_interceptors = interceptors;
			_innerInvocation = innerInvocation;
			_arguments = arguments;
		}

		public MethodBase Method
		{
			get { return _innerInvocation.Method; }
		}

		public object[] Arguments
		{
			get { return _arguments; }
		}

		public void SetArguments(object[] arguments)
		{
			_arguments = arguments;
		}

		public MemberInfo StaticPart
		{
			get { return _innerInvocation.Method; }
		}

		public object This
		{
			get { return _innerInvocation.Proxy; }
		}

		public object Proceed()
		{
			object retVal = null;

			if (_currentIndex + 1 <= _interceptors.Length)
			{
				retVal = _interceptors[_currentIndex++].Invoke(this);
			}
			else
			{
				retVal = _innerInvocation.Proceed(_arguments);
			}

			return retVal;
		}
	}
}