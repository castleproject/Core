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


namespace Castle.Model
{
    using System;
    using System.Reflection;
    using Castle.Model.Internal;

    /// <summary>
    /// Represents a property and the respective optional 
    /// dependency.
    /// </summary>
    [Serializable]
    public class PropertySet
    {
        private PropertyInfo propertyInfo;
        private DependencyModel dependency;
#if DOTNET2
        private bool containsGenericTypes;
        private Type[] indexTypes;
#endif

        public PropertySet(PropertyInfo propertyInfo, DependencyModel dependency)
        {
            this.propertyInfo = propertyInfo;
            this.dependency = dependency;
#if DOTNET2
            this.containsGenericTypes = propertyInfo.DeclaringType.ContainsGenericParameters;
            System.Collections.Generic.List<Type> types = new System.Collections.Generic.List<Type>();
            if(containsGenericTypes )
            {
		        foreach (ParameterInfo info in propertyInfo.GetIndexParameters())
		        {
		            types.Add(info.ParameterType);
		        }
            }
            this.indexTypes = types.ToArray();
#endif
        }

        public PropertyInfo Property
        {
            get
            {
#if DOTNET2
                if (containsGenericTypes)
                {
                    Type type = CurrentContext.MakeGenericType(propertyInfo.DeclaringType);
                    return type.GetProperty(propertyInfo.Name, propertyInfo.PropertyType,indexTypes);
                }
#endif
                return propertyInfo;
            }
        }

        public DependencyModel Dependency
        {
            get { return dependency; }
        }
    }
}
