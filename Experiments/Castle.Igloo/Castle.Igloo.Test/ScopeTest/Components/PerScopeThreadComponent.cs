
using Castle.Igloo.Attributes;

namespace Castle.Igloo.Test.ScopeTest.Components
{
    [Scope(Scope = ScopeType.Thread)]
    public class PerScopeThreadComponent : IComponent
    {
		#region IComponent Members

		public int ID
		{
			get
			{
				return GetHashCode();
			}
		}

		#endregion
	}
}

