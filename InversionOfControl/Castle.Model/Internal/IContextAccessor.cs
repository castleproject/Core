using System.Collections;

namespace Castle.Model.Internal
{
    internal interface IContextAccessor
    {
        void Push();
        void Pop();
        Stack Stack { get; }
        IDictionary Items { get; }
    }
}