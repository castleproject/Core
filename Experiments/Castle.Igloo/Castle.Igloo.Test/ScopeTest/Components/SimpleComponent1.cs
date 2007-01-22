
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Thread, UseProxy = true)]
    public class SimpleComponent1 : IComponent1
    {
        private IComponent2 _simpleComponent2 = null;


        public SimpleComponent1(IComponent2 simpleComponent2)
        {
            _simpleComponent2 = simpleComponent2;
        }

        #region IComponent Members

        public IComponent2 SimpleComponent2
        {
            get { return _simpleComponent2; }
        }

        public int ID
        {
            get
            {
                return 101;
            }
        }


        #endregion
    }
}
