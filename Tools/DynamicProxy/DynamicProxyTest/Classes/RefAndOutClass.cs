namespace Castle.DynamicProxy.Test.Classes
{
	using System;

	/// <summary>
	/// Summary description for RefAndOutClass.
	/// </summary>
	public class RefAndOutClass
	{
		public RefAndOutClass()
		{
		}

		public void RefPongOne(ref int i)
		{
			i = 1;
		}

		public void OutPingTwo(out int i)
		{
			i = 2;
		}
	}
}
