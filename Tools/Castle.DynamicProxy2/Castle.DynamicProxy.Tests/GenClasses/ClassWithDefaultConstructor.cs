using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.DynamicProxy.Tests.GenClasses
{
	public class ClassWithDefaultConstructor
	{
		private int number;


		public int Number
		{
			get { return number; }
			set { number = value; }
		}
	}
}
