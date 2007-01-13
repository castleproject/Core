
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{

    [Scope(Scope = ScopeType.Singleton)]
    public class SingletonScopeComponent : IComponent
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
