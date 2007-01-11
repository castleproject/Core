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
using System.Diagnostics;
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.Igloo.Attributes;
using Castle.Igloo.Scopes;
using Castle.Igloo.Controllers;
using Castle.Igloo.Util;

namespace Castle.Igloo.UIComponents
{
    /// <summary>
    /// A UI component that keeps all infos to do injection. 
    /// </summary>
    public sealed class UIComponent
    {
        /// <summary>
        /// Binding token
        /// </summary>
        private BindingFlags BINDING_FLAGS_SET
            = BindingFlags.Public
            | BindingFlags.SetProperty
            | BindingFlags.Instance
            | BindingFlags.SetField
            ;
        
        public const string COMPONENT_SUFFIX = ".uicomponent";
        public const string VIEW_SUFFIX = "view.";

        private string _name = string.Empty;
        private IKernel _kernel = null;
        private Type _componentType = null;
        private IDictionary<InjectAttribute, PropertyInfo> _inMembers = new Dictionary<InjectAttribute, PropertyInfo>();
        private IDictionary<OutjectAttribute, PropertyInfo> _outMembers = new Dictionary<OutjectAttribute, PropertyInfo>();
        private IList<PropertyInfo> _inControllers = new List<PropertyInfo>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }


        /// <summary>
        /// Gets the in members (properties or fields).
        /// </summary>
        /// <value>The in properties.</value>
        public IDictionary<InjectAttribute, PropertyInfo> InMembers
        {
            get { return _inMembers; }
        }

        /// <summary>
        /// Gets the out (properties or fields).
        /// </summary>
        /// <value>The out properties.</value>
        public IDictionary<OutjectAttribute, PropertyInfo> OutMembers
        {
            get { return _outMembers; }
        }

        /// <summary>
        /// Gets the type of the component.
        /// </summary>
        /// <value>The type of the component.</value>
        public Type ComponentType
        {
            get { return _componentType; }
        }

        /// <summary>
        /// Gets a value indicating whether [needs injection].
        /// </summary>
        /// <value><c>true</c> if [needs injection]; otherwise, <c>false</c>.</value>
        public bool NeedsInjection
        {
            get { return _inMembers.Count > 0; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UIComponent"/> class.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="kernel">The kernel</param>
        public UIComponent(Type componentType, IKernel kernel)
        {
            AssertUtils.ArgumentNotNull(componentType, "componentType");
            AssertUtils.ArgumentNotNull(kernel, "kernel");

            _kernel = kernel; 
            _name = componentType.FullName + COMPONENT_SUFFIX; ;
            _componentType = componentType;
            
            InitMembers();
        }

        
        /// <summary>
        /// Inject context variable values into @In attributes
        /// of a component instance.
        /// </summary>
        /// <param name="instance">A NTie component instance.</param>
        public void Inject(Object instance)
        {
            InjectMembers(instance);
            InjectControllers(instance);
        }


        /// <summary>
        /// Outject context variable values from @Out attributes
        /// of a component instance.
        /// </summary>
        /// <param name="instance">A NTie component instance.</param>
        public void Outject(Object instance)
        {
            OutjectMembers(instance);
            // TO DO Outject field
        }

        /// <summary>
        /// TO DO Inject field
        /// </summary>
        /// <param name="instance"></param>
        private void InjectMembers(Object instance)
        {
            if (NeedsInjection)
            {
                Trace.WriteLine("injecting dependencies of : " + _name);

                IScopeRegistry scopeRegistry = _kernel[typeof(IScopeRegistry)] as IScopeRegistry;

                foreach (KeyValuePair<InjectAttribute, PropertyInfo> kvp in InMembers)
                {                   
                    PropertyInfo propertyInfo = kvp.Value;
                    object instanceToInject = scopeRegistry.GetFromScopes(kvp.Key);

                    if (instanceToInject == null)
                    {
                        if (kvp.Key.Create)
                        {
                            instanceToInject = Activator.CreateInstance(propertyInfo.PropertyType);

                            IScope scope = scopeRegistry[kvp.Key.Scope];
                            scope.Add(kvp.Key.Name, instanceToInject);
                        }
                        else
                        {
                            // do log / use  kvp.Key.Required
                        }
                    }
                    propertyInfo.SetValue(instance, instanceToInject, null);
                }
            }
        }

        private void InjectControllers(Object instance)
        {
            foreach(PropertyInfo propertyInfo in _inControllers)
            {
                object controller = _kernel[propertyInfo.PropertyType];
                propertyInfo.SetValue(instance, controller, null);
            }       
        }
        

        private void OutjectMembers(Object instance)
        {
            // TO DO
        }
        
        private void InitMembers()
        {
            if (AttributeUtil.HasScopeAttribute(_componentType))
            {
                _name = VIEW_SUFFIX + _name;
                RetrieveInjectedProperties();
            }
        }
        
        /// <summary>
        /// Retrieves the injected user IScope component and
        /// the injected IController component on a view component
        /// </summary>
        /// <remarks>
        /// Today, only check injected properties memnbers
        /// TO DO also check injected fields 
        /// </remarks>
        private void RetrieveInjectedProperties()
        {
            PropertyInfo[] properties = _componentType.GetProperties(BINDING_FLAGS_SET);
            
            for (int i = 0; i < properties.Length; i++)
            {
                if ( !typeof(IController).IsAssignableFrom(properties[i].PropertyType))
                {
                    InjectAttribute injectAttribute = AttributeUtil.GetInjectAttribute(properties[i]);
                    if (injectAttribute != null)
                    {
                        if (injectAttribute.Name.Length == 0)
                        {
                            injectAttribute.Name = properties[i].Name;
                        }
                        _inMembers.Add(injectAttribute, properties[i]);
                    }
                }
            }
            
            // Inject attribute are ignored for IController component
            for (int i = 0; i < properties.Length; i++)
            {
                if ( (typeof(IController).IsAssignableFrom(properties[i].PropertyType)) )
                {
                    _inControllers.Add(properties[i]);
                }                 
            }
        }
    }
}
