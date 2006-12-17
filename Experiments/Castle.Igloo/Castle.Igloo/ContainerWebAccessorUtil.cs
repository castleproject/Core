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

using System.Web;
using Castle.Igloo.Util;

using Castle.Windsor;
 
namespace Castle.Igloo
{

	/// <summary>
	/// Uses the HttpContext and the <see cref="IContainerAccessor"/> 
	/// to access the container instance.
	/// </summary>
	public abstract class ContainerWebAccessorUtil
	{
        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        internal static IWindsorContainer Container
        {
            get
            {
                IContainerAccessor accessor = WebUtil.GetCurrentHttpContext().ApplicationInstance as IContainerAccessor;

                if (accessor == null)
                {
                    throw new ApplicationException("You must extend the HttpApplication in your web project " +
                        "and implement the IContainerAccessor to properly expose your container instance");
                }

                IWindsorContainer container = accessor.Container;

                if (container == null)
                {
                    throw new ApplicationException("The container seems to be unavailable in " +
                        "your HttpApplication subclass");
                }

                return container;
            }
        }
	}
}
