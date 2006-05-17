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
    /// Represents a constructor of the component 
    /// that the container can use to initialize it properly.
    /// </summary>
    [Serializable]
    public class ConstructorCandidate
    {
        private ConstructorInfo constructorInfo;
        private DependencyModel[] dependencies;
        private int points;
#if DOTNET2
        private bool containsGenericTypes;
#endif

        public ConstructorCandidate(ConstructorInfo constructorInfo, params DependencyModel[] dependencies)
        {
            this.constructorInfo = constructorInfo;
            this.dependencies = dependencies;
#if DOTNET2
            this.containsGenericTypes = constructorInfo.DeclaringType.ContainsGenericParameters;
#endif

        }

        public ConstructorInfo Constructor
        {
            get
            {
#if DOTNET2
                if (containsGenericTypes)
                {
                    Type type = CurrentContext.MakeGenericType(constructorInfo.DeclaringType);
                    System.Collections.Generic.List<Type> types = new System.Collections.Generic.List<Type>();
                    foreach (ParameterInfo info in constructorInfo.GetParameters())
                    {
                        types.Add(
                            CurrentContext.MakeGenericType(info.ParameterType));
                    }

                    return type.GetConstructor(types.ToArray());
                }
#endif
                return constructorInfo;
            }
        }

        public DependencyModel[] Dependencies
        {
            get { return dependencies; }
        }

        public int Points
        {
            get { return points; }
            set { points = value; }
        }
    }
}
