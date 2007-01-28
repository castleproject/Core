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

using System.Collections.Generic;
using System.Reflection;
using Castle.Core;
using Castle.Igloo.ComponentActivator;
using Castle.Igloo.Interceptors;
using Castle.Igloo.LifestyleManager;
using Castle.Igloo.Util;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.Igloo.Attributes;

namespace Castle.Igloo
{
    /// <summary>
    /// Sets scope on component by analysing <see cref="ScopeAttribute"/> or configuration file.
    /// </summary>
    public class ScopeInspector : IContributeComponentModelConstruction
    {
        public const string SCOPE_ATTRIBUTE = "_SCOPE_ATTRIBUTE_";
        public const string SCOPE_TOKEN = "scope";
        public const string PROXY_TOKEN = "proxyScope";
      
        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            if (model.Configuration != null &&
                model.Configuration.Attributes[SCOPE_TOKEN]!=null)
            {
                ScopeAttribute scopeAttribute = new ScopeAttribute();
                scopeAttribute.Scope = model.Configuration.Attributes[SCOPE_TOKEN];
                
                if (model.Configuration.Attributes[SCOPE_TOKEN]!=null)
                {
                    bool result = false;
                    bool.TryParse(model.Configuration.Attributes[PROXY_TOKEN], out result);
                    if (result)
                    {
                        scopeAttribute.UseProxy = bool.Parse(model.Configuration.Attributes[PROXY_TOKEN]);
                    }
                }

                DecorateComponent(model, scopeAttribute);
            }
            else if (AttributeUtil.HasScopeAttribute(model.Implementation))
            {
                ScopeAttribute scopeAttribute = AttributeUtil.GetScopeAttribute(model.Implementation);
                DecorateComponent(model, scopeAttribute);
            }                
        }

        /// <summary>
        /// Decorates the component.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="scopeAttribute">The scope attribute.</param>
        private void DecorateComponent(ComponentModel model, ScopeAttribute scopeAttribute)
        {
           if (!model.Name.StartsWith(ProxyScopeInterceptor.TARGET_NAME_PREFIX))
            {
                if (scopeAttribute.UseProxy)
                {
                    scopeAttribute.Scope = ScopeType.Singleton;
                    
                    model.CustomComponentActivator = typeof (ScopeComponentActivator);

                    // Ensure its CustomLifestyle
                    model.LifestyleType = LifestyleType.Custom;
                    model.CustomLifestyle = typeof(ScopeLifestyleManager);
                }
                else
                {
                    // Ensure its CustomLifestyle
                    model.LifestyleType = LifestyleType.Custom;
                    model.CustomLifestyle = typeof (ScopeLifestyleManager);

                    // Add the bijection interceptor
                    if (NeedBijection(model))
                    {
                        model.Interceptors.AddFirst(new InterceptorReference(typeof (BijectionInterceptor)));
                    }
                }
                model.ExtendedProperties.Add(SCOPE_ATTRIBUTE, scopeAttribute);
            }
            else
            {
                model.ExtendedProperties.Add(SCOPE_ATTRIBUTE, scopeAttribute);
                // Ensure its CustomLifestyle
                model.LifestyleType = LifestyleType.Custom;
                model.CustomLifestyle = typeof(ScopeLifestyleManager);

                // Add the bijection interceptor
                if (NeedBijection(model))
                {
                    model.Interceptors.AddFirst(new InterceptorReference(typeof(BijectionInterceptor)));
                }
            }
        }

        private bool NeedBijection(ComponentModel model)
        {
            bool needBijection = false;
            if ( model.ExtendedProperties[BijectionInspector.IN_MEMBERS]!=null )
            {
                IDictionary<InjectAttribute, PropertyInfo> inMembers = (IDictionary<InjectAttribute, PropertyInfo>)model.ExtendedProperties[BijectionInspector.IN_MEMBERS];
                needBijection = inMembers.Count > 0;
            }
            if (model.ExtendedProperties[BijectionInspector.OUT_MEMBERS] != null && !needBijection)
            {
                IDictionary<OutjectAttribute, PropertyInfo> outMembers = (IDictionary<OutjectAttribute, PropertyInfo>)model.ExtendedProperties[BijectionInspector.OUT_MEMBERS];
                needBijection = outMembers.Count > 0;
            }
            return needBijection;
        }
        
    }
}

