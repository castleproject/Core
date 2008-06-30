using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>Description of an invocation to a constuctor, given to an interceptor upon construtor-call.</p>
	/// <p>A constructor invocation is a joinpoint and can be intercepted by a constructor interceptor.</p>
	/// 
	/// <seealso cref="AopAlliance.Intercept.IConstructorInterceptor"/>
	/// </summary>
	public interface IConstructorInvocation : IInvocation
	{
        /// <summary>
        /// <p>Gets the constructor being called.</p>
        /// <p>This method is a friendly implementation of the IJoinpoint.GetStaticPart() method (same result). </p>
        /// </summary>
        /// <returns>The constructor being called.</returns>
        System.Reflection.ConstructorInfo GetConstructor();
	}
}
