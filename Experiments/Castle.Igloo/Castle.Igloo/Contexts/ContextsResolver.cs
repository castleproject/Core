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
using Castle.Core;
using Castle.MicroKernel;

namespace Castle.Igloo.Contexts
{
    /// <summary>
    /// Custom resolver used by the MicroKernel. It gives
    /// us some contextual information that we use to set up a logging
    /// before satisfying the dependency
    /// </summary>
    public class ContextsResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextsResolver"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public ContextsResolver(IKernel kernel)
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
			if (CanResolve(context,  parentResolver, model, dependency))
			{
                IContexts contexts = _kernel[typeof(IContexts)] as IContexts;

                if (dependency.DependencyKey == "ApplicationContext")
			    {
			       return contexts.ApplicationContext;
			    }
                else if (dependency.DependencyKey == "ConversationContext")
                {
                    return contexts.ConversationContext;
                }
                else if (dependency.DependencyKey == "PageContext")
                {
                    return contexts.PageContext;
                }
                else if (dependency.DependencyKey == "RequestContext")
                {
                    return contexts.RequestContext;
                }
                else if (dependency.DependencyKey == "SessionContext")
                {
                    return contexts.SessionContext;
                }
			    else
                {
                    throw new InvalidCastException("The IContexts doesn't contains an IContext of type " +
                                                   dependency.DependencyKey);
                }
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
			return dependency.TargetType == typeof(IContext);
		}
    }
}
