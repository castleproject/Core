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
using Castle.Core;
using Castle.MicroKernel;
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Contexts
{
    public class ContextResolver: ISubDependencyResolver
    {
        private readonly IKernel _kernel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextResolver"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public ContextResolver(IKernel kernel)
		{
            _kernel = kernel;
		}

        /// <summary>
        /// Should return an instance of a service or property values as
        /// specified by the dependency model instance.
        /// It is also the responsability of <see cref="Castle.MicroKernel.IDependencyResolver"/>
        /// to throw an exception in the case a non-optional dependency
        /// could not be resolved.
        /// </summary>
        /// <param name="context">Creation context, which is a resolver itself</param>
        /// <param name="parentResolver">Parent resolver</param>
        /// <param name="model">Model of the component that is requesting the dependency</param>
        /// <param name="dependency">The dependency model</param>
        /// <returns>The dependency resolved value or null</returns>
		public object Resolve(CreationContext context, 
		                      ISubDependencyResolver parentResolver, 
		                      ComponentModel model, DependencyModel dependency)
		{
			if (CanResolve(context, parentResolver, model, dependency))
			{
                IContexts contexts = _kernel[typeof(IContexts)] as IContexts;
                IDictionary<string, InjectAttribute> inMembers = (IDictionary<string, InjectAttribute>)model.ExtendedProperties[BijectionInspector.IN_MEMBERS];
                InjectAttribute inAttribute = inMembers[dependency.DependencyKey];

                return contexts.GetFromContexts(inAttribute);
			}

			return null;
		}

        /// <summary>
        /// Returns true if the resolver is able to satisfy this dependency.
        /// </summary>
        /// <param name="context">Creation context, which is a resolver itself</param>
        /// <param name="parentResolver">Parent resolver</param>
        /// <param name="model">Model of the component that is requesting the dependency</param>
        /// <param name="dependency">The dependency model</param>
        /// <returns>
        /// 	<c>true</c> if the dependency can be satisfied
        /// </returns>
		public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
		{
            if (model.ExtendedProperties.Contains(BijectionInspector.IN_MEMBERS))
            {
                IDictionary<string, InjectAttribute> inMembers = (IDictionary<string, InjectAttribute>)model.ExtendedProperties[BijectionInspector.IN_MEMBERS];

                if (inMembers.ContainsKey(dependency.DependencyKey))
                {
                    IContexts contexts = _kernel[typeof(IContexts)] as IContexts;
                    InjectAttribute inAttribute = inMembers[dependency.DependencyKey];
                    return contexts.IsInContexts(inAttribute.Name);                
                }
                else
                {
                    return false;
                }
            }
            return false;

		}
    }
}

