using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>This interface represents an invocation in the program.</p>
	/// <p>An invocation is a joinpoint and can be intercepted by an interceptor.</p>
	/// </summary>
	public interface IInvocation : IJoinpoint
	{
        /// <summary>
        /// Get the arguments as an array object. It is possible to change element values within this array to change the arguments.
        /// </summary>
        object[] Arguments { get; }
	}
}
