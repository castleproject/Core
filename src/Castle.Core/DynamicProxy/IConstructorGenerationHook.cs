using System.Reflection;

namespace Castle.DynamicProxy
{
    public enum ProxyConstructorImplementation
    {
        /// <summary>
        /// Do not generate a constructor. The proxy class will not
        /// be constructible with the specified constructor when this option is used.
        /// This is the default for not visible base constructors.
        /// </summary>
        SkipConstructor,

        /// <summary>
        /// Call the base constructor from the proxy constructor. This is the default for
        /// visible base constructors.
        /// </summary>
        CallBase,

        /// <summary>
        /// Do not call the base constructor from the proxy constructor.
        /// The generated code will fail verification with PEVerify and will only
        /// run in full trust. Under partial trust the runtime will throw VerificationException
        /// when an attempt is made to execute the code.
        /// </summary>
        DoNotCallBase,
    }

    public sealed class ConstructorImplementationAnalysis
    {
        /// <summary>
        /// True if the base member is visible (public or internal with InternalsVisibleTo)
        /// </summary>
        public bool IsBaseVisible { get; private set; }

        internal ConstructorImplementationAnalysis(bool isBaseVisible)
        {
            IsBaseVisible = isBaseVisible;
        }
    }

    public interface IConstructorGenerationHook
    {
        /// <summary>
        /// Specifies how to generate a default constructor for the proxy class when
        /// the target class doesn't have a default constructor.
        /// </summary>
        ProxyConstructorImplementation DefaultConstructorImplementation { get; }

        /// <summary>
        /// Specifies how to implement the given constructor in the target class.
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <param name="analysis">Additional information about the constructor.</param>
        /// <returns></returns>
        ProxyConstructorImplementation GetConstructorImplementation(ConstructorInfo constructorInfo, ConstructorImplementationAnalysis analysis);
    }
}
