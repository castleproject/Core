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
using System.Collections.Generic;

namespace Castle.Igloo.UIComponents
{
    public sealed class UIComponentRepository
    {
        private IDictionary<Type, UIComponent> _componentsByType = new Dictionary<Type, UIComponent>();
        private IDictionary<string, UIComponent> _componentsByName = new Dictionary<string, UIComponent>();


        /// <summary>
        /// Adds the uiComponent.
        /// </summary>
        /// <param name="uiComponent">The uiComponent.</param>
        public void AddComponent(UIComponent uiComponent)
        {
            if (uiComponent == null)
            {
                throw new ArgumentNullException("uiComponent");
            }
            _componentsByType.Add(uiComponent.ComponentType, uiComponent);
            _componentsByName.Add(uiComponent.Name, uiComponent);
        }



        /// <summary>
        /// Gets the component for this name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The found component</returns>
        public UIComponent ComponentForName(string name)
        {
            return _componentsByName[name + UIComponent.COMPONENT_SUFFIX];
        }


        /// <summary>
        ///  Gets the component for this type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The found component</returns>
        public UIComponent ComponentForType(Type type)
        {
            return _componentsByType[type];
        }
    }
}
