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

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Castle.Igloo.Inspectors
{
    /// <summary>
    /// Retrieves injected/outjected properties
    /// and
    /// sets scope on component.
    /// </summary>
    public class IglooInspector : IContributeComponentModelConstruction
    {
        #region IContributeComponentModelConstruction Members

        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel,ComponentModel model)
        {
            // Ensures that the 2 inspectors are done in this order
            
            BijectionInspector bijectionInspector = new BijectionInspector();
            bijectionInspector.ProcessModel(kernel, model);
            
            ScopeInspector scopeInspector = new ScopeInspector();
            scopeInspector.ProcessModel(kernel, model);
        }

        #endregion
    }
}
