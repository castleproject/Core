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
	using System.Reflection;
	using Contributors;

	public class MethodToGenerate : IProxyMethod
	{
		private readonly bool standalone;
		private readonly MethodInfo methodOnTarget;
		private readonly bool proxyable;
		private readonly MethodInfo method;
		private readonly ITypeContributor target;

		public MethodToGenerate(MethodInfo method, bool standalone, ITypeContributor target, MethodInfo methodOnTarget, bool proxyable)
		{
			this.method = method;
			this.target = target;
			this.standalone = standalone;
			this.methodOnTarget = methodOnTarget;
			this.proxyable = proxyable;
		}

		public bool Proxyable
		{
			get { return proxyable; }
		}

		public MethodInfo MethodOnTarget
		{
			get { return methodOnTarget; }
		}

		public bool Standalone
		{
			get { return standalone; }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public bool HasTarget
		{
			get
			{
				return target != null;
			}
		}

	}
}