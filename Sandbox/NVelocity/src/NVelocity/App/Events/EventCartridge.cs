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
	using System.Collections;
	using NVelocity.Context;
	using NVelocity.Exception;

	/// <summary>
	/// 'Package' of event handlers...
	/// </summary>
	public class EventCartridge
	{
		public event ReferenceInsertionEventHandler ReferenceInsertion;
		public event NullSetEventHandler NullSet;
		public event MethodExceptionEventHandler MethodExceptionEvent;
		
		/// <summary>
		/// Called during Velocity merge before a reference value will
		/// be inserted into the output stream.
		/// </summary>
		/// <param name="referenceStack">the stack of objects used to reach this reference</param>
		/// <param name="reference">reference from template about to be inserted</param>
		/// <param name="value"> value about to be inserted (after toString() )</param>
		/// <returns>
		/// Object on which toString() should be called for output.
		/// </returns>
		internal Object ReferenceInsert(Stack referenceStack, String reference, Object value)
		{
			if (this.ReferenceInsertion != null)
			{
				ReferenceInsertionEventArgs args = new ReferenceInsertionEventArgs(referenceStack, reference, value);
				this.ReferenceInsertion(this, args);
				value = args.NewValue;
			}

			return value;
		}

		/// <summary>
		/// Called during Velocity merge to determine if when
		/// a #set() results in a null assignment, a warning
		/// is logged.
		/// </summary>
		/// <returns>true if to be logged, false otherwise</returns>
		internal bool ShouldLogOnNullSet(String lhs, String rhs)
		{
			if (this.NullSet == null)
				return true;

			NullSetEventArgs e = new NullSetEventArgs(lhs, rhs);
			this.NullSet(this, e);

			return e.ShouldLog;
		}

		/// <summary>
		/// Called during Velocity merge if a reference is null
		/// </summary>
		/// <param name="claz">Class that is causing the exception</param>
		/// <param name="method">method called that causes the exception</param>
		/// <param name="e">Exception thrown by the method</param>
		/// <returns>Object to return as method result</returns>
		/// <exception cref="Exception">exception to be wrapped and propogated to app</exception>
		internal Object HandleMethodException(Type claz, String method, Exception e)
		{
			// if we don't have a handler, just throw what we were handed
			if (this.MethodExceptionEvent == null)
				throw new VelocityException(e.Message, e);

			MethodExceptionEventArgs mea = new MethodExceptionEventArgs(claz, method, e);
			this.MethodExceptionEvent(this, mea);

			if (mea.ValueToRender != null)
				return mea.ValueToRender;
			else
				throw new VelocityException(e.Message, e);
		}

		/// <summary>
		/// Attached the EventCartridge to the context
		/// </summary>
		/// <param name="context">context to attach to</param>
		/// <returns>true if successful, false otherwise</returns>
		public bool AttachToContext(IContext context)
		{
			if (context is InternalEventContext)
			{
				InternalEventContext iec = (InternalEventContext) context;
				iec.AttachEventCartridge(this);
				return true;
			}
			else
				return false;
		}
	}
}