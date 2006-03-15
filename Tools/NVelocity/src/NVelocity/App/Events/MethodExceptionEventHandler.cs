namespace NVelocity.App.Events
{
	using System;

	/// <summary>
	/// Called when a method throws an exception.  This gives the
	/// application a chance to deal with it and either
	/// return something nice, or throw.
	/// 
	/// Please return what you want rendered into the output stream.
	/// </summary>
	public delegate void MethodExceptionEventHandler(Object sender, MethodExceptionEventArgs e);

	public class MethodExceptionEventArgs : EventArgs
	{
		Object valueToRender;
		Exception exceptionThrown;
		Type targetClass;
		String targetMethod;

		public MethodExceptionEventArgs(Type targetClass, String targetMethod, Exception exceptionThrown)
		{
			this.targetClass = targetClass;
			this.targetMethod = targetMethod;
			this.exceptionThrown = exceptionThrown;
		}
		
		public Object ValueToRender
		{
			get { return valueToRender; }
			set { valueToRender = value; }
		}

		public Exception ExceptionThrown { get { return exceptionThrown; } }
		public Type TargetClass { get { return targetClass; } }
		public String TargetMethod { get { return targetMethod; } }
	}
}