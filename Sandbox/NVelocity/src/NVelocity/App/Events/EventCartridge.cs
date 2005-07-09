namespace NVelocity.App.Events
{
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
    using System;
    using NVelocity.Context;

    /// <summary>  'Package' of event handlers...
    /// *
    /// </summary>
    /// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author> <a href="mailto:j_a_fernandez@yahoo.com">Jose Alberto Fernandez</a>
    /// </author>
    /// <version> $Id: EventCartridge.cs,v 1.4 2003/10/27 13:54:07 corts Exp $
    ///
    /// </version>
    public class EventCartridge : ReferenceInsertionEventHandler, NullSetEventHandler, MethodExceptionEventHandler {
	private ReferenceInsertionEventHandler rieh = null;
	private NullSetEventHandler nseh = null;
	private MethodExceptionEventHandler meeh = null;

	/// <summary>  Adds an event handler(s) to the Cartridge.  This method
	/// will find all possible event handler interfaces supported
	/// by the passed in object.
	/// *
	/// </summary>
	/// <param name="ev">object impementing a valid EventHandler-derived interface
	/// </param>
	/// <returns>true if a supported interface, false otherwise or if null
	///
	/// </returns>
	public virtual bool addEventHandler(EventHandler ev) {
	    if (ev == null) {
		return false;
	    }

	    bool found = false;

	    if (ev is ReferenceInsertionEventHandler) {
		rieh = (ReferenceInsertionEventHandler) ev;
		found = true;
	    }

	    if (ev is NullSetEventHandler) {
		nseh = (NullSetEventHandler) ev;
		found = true;
	    }

	    if (ev is MethodExceptionEventHandler) {
		meeh = (MethodExceptionEventHandler) ev;
		found = true;
	    }

	    return found;
	}

	/// <summary>  Removes an event handler(s) from the Cartridge.  This method
	/// will find all possible event handler interfaces supported
	/// by the passed in object and remove them.
	/// *
	/// </summary>
	/// <param name="ev">object impementing a valid EventHandler-derived interface
	/// </param>
	/// <returns>true if a supported interface, false otherwise or if null
	///
	/// </returns>
	public virtual bool removeEventHandler(EventHandler ev) {
	    if (ev == null) {
		return false;
	    }

	    bool found = false;

	    if (ev == rieh) {
		rieh = null;
		found = true;
	    }

	    if (ev == nseh) {
		nseh = null;
		found = true;
	    }

	    if (ev == meeh) {
		meeh = null;
		found = true;
	    }

	    return found;
	}

	/// <summary>  Implementation of ReferenceInsertionEventHandler method
	/// <code>referenceInsert()</code>.
	/// *
	/// Called during Velocity merge before a reference value will
	/// be inserted into the output stream.
	/// *
	/// </summary>
	/// <param name="reference">reference from template about to be inserted
	/// </param>
	/// <param name="value"> value about to be inserted (after toString() )
	/// </param>
	/// <returns>Object on which toString() should be called for output.
	///
	/// </returns>
	public virtual System.Object referenceInsert(System.String reference, System.Object value_Renamed) {
	    if (rieh == null) {
		return value_Renamed;
	    }

	    return rieh.referenceInsert(reference, value_Renamed);
	}

	/// <summary>  Implementation of NullSetEventHandler method
	/// <code>shouldLogOnNullSet()</code>.
	/// *
	/// Called during Velocity merge to determine if when
	/// a #set() results in a null assignment, a warning
	/// is logged.
	/// *
	/// </summary>
	/// <param name="reference">reference from template about to be inserted
	/// </param>
	/// <returns>true if to be logged, false otherwise
	///
	/// </returns>
	public virtual bool shouldLogOnNullSet(System.String lhs, System.String rhs) {
	    if (nseh == null) {
		return true;
	    }

	    return nseh.shouldLogOnNullSet(lhs, rhs);
	}

	/// <summary>  Implementation of MethodExceptionEventHandler  method
	/// <code>methodException()</code>.
	/// *
	/// Called during Velocity merge if a reference is null
	/// *
	/// </summary>
	/// <param name="claz"> Class that is causing the exception
	/// </param>
	/// <param name="method">method called that causes the exception
	/// </param>
	/// <param name="e">Exception thrown by the method
	/// </param>
	/// <returns>Object to return as method result
	/// @throws exception to be wrapped and propogated to app
	///
	/// </returns>
	public virtual System.Object methodException(System.Type claz, System.String method, System.Exception e) {
	    /*
	    *  if we don't have a handler, just throw what we were handed
	    */
	    if (meeh == null) {
		throw e;
	    }

	    /*
	    *  otherwise, call it..
	    */
	    return meeh.methodException(claz, method, e);
	}

	/// <summary>  Attached the EventCartridge to the context
	/// *
	/// Final because not something one should mess with lightly :)
	/// *
	/// </summary>
	/// <param name="context">context to attach to
	/// </param>
	/// <returns>true if successful, false otherwise
	///
	/// </returns>
	public bool attachToContext(IContext context) {
	    if (context is InternalEventContext) {
		InternalEventContext iec = (InternalEventContext) context;

		iec.AttachEventCartridge(this);

		return true;
	    } else {
		return false;
	    }
	}
    }
}
