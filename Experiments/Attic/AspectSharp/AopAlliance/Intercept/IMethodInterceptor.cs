using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>Intercepts calls on an interface on its way to the target. These are nested "on top" of the target.</p>
	/// </summary>
    /// <example>
    /// <p>The user should implement the <c>Invoke(IMethodInvocation)</c> method to modify the original behavior.</p>
    /// <code>
    ///     class TracingInterceptor : IMethodInterceptor 
    ///     {
    ///         Object Invoke(IMethodInvocation i) 
    ///         {
    ///             Console.WriteLine("Method {0} is called on {1} with args {2}", i.GetMethod(), i.GetThis(), i.GetArguments());
    ///             object ret = i.Proceed();
    ///             Console.WriteLine("Method {0} returned {1}", i.GetMethod(), ret);
    ///             
    ///             return ret;
    ///         }
    ///     }
    /// </code>
    /// </example>
	
	public interface IMethodInterceptor : IInterceptor
	{
        /// <summary>
        /// Implement this method to perform extra treatments before and after the invocation. 
        /// Polite implementations would certainly like to invoke IJoinpoint.Proceed(). 
        /// </summary>
        /// <param name="invocation">The method invocation joinpoint</param>
        /// <returns>The result of the call to IJoinpoint.Proceed(), might be intercepted by the interceptor.</returns>
        /// <exception cref="System.Exception">if the interceptors or the target-object throws an exception.</exception>
        object Invoke(IMethodInvocation invocation);
	}
}
