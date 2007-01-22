using System;
using System.Collections.Generic;
using System.Text;
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Thread, UseProxy = true)]
    public class SimpleComponent2 : IComponent2
    {
        private IComponent _simpleComponent = null;

        public SimpleComponent2(IComponent simpleComponent)
        {
            _simpleComponent = simpleComponent;
        }

        #region IComponent2 Members

        public string Name
        {
            get { return "SimpleComponent2"; }
        }

        public IComponent SimpleComponent
        {
            get { return _simpleComponent; }
        }

        #endregion
    }
}
