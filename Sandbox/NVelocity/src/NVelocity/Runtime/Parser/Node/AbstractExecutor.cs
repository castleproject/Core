namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.Reflection;

	/// <summary> Abstract class that is used to execute an arbitrary
	/// method that is in introspected. This is the superclass
	/// for the GetExecutor and PropertyExecutor.
	/// </summary>
	public abstract class AbstractExecutor
	{
		protected internal RuntimeLogger rlog = null;

		/// <summary> Method to be executed.
		/// </summary>
		protected internal MethodInfo method = null;

		protected internal PropertyInfo property = null;

		/// <summary> Execute method against context.
		/// </summary>
		public abstract Object execute(Object o);

		/// <summary> Tell whether the executor is alive by looking
		/// at the value of the method.
		/// </summary>
		public bool isAlive()
		{
			return (method != null || property != null);
		}

		public MethodInfo Method
		{
			get { return method; }
		}

	}
}