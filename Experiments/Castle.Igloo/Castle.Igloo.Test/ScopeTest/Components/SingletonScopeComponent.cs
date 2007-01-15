
namespace Castle.Igloo.Test.ScopeTest.Components
{

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
