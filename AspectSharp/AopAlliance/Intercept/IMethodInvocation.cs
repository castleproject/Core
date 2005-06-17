using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>Description of an invocation to a method, given to an interceptor upon method-call.</p>
	/// <p>A method invocation is a joinpoint and can be intercepted by a method interceptor.</p>
	/// 
	/// <seealso cref="AopAlliance.Intercept.IMethodInterceptor"/>
	/// </summary>
	public interface IMethodInvocation : IInvocation
	{
        /// <summary>
        /// <p>Gets the method being called.</p>
        /// <p>This method is a friendly implementation of the IJoinpoint.GetStaticPart() method (same result). </p>
        /// </summary>
        System.Reflection.MethodBase Method { get; }
	}
}
