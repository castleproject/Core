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

namespace Castle.MonoRail.Framework.Views.NVelocity.Tests
{
	using System;
	using System.IO;

	using NUnit.Framework;

	using Castle.MonoRail.Engine.Tests;

	[TestFixture]
	public class SmartControllerTestCase : AbstractCassiniTestCase
	{
		[Test]
		public void StringMethod()
		{
			string url = "/smart/stringmethod.rails?name=hammett";
			string expected = "incoming hammett";

			Execute(url, expected);

			url = "/smart/stringmethod.rails?NAME=hammett";
			expected = "incoming hammett";

			Execute(url, expected);
		}

		[Test]
		public void Complex()
		{
			string url = "/smart/complex.rails?strarg=hammett&intarg=1&strarray=a";
			string expected = "incoming hammett 1 a";

			Execute(url, expected);

			url = "/smart/complex.rails?strarg=&intarg=&strarray=a,b,c";
			expected = "incoming  0 a,b,c";

			Execute(url, expected);
		}

		[Test]
		public void SimpleBind()
		{
			string url = "/smart/SimpleBind.rails?name=hammett&itemcount=11&price=20";
			string expected = "incoming hammett 11 20";

			Execute(url, expected);

			url = "/smart/SimpleBind.rails?name=hammett";
			expected = "incoming hammett 0 0";

			Execute(url, expected);
		}

		[Test]
		public void ComplexBind()
		{
			string url = "/smart/ComplexBind.rails?name=hammett&itemcount=11&price=20&id=1&contact.email=x&contact.phone=y";
			string expected = "incoming hammett 11 20 1 x y";

			Execute(url, expected);
		}

		[Test]
		public void ComplexBindWithPrefix()
		{
			string url = "/smart/ComplexBindWithPrefix.rails?name=hammett&itemcount=11&price=20&person.id=1&person.contact.email=x&person.contact.phone=y";
			string expected = "incoming hammett 11 20 1 x y";

			Execute(url, expected);
		}

		[Test]
		[Ignore("Crashes NUnit with '.', hexadecimal value 0x00, is an invalid character")]
		public void FillingBehavior1()
		{
			string url = "/smart/FillingBehavior.rails?name=someone&date1day=11&date1month=10&date1year=2005";
			string expected = "incoming someone 11/10/2005 " + DateTime.Now.AddDays(1).ToShortDateString();

			Execute(url, expected);
		}

		[Test]
		[Ignore("Crashes NUnit with '.', hexadecimal value 0x00, is an invalid character")]
		public void FillingBehavior2()
		{
			string url = "/smart/FillingBehavior.rails";
			string expected = "incoming hammett " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.AddDays(1).ToShortDateString();

			Execute(url, expected);
		}

		protected override String ObtainPhysicalDir()
		{
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"..\TestSiteNVelocity" );
		}
	}
}
