using System;

namespace AopAlliance.Intercept
{
	/// <summary>
	/// <p>This interface represents a generic runtime joinpoint (in the AOP
	/// terminology).</p>
	///
	/// <p>A runtime joinpoint is an <i>event</i> that occurs on a static
	/// joinpoint (i.e. a location in a the program). For instance, an
	/// invocation is the runtime joinpoint on a method (static joinpoint).
	/// The static part of a given joinpoint can be generically retrieved
	/// using the <c>GetStaticPart()</c> method.</p>
	///
	/// <p>In the context of an interception framework, a runtime joinpoint
	/// is then the reification of an access to an accessible object (a
	/// method, a constructor, a field), i.e. the static part of the
	/// joinpoint. It is passed to the interceptors that are installed on
	/// the static joinpoint.</p>
	/// 
    /// <seealso cref="AopAlliance.Intercept.IInterceptor"/>
    /// </summary>
    public interface IJoinpoint
	{
        /// <summary>
        /// <p>Returns the static part of this joinpoint.</p> 
        /// <p>The static part is an accessible object on which a chain of interceptors are installed.</p>
        /// </summary>
        System.Reflection.MemberInfo StaticPart { get; }

        /// <summary>
        /// <p>Returns the object that holds the current joinpoint's static part.</p>
        /// <p>For instance, the target object for an invocation.</p>
        /// </summary>
        object This { get; }

        /// <summary>
        /// <p>Proceeds to the next interceptor in the chain.</p>
        /// <p>The implementation and the semantics of this method depends on the actual joinpoint type (see the children interfaces).</p>
        /// </summary>
        /// <returns>See the children interfaces' proceed definition.</returns>
        /// <exception cref="System.Exception">if the joinpoint throws an exception.</exception>
        object Proceed();
	}
}
