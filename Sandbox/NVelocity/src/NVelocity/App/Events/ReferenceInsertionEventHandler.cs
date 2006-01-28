namespace NVelocity.App.Events
{
	using System;
	using System.Collections;

	public class ReferenceInsertionEventArgs : EventArgs
	{
		Stack referenceStack;
		Object originalValue, newValue;
		String rootString;

		public ReferenceInsertionEventArgs(Stack referenceStack, String rootString, Object value)
		{
			this.rootString = rootString;
			this.referenceStack = referenceStack;
			this.originalValue = this.newValue = value;
		}
		
		public Stack GetCopyOfReferenceStack()
		{
			return (Stack) referenceStack.Clone();
		}

		public String RootString
		{
			get { return rootString; }
		}

		public Object OriginalValue
		{
			get { return originalValue; }
		}

		public Object NewValue
		{
			get { return newValue; }
			set { newValue = value; }
		}
	}
	
	/// <summary>
	/// Reference 'Stream insertion' event handler.  Called with object
	/// that will be inserted into stream via value.toString().
	/// Make sure you return an Object that will toString() without throwing an exception.
	/// </summary>
	public delegate void ReferenceInsertionEventHandler(Object sender, ReferenceInsertionEventArgs e);
}