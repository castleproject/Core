#region Apache Notice
/*****************************************************************************
 * 
 * Castle.Igloo
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System;
using System.Web;
using Castle.Core;
using Castle.Igloo.Interceptors;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Util;
using Castle.MicroKernel;
//using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.ModelBuilder;
using Castle.Igloo.Attributes;

namespace Castle.Igloo
{
    /// <summary>
    /// Analyses <see cref="ScopeAttribute"/> on component.
    /// </summary>
    public class ScopeInspector : IContributeComponentModelConstruction
    {
 
        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            if (!AttributeUtil.HasScopeAttribute(model.Implementation))
            {
                return;
            }

            // For unit test
            HttpContext current = HttpContext.Current;
            ScopeAttribute scopeAttribute = AttributeUtil.GetScopeAttribute(model.Implementation);

            // Ensure its CustomLifestyle
            if (current != null)
            {
                if (scopeAttribute.Scope==ScopeType.Request)
                {
                    model.LifestyleType = LifestyleType.Custom;
                    model.CustomLifestyle = typeof(ScopeWebRequestLifestyleManager);

                    // Add the scope interceptor
                    model.Interceptors.AddFirst(new InterceptorReference(typeof(BijectionInterceptor)));
                }
                else if (scopeAttribute.Scope == ScopeType.Session)
                {
                    model.LifestyleType = LifestyleType.Custom;
                    model.CustomLifestyle = typeof(ScopeWebSessionLifestyleManager);
                    
                    // Add the scope interceptor
                    model.Interceptors.AddFirst(new InterceptorReference(typeof(BijectionInterceptor)));
                }
                else if (scopeAttribute.Scope == ScopeType.Application)
                {
                    model.LifestyleType = LifestyleType.Custom;
                    model.CustomLifestyle = typeof(ScopeWebApplicationLifestyleManager);

                    // Add the scope interceptor
                    model.Interceptors.AddFirst(new InterceptorReference(typeof(BijectionInterceptor)));
                }
                else
                {
                    throw new NotImplementedException("To do, other scope such as Session, Page");
                }

            }
            else
            {
                if (scopeAttribute.Scope == ScopeType.Request)
                {
                    // For unit test
                    model.LifestyleType = LifestyleType.Transient;
                }
                else if (scopeAttribute.Scope == ScopeType.Session)
                {
                    // For unit test
                    model.LifestyleType = LifestyleType.Transient;
                }
                else
                {
                    throw new NotImplementedException("To do, other scope such as Session, Page");
                }
            }
        }


    }
}

