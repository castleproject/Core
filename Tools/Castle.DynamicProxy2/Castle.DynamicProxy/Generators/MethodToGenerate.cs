// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	public class ProxyMethod : IProxyMethod
	{
		private readonly MethodInfo method;

		public ProxyMethod(MethodInfo method, object target)
		{
			this.method = method;
			this.target = target;
		}

		private readonly object target;

		public MethodInfo Method
		{
			get { return method; }
		}

		public bool HasTarget
		{
			get { return target != null; }
		}

		public object Target
		{
			get { return target;}
		}
	}

	public class MethodToGenerate : ProxyMethod
	{
		private readonly bool standalone;

		public MethodToGenerate(MethodInfo method, bool standalone, object target)
			: base(method,target)
		{
			this.standalone = standalone;
		}

		public bool Standalone
		{
			get { return standalone; }
		}
	}
}