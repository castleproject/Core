using System.Runtime.CompilerServices;
// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Rook.RuntimeSupport.Tests.OrdinaryClasses
{
	using System;


	public class MySimpleClass
	{
		private static readonly Dispatcher staticDisp = new Dispatcher( typeof(MySimpleClass).GetMethod("MyStaticMethod") );
		private readonly Dispatcher instanceDisp = new Dispatcher( typeof(MySimpleClass).GetMethod("SomeMethod") );

		public MySimpleClass()
		{
		}

		public void SomeMethod()
		{
			Console.WriteLine("SomeMethod called");
		}

		public static void MyStaticMethod()
		{
			Console.WriteLine("MyStaticMethod called");
		}

		public static object StaticSend(String symbol, params object[] args)
		{
			bool matchFound = false;
			object ret = staticDisp.Send(null, ref matchFound, symbol, args);
			
			if (matchFound)
			{
				return ret;
			}

			return method_missing(null, symbol, args);
		}

		public object InstanceSend(String symbol, params object[] args)
		{
			bool matchFound = false;

			object ret = instanceDisp.Send(this, ref matchFound, symbol, args);
			
			if (matchFound)
			{
				return ret;
			}

			return method_missing(null, symbol, args);
		}

		public static object method_missing(object instance, String symbol, params object[] args)
		{
			
		}
	}
}
