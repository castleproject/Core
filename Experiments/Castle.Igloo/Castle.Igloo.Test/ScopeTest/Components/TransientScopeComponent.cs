
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Transient)]
    public class TransientScopeComponent : IComponent
    {

        #region IComponent Members

        public int ID
        {
            get
            {
                return this.GetHashCode();
            }
        }

        #endregion
    }
}
