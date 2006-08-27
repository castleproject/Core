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

namespace Castle.Facilities.SecurityManagement
{
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using Castle.MicroKernel;
    using Castle.Core.Interceptor;
    using Castle.Services.Security;

    /// <summary>
	/// Summary description for SecurityInterceptor.
	/// </summary>
	public class SecurityInterceptor : IMethodInterceptor
	{
        private IKernel _kernel;

        public SecurityInterceptor(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object Intercept(IMethodInvocation invocation, params object[] args)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;

            if (!methodInfo.IsDefined( typeof(PermissionAttribute), true ))
            {
                return invocation.Proceed(args);
            }
            else
            {
                object[] attrs = methodInfo.GetCustomAttributes( typeof(PermissionAttribute), true );

                PermissionAttribute permissionAtt = (PermissionAttribute) attrs[0];

                ISecurityManager manager = (ISecurityManager) _kernel[ typeof(ISecurityManager) ];

                IPolicy policy = 
                    manager.Generate( 
                    permissionAtt, Thread.CurrentPrincipal );

                if (policy == null)
                {
                    return invocation.Proceed(args);
                }

                object value = null;

                if(policy.Evaluate())
                {
                    value = invocation.Proceed(args);
                }
                else
                {
                    throw new SecurityException("Not Allowed");
                }
                
                return value;
            }
        }
	}
}
