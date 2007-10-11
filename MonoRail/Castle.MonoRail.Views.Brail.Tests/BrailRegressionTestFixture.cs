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

namespace Castle.MonoRail.Views.Brail.Tests
{
	using System.Collections;
	using NUnit.Framework;

	[TestFixture]
	public class BrailRegressionTestFixture : BaseViewOnlyTestFixture
	{
		[Test]
		public void CanCompareToNullableParameter()
		{
			Hashtable args = new Hashtable();
			args["myVariable"] = "Hello";
			string view = ProcessView(args, "regressions/CanCompareToNullableParameter");
			Assert.AreEqual("Eq", view);
		}

		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter()
		{
			string view = ProcessView("regressions/questionMarkOp_if");
			Assert.AreEqual("", view);
		}

		[Test]
		public void CanUseQuestionMarkOperatorInIfStatementToValidatePresenceOfParameter_WhenPassed()
		{
			Hashtable args = new Hashtable();
			args["errorMsg"] = "Hello"; 
			string view = ProcessView(args, "regressions/questionMarkOp_if_whenPassed");
			Assert.AreEqual("Hello", view);
		}

		[Test]
		public void CanUseQuestionMarkOperatorInUnlessStatementToValidatePresenceOfParameter()
		{
			string view = ProcessView("regressions/questionMarkOp_unless");
			Assert.AreEqual("\r\nError does not exist\r\n", view);
		}

        [Test]
        public void CanUseQuestionMarkOperatorInUnlessStatementToValidatePresenceOfParameter_whenPassed()
        {
            Hashtable args = new Hashtable();
            args["errorMsg"] = "Hello";
            string view = ProcessView(args, "regressions/questionMarkOp_unless_whenPassed");
            Assert.AreEqual("", view);
        }


		[Test]
		public void HtmlEncodingStringInterpolation()
		{
			Hashtable args = new Hashtable();
			args["htmlCode"] = "<script>alert('a');</script>";
			string view = ProcessView(args, "regressions/HtmlEncodingStringInterpolation");
			Assert.AreEqual("&lt;script&gt;alert('a');&lt;/script&gt;", view);
		}

		[Test]
		public void StringInterpolationInCodeBlockWillNotBeEscaped()
		{
			Hashtable args = new Hashtable();
			args["htmlCode"] = "<script>alert('a');</script>";
			string view = ProcessView(args, "regressions/StringInterpolationInCodeBlockWillNotBeEscaped");
			Assert.AreEqual("Code <script>alert('a');</script>", view);
		}
	}
}