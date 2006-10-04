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

namespace Castle.DynamicProxy
{
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Runtime.Serialization;

    using Castle.DynamicProxy.Serialization;
    using Castle.DynamicProxy.Invocation;

    /// <summary>
    /// Summary description for GeneratorContext.
    /// </summary>
    public sealed class GeneratorContext : DictionaryBase
    {
        private IList _skipInterfaces = new ArrayList();
        private IList _skipMethods = new ArrayList();
        private IList _generateNewSlot = new ArrayList();
        private ArrayList _mixins = new ArrayList();
        private Type _proxyObjectReference = typeof(ProxyObjectReference);
        private Type _interceptor = typeof(IInterceptor);
        private Type _invocation = typeof(IInvocation);
        private Type _interfaceInvocation = typeof(InterfaceInvocation);
        private Type _sameClassInvocation = typeof(SameClassInvocation);

        public GeneratorContext()
        {
            // By default we skip a few interfaces
            AddInterfaceToSkip(typeof(ISerializable));

            // And their methods
            AddMethodToSkip(typeof(ISerializable).GetMethod("GetObjectData"));
        }

        /// <summary>
        /// The implementor of IObjectReference responsible for 
        /// the deserialization and reconstruction of the proxy object
        /// </summary>
        public Type ProxyObjectReference
        {
            get { return _proxyObjectReference; }
            set { _proxyObjectReference = value; }
        }

        public Type Interceptor
        {
            get { return _interceptor; }
            set { _interceptor = value; }
        }

        public Type Invocation
        {
            get { return _invocation; }
            set { _invocation = value; }
        }

        public Type InterfaceInvocation
        {
            get { return _interfaceInvocation; }
            set { _interfaceInvocation = value; }
        }

        public Type SameClassInvocation
        {
            get { return _sameClassInvocation; }
            set { _sameClassInvocation = value; }
        }

        public bool HasMixins
        {
            get { return _mixins.Count != 0; }
        }

        public void AddMixinInstance(object instance)
        {
            _mixins.Add(instance);
        }

        public object[] MixinsAsArray()
        {
            return _mixins.ToArray();
        }

        public bool ShouldSkip(Type interfaceType)
        {
            return _skipInterfaces.Contains(interfaceType);
        }

        public bool ShouldSkip(MethodInfo method)
        {
            return _skipMethods.Contains(method);
        }

        /// <summary>
        /// Checks if the method has the same signature as a method that was marked as
        /// one that should generate a new vtable slot.
        /// </summary>
        public bool ShouldCreateNewSlot(MethodInfo method)
        {
            string methodStr = method.ToString();
            foreach (MethodInfo candidate in _generateNewSlot)
            {
                if (candidate.ToString() == methodStr)
                    return true;
            }
            return false;
        }

        public void AddInterfaceToSkip(Type interfaceType)
        {
            _skipInterfaces.Add(interfaceType);
        }

        public void AddMethodToSkip(MethodInfo method)
        {
            _skipMethods.Add(method);
        }

        public void AddMethodToGenerateNewSlot(MethodInfo method)
        {
            _generateNewSlot.Add(method);
        }

        public object this[String key]
        {
            get { return Dictionary[key]; }
            set { Dictionary[key] = value; }
        }
    }
}
