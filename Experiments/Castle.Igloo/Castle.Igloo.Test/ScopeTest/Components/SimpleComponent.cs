
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{

    [Scope(Scope = ScopeType.Thread, UseProxy=true)]
    public class SimpleComponent : IComponent
    {
        #region IComponent Members

        public int ID
        {
            get
            {
                return 99;
            }
        }

        #endregion
    }
}
