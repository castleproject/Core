// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.MicroKernel.SubSystems.Conversion;


	/// <summary>
	/// Summary description for DefaultConversionManagerTestCase.
	/// </summary>
	[TestFixture]
	public class DefaultConversionManagerTestCase
	{
		DefaultConversionManager conversionMng = new DefaultConversionManager();

		[Test]
		public void PerformConversionInt()
		{
			Assert.AreEqual(100, conversionMng.PerformConversion("100", typeof(int)));
			Assert.AreEqual(1234, conversionMng.PerformConversion("1234", typeof(int)));
		}

		[Test]
		public void PerformConversionChar()
		{
			Assert.AreEqual('a', conversionMng.PerformConversion("a", typeof(Char)));
		}

		[Test]
		public void PerformConversionBool()
		{
			Assert.AreEqual(true, conversionMng.PerformConversion("true", typeof(bool)));
			Assert.AreEqual(false, conversionMng.PerformConversion("false", typeof(bool)));
		}
		
		[Test]
		public void PerformConversionType()
		{
			Assert.AreEqual(typeof(DefaultConversionManagerTestCase), 
				conversionMng.PerformConversion(
					"Castle.MicroKernel.Tests.DefaultConversionManagerTestCase, Castle.MicroKernel.Tests", 
					typeof(Type)));
		}
	}
}
