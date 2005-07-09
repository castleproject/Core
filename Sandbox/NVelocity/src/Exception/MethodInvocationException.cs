namespace NVelocity.Exception
{
    using System;

    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */

    /// <summary>  Application-level exception thrown when a reference method is
    /// invoked and an exception is thrown.
    /// <br>
    /// When this exception is thrown, a best effort will be made to have
    /// useful information in the exception's message.  For complete
    /// information, consult the runtime log.
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version> $Id: MethodInvocationException.cs,v 1.3 2003/10/27 13:54:08 corts Exp $
    ///
    /// </version>
    public class MethodInvocationException:VelocityException {
	public virtual System.String MethodName
	{
	    get {
		return methodName;
	    }

	}
	public virtual System.Exception WrappedThrowable
	{
	    get {
		return wrapped;
	    }

	}
	public virtual System.String ReferenceName
	{
	    get {
		return referenceName;
	    }

	    set {
		referenceName = value;
	    }

	}
	private System.String methodName = "";
	private System.String referenceName = "";
	//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
	private System.Exception wrapped = null;

	/// <summary>  CTOR - wraps the passed in exception for
	/// examination later
	/// *
	/// </summary>
	/// <param name="message">
	/// </param>
	/// <param name="e">Throwable that we are wrapping
	/// </param>
	/// <param name="methodName">name of method that threw the exception
	///
	/// </param>
	//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'
	public MethodInvocationException(System.String message, System.Exception e, System.String methodName):base(message) {
	    this.wrapped = e;
	    this.methodName = methodName;
	}

	/// <summary>  Returns the name of the method that threw the
	/// exception
	/// *
	/// </summary>
	/// <returns>String name of method
	///
	/// </returns>

	/// <summary>  returns the wrapped Throwable that caused this
	/// MethodInvocationException to be thrown
	///
	/// </summary>
	/// <returns>Throwable thrown by method invocation
	///
	/// </returns>
	//UPGRADE_NOTE: Exception 'java.lang.Throwable' was converted to ' ' which has different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1100"'

	/// <summary>  Sets the reference name that threw this exception
	/// *
	/// </summary>
	/// <param name="reference">name of reference
	///
	/// </param>

	/// <summary>  Retrieves the name of the reference that caused the
	/// exception
	/// *
	/// </summary>
	/// <returns>name of reference
	///
	/// </returns>
    }
}
