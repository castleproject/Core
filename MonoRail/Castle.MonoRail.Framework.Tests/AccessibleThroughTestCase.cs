// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using NUnit.Framework;

	#region Integration Tests
	[TestFixture]
	public class AccessibleThroughTestCase : AbstractTestCase
	{
		[Test]
		public void AccessibleThroughPostVerbByPost()
		{
			DoPost("home/PostOnlyMethod.rails");
			AssertSuccess();
		}

		[Test]
		public void AccessibleThroughPostVerbByGet()
		{
			DoGet("home/PostOnlyMethod.rails");
			AssertReplyContains("Access to the action [PostOnlyMethod] on controller [home] is not allowed by the http verb [GET].");
		}

		[Test]
		public void AccessibleThroughGetVerbByPost()
		{
			DoPost("home/GetOnlyMethod.rails");
			AssertReplyContains("Access to the action [GetOnlyMethod] on controller [home] is not allowed by the http verb [POST].");
		}

		[Test]
		public void AccessibleThroughGetVerbByGet()
		{
			DoGet("home/GetOnlyMethod.rails");
			AssertSuccess();
		}
	}
	#endregion

	#region Unit Tests
	[TestFixture]
	public class AccessibleThroughUnitTests
	{
		[Test]
		public void ForHttpMethod_WhenOnlyGetIsAllowedAndPostIsTried_IsFalse()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Get);
			Assert.IsFalse(attrib.ForHttpMethod(Verb.Post));
		}

		[Test]
		public void ForHttpMethod_WhenOnlyPostIsAllowedAndGetIsTried_IsFalse()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Post);
			Assert.IsFalse(attrib.ForHttpMethod(Verb.Get));
		}

		[Test]
		public void ForHttpMethod_WhenGetIsAllowedAndGetIsTried_IsTrue()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Get);
			Assert.IsTrue(attrib.ForHttpMethod(Verb.Get));
		}

		[Test]
		public void ForHttpMethod_WhenPostIsAllowedAndPostIsTried_IsTrue()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Post);
			Assert.IsTrue(attrib.ForHttpMethod(Verb.Post));
		}

		[Test]
		public void ForHttpMethod_WhenPostAndGetIsAllowedAndPostIsTried_IsTrue()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Post | Verb.Get);
			Assert.IsTrue(attrib.ForHttpMethod(Verb.Post));
		}

		[Test]
		public void ForHttpMethod_WhenPostAndGetIsAllowedAndGetIsTried_IsTrue()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Post | Verb.Get);
			Assert.IsTrue(attrib.ForHttpMethod(Verb.Get));
		}

		[Test]
		public void ForHttpMethod_WhenPostAndGetIsAllowedAndDeleteIsTried_IsFalse()
		{
			AccessibleThroughAttribute attrib = new AccessibleThroughAttribute(Verb.Post | Verb.Get);
			Assert.IsFalse(attrib.ForHttpMethod(Verb.Delete));
		}

		[Test]
		public void HttpMethodToVerb_WithSingleMethod_ConvertsToVerb()
		{
			Assert.AreEqual(Verb.Get, AccessibleThroughAttribute.HttpMethodToVerb("GET"));
		}

		[Test]
		public void HttpMethodToVerb_WithMultipleMethods_ConvertsToVerb()
		{
			Assert.AreEqual(Verb.Get | Verb.Post, AccessibleThroughAttribute.HttpMethodToVerb("GET, POST"));
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void HttpMethodToVerb_WhenInvalidVerbUsed_Throws()
		{
			AccessibleThroughAttribute.HttpMethodToVerb("GOOSE");
			Assert.Fail();
		}
	}
	#endregion
}
