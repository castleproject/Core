namespace Castle.DynamicProxy.Test.Classes
{
	using System;
	using System.Collections;

	public enum SByteEnum : sbyte
	{
		One,
		Two
	}

	public class ClassWithSByteEnum
	{
		SByteEnum en;

		public virtual SByteEnum En
		{
			get { return en; }
			set { en = value; }
		}
	}
}
