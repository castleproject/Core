namespace Castle.DynamicProxy.Test.Classes
{
	using System;

	/// <summary>
	/// Summary description for ClassMarshalByRef.
	/// </summary>
	public class ClassMarshalByRef : MarshalByRefObject
	{
		public ClassMarshalByRef()
		{
		}

		public int Pong(int i)
		{
			return i;
		}

		public object Ping(object o)
		{
			return o;	
        }
	}
}
