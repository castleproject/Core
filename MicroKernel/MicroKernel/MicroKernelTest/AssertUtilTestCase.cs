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

namespace Castle.MicroKernel.Test
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for AssertUtilTestCase.
	/// </summary>
	[TestFixture]
	public class AssertUtilTestCase : Assertion
	{
		[Test]
		public void ArgumentNotNull()
		{
			try
			{
				AssertUtil.ArgumentNotNull( "", "value" );
			}
			catch(ArgumentNullException)
			{
				Fail("The argument wasn't null");
			}

			try
			{
				AssertUtil.ArgumentNotNull( null, "value" );
				Fail("The argument was null");
			}
			catch(ArgumentNullException)
			{
			}
		}

		[Test]
		public void ArgumentMustNotBeInterface()
		{
			try
			{
				AssertUtil.ArgumentMustNotBeInterface( typeof(String), "value" );
			}
			catch(ArgumentNullException)
			{
				Fail("The argument wasn't an interface");
			}

			try
			{
				AssertUtil.ArgumentMustNotBeInterface( typeof(IList), "value" );
				Fail("The argument was an interface");
			}
			catch(ArgumentNullException)
			{
			}
		}

		[Test]
		public void ArgumentMustNotBeAbstract()
		{
			try
			{
				AssertUtil.ArgumentMustNotBeAbstract( typeof(String), "value" );
			}
			catch(ArgumentNullException)
			{
				Fail("The argument wasn't invalid");
			}

			try
			{
				AssertUtil.ArgumentMustNotBeAbstract( typeof(AssertUtil), "value" );
				Fail("The argument was invalid");
			}
			catch(ArgumentNullException)
			{
			}
		}

		[Test]
		public void ArgumentMustBeInterface()
		{
			try
			{
				AssertUtil.ArgumentMustBeInterface( typeof(IList), "value" );
			}
			catch(ArgumentNullException)
			{
				Fail("The argument wasn't invalid");
			}

			try
			{
				AssertUtil.ArgumentMustBeInterface( typeof(AssertUtil), "value" );
				Fail("The argument was invalid");
			}
			catch(ArgumentNullException)
			{
			}
		}
	}
}
