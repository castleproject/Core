using System;

using AopAlliance.Aop;

namespace AopAlliance.Intercept
{
	/// <summary>
    /// This interface represents a generic interceptor. 
    ///
    /// <p>A generic interceptor can intercept runtime events that occur within a base program. 
    /// Those events are materialized by (reified in) joinpoints. Runtime joinpoints can be invocations, field access, exceptions...</p>
    ///
    /// <p>This interface is not used directly. Use the the sub-interfaces to intercept specific events.</p>
    /// 
    /// <seealso cref="AopAlliance.Intercept.IJoinpoint"/>
    /// </summary>
	public interface IInterceptor : IAdvice
	{
	}
}
