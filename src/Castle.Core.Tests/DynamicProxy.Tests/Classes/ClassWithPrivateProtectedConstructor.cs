//----------------------------------------------------
// Copyright 2020 Epic Systems Corporation
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle.DynamicProxy.Tests.Classes
{
	public class ClassWithPrivateProtectedConstructor
	{
		private protected ClassWithPrivateProtectedConstructor()
		{
			_someString = "Something";
		}

		private string _someString = string.Empty;

		public string SomeString
		{
			get { return _someString; }
			set { _someString = value; }
		}
	}
}
