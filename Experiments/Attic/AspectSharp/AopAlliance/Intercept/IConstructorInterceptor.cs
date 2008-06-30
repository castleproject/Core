using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>Intercepts the construction of a new object.</p>
	/// </summary>
	/// <example>
	/// <p>The user should implement the <c>Construct(IConstructorInvocation)</c> method to modify the original behavior. 
	/// E.g. the following class implements a singleton interceptor (allows only one unique instance for the intercepted class): </p>
	/// <code>
    ///     class DebuggingInterceptor : IConstructorInterceptor 
    ///     {
    ///         object instance = null;
    ///     
    ///         object Construct(IConstructorInvocation i) 
    ///         {
    ///             if (instance == null) 
    ///             {
    ///                 return instance = i.Proceed();
    ///             } 
    ///             else 
    ///             {
    ///                 throw new Exception("singleton does not allow multiple instance");
    ///             }
    ///         }
    ///     }
	/// </code>
	/// </example>
	public interface IConstructorInterceptor : IInterceptor
	{
        /// <summary>
        /// <p>Implement this method to perform extra treatments before and after the consrution of a new object. 
        /// Polite implementations would certainly like to invoke IJoinpoint.Proceed(). </p>
        /// </summary>
        /// <param name="invocation">The construction joinpoint </param>
        /// <returns>The newly created object, which is also the result of the call to IJoinpoint.Proceed(), might be replaced by the interceptor.</returns>
        /// <exception cref="System.Exception">if the interceptors or the target-object throws an exception.</exception>
        object Construct(IConstructorInvocation invocation);
	}
}
