
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Singleton)]
    public class SimpleComponent0 : IComponent0
    {
        private IComponent1 _simpleComponent1 = null;

        public SimpleComponent0(IComponent1 simpleComponent1)
        {
            _simpleComponent1 = simpleComponent1;
        }

        #region IComponent Members

        public int ID
        {
            get { return this.GetHashCode(); }
        }

        public IComponent1 SimpleComponent1
        {
            get { return _simpleComponent1; }
        }

        #endregion
    }
}
