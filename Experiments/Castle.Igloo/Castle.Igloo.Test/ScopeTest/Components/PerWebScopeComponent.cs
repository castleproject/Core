
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Request)]
    public class PerScopeWebRequestComponent : IComponent
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
