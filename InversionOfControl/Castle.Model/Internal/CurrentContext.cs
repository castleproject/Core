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

using System.Web;

namespace Castle.Model.Internal
{
    using System;
    using System.Text;
    using System.Collections;

    public sealed class CurrentContext
    {
        private static IContextAccessor accessor;

        const string GenericTypeParametersKey = "GenericTypeParameters.Key";

        private CurrentContext()
        {
        }

        public static Type[] GenericTypeParameters
        {
            get { return Accessor.Items[GenericTypeParametersKey] as Type[]; }
            set { Accessor.Items[GenericTypeParametersKey] = value; }
        }

        internal static IContextAccessor Accessor
        {
            get
            {
                if (accessor == null)
                    InitContext();
                return accessor;
            }
        }

        internal static IDictionary Items
        {
            get { return Accessor.Items; }
        }


        private static void InitContext()
        {
            //Safe for multi threading without locks
            if (HttpContext.Current != null)
                accessor = new WebContextAccessor();
            else
                accessor = new LocalThreadContextAccessor();
        }

        public static IDisposable EnterContext(Type[] genericTypeParameters)
        {
            Accessor.Push();
            GenericTypeParameters = genericTypeParameters;
            return new ClearContextDisposer();
        }

        public class ClearContextDisposer : IDisposable
        {
            #region IDisposable Members

            public void Dispose()
            {
                CurrentContext.ClearContext();
            }

            #endregion
        }

        public static void ClearContext()
        {
            Accessor.Pop();
        }

#if DOTNET2
        /// <summary>
        /// Intended to support caching of generic types.
        /// </summary>
        /// <param name="implementation">Unclosed generic type</param>
        /// <returns>Finished generic type</returns>
        internal static Type MakeGenericType(Type implementation)
        {
            Type[] parameters = GenericTypeParameters;
            if (parameters == null)
                return implementation;
            Type realImplementation;
            if (implementation.ContainsGenericParameters)
                realImplementation = implementation.GetGenericTypeDefinition();
            else
                realImplementation = implementation;
            return realImplementation.MakeGenericType(parameters);
        }
#endif

        public static Type GetImplementationType(Type targetType)
        {
#if DOTNET2

            if (targetType.ContainsGenericParameters)
                return CurrentContext.MakeGenericType(targetType);
#endif
            return targetType;
        }
    }
}