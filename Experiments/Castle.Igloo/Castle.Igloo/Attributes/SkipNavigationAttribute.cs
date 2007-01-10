using System;

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Specifies that a controller méthod must not declench a navigation to another view.
    /// </summary>
    [AttributeUsageAttribute( AttributeTargets.Class | AttributeTargets.Method )]
    public class SkipNavigationAttribute : Attribute 
    {
    }
}
