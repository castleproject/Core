using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using System.Reflection;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Views.Brail
{
    public class DslProvider : IQuackFu
    {
        private readonly BrailBase view = null;
        public DslProvider(BrailBase view)
        {
            this.view = view;
            foreach (PropertyInfo prop in typeof(BrailBase).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name == "Dsl" || prop.Name == "ViewEngine" || prop.Name == "Properties" || prop.CanRead == false)
                {
                    continue;
                }

                viewProperties.Add(prop.Name, prop.GetGetMethod());
            }
        }

        private IDslLanguageExtension currentExtension = null;
        private readonly IDictionary<string, MethodInfo> extensionMethods = new Dictionary<string, MethodInfo>();
        private readonly IDictionary<string, MethodInfo> viewProperties = new Dictionary<string, MethodInfo>();

        public void Register(IDslLanguageExtension dslExtension)
        {
            if (currentExtension == null)
            {
                foreach (MethodInfo method in dslExtension.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if(method.DeclaringType==typeof(object))
                        continue;
                    string name = CreateMethodKey(method.Name, method.GetParameters());
                    extensionMethods.Add(name, method);
                }
                currentExtension = dslExtension;
            }
        }

        private string CreateMethodKey(string methodName, object[] parameters)
        {
            return string.Format("{0}_{1}", methodName, parameters.GetLength(0));
        }

        #region IQuackFu Members

        public object QuackGet(string name, object[] parameters)
        {
            if (view.IsDefined(name))
            {
                return view.GetParameter(name);
            }

            if (viewProperties.ContainsKey(name))
            {
                return viewProperties[name].Invoke(view, null);
            }

            return null;
        }

        public object QuackInvoke(string name, params object[] args)
        {
            string methodName = CreateMethodKey(name, args);
            if (extensionMethods.ContainsKey(methodName))
            {
                MethodInfo method = extensionMethods[methodName];
                method.Invoke(currentExtension, args);
                return null;
            }
            //didn't find it in the hard coded methods, so we want to raise the generic ones.
            switch (args.Length)
            {
                case 0:
                    currentExtension.Tag(name);
                    break;
                case 1:
                    currentExtension.Tag(name, (ICallable)args[0]);
                    break;
                case 2:
                    currentExtension.Tag(name, (IDictionary)args[0],(ICallable)args[1]);
                    break;
            }
            return null;
        }

        public object QuackSet(string name, object[] parameters, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
