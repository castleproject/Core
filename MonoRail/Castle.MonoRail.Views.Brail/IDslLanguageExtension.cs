using System.Collections;
using Boo.Lang;

namespace Castle.MonoRail.Views.Brail
{
    public interface IDslLanguageExtension
    {
        void Tag(string name);
        void Tag(string name, ICallable block);
        void Tag(string name, IDictionary attributes, ICallable block);

        void Flush();
    }
}