namespace NVelocity.App.Events
{
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
			if (context is IInternalEventContext)
			{
				IInternalEventContext iec = (IInternalEventContext) context;
				iec.AttachEventCartridge(this);
				return true;
			}
			else
				return false;
		}
	}
}
