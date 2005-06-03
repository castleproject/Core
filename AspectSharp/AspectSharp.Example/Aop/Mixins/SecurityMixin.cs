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

namespace AspectSharp.Example.Aop.Mixins
{
	using System;

	using AspectSharp.Core;

	using AspectSharp.Example.Security;

	/// <summary>
	/// Summary description for SecurityMixin.
	/// </summary>
	public class SecurityMixin : ISecurityResource, IProxyAware	
	{
		private object proxy;

		public SecurityMixin()
		{
		}

		public void SetProxy(object proxy)
		{
			this.proxy = proxy;
		}

		public String ResourceType
		{
			get { return proxy.GetType().Name; }
		}
	}
}